using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Commands;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Unturned.Users;
using ShopsUI.API.Shops;
using ShopsUI.Configuration;
using SilK.Unturned.Extras.Configuration;
using SilK.Unturned.Extras.Events;
using SilK.Unturned.Extras.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI.UI
{
    public abstract class ShopsUISessionBase<TShopData, TShop, TShopDirectory, TAsset> : SingleEffectUISession,
        IInstanceAsyncEventListener<BalanceUpdatedEvent>
        where TShop : IShop<TShopData>
        where TShopData : IShopData
        where TShopDirectory : IShopDirectory<TShop, TShopData>
    {
        public class ShopInfo
        {
            public TShopData ShopData { get; }

            public TAsset Asset { get; }

            public string SearchName { get; }

            public string? ImageUrl { get; }

            public ShopInfo(TShopData shopData, TAsset asset, string searchName, string? imageUrl)
            {
                ShopData = shopData;
                Asset = asset;
                SearchName = searchName;
                ImageUrl = imageUrl;
            }
        }

        public override string Id => "SilK.ShopsUI";

        public override ushort EffectId { get; }

        protected readonly IConfigurationParser<ShopsUIConfig> Configuration;
        protected readonly IStringLocalizer StringLocalizer;
        protected readonly IEconomyProvider EconomyProvider;
        protected readonly TShopDirectory ShopDirectory;
        protected readonly ILogger Logger;

        private readonly UIList<ShopInfo> _shopsUIList;
        private readonly UIList<IShopCategory<TShopData>> _categoriesUIList;

        private DateTime _lastCategory = DateTime.MinValue;
        private DateTime _lastBuy = DateTime.MinValue;
        private DateTime _lastSell = DateTime.MinValue;

        private int _shownNotifications;
        private int _notificationSuccessIndex;
        private int _notificationErrorIndex;
        private readonly Queue<(string message, bool isError)> _notificationsWaiting = new();

        private string? _searchFilter;
        private List<ShopInfo> _currentShopsList = new();

        protected ShopsUISessionBase(UnturnedUser user, IServiceProvider serviceProvider) : base(user, serviceProvider)
        {
            Configuration = serviceProvider.GetRequiredService<IConfigurationParser<ShopsUIConfig>>();
            StringLocalizer = serviceProvider.GetRequiredService<IStringLocalizer>();
            EconomyProvider = serviceProvider.GetRequiredService<IEconomyProvider>();
            ShopDirectory = serviceProvider.GetRequiredService<TShopDirectory>();

            var loggerType = typeof(ILogger<>).MakeGenericType(GetType());
            Logger = (ILogger)serviceProvider.GetRequiredService(loggerType);

            _shopsUIList = new UIList<ShopInfo>(UIElements.MaxShopItems, DisplayShopItem, DisplayNoShopItem);
            _categoriesUIList = new UIList<IShopCategory<TShopData>>(UIElements.MaxCategories, DisplayCategory, DisplayNoCategory);

            EffectId = Configuration.Instance.UI.MainEffect;
        }

        protected abstract Task<TAsset?> GetAsset(ushort assetId);

        protected abstract Task<string?> GetImageUrl(ushort assetId);

        protected abstract void DisplayShopItem(ShopInfo shopInfo, int index);

        protected virtual Task<IShopCategory<TShopData>?> GetDefaultCategory() =>
            Task.FromResult<IShopCategory<TShopData>?>(null);

        protected virtual UniTask OnShopBuy(TShop shop, TAsset asset) => UniTask.CompletedTask;

        protected virtual UniTask OnShopSell(TShop shop, TAsset asset) => UniTask.CompletedTask;

        protected abstract string GetAssetName(TAsset asset);

        protected virtual bool ShouldShowShop(TShopData shopData) => true;

        protected override async UniTask OnStartAsync()
        {
            var categories = (await GetCategories()).ToList();

            IEnumerable<ShopInfo>? shops = null;

            var firstCategory = categories.FirstOrDefault();
            if (firstCategory?.Shops != null)
            {
                shops = await GetShopInfos(firstCategory.Shops);
            }

            var balance = await EconomyProvider.GetBalanceAsync(User.Id, User.Type);

            await UniTask.SwitchToMainThread();

            SubscribeButtonClick(UIElements.CloseButton, async _ => await EndAsync());
            SubscribeButtonClick(UIElements.ShopCategoryButtons.Pattern, OnCategoryClicked, true);
            SubscribeTextInputted(UIElements.ShopSearchInputField, OnSearchInput);
            
            if (typeof(IBuyShop<TShopData>).IsAssignableFrom(typeof(TShop)))
            {
                SubscribeButtonClick(UIElements.ShopItemBuyButtons.Pattern, OnShopBuyClicked, true);
            }

            if (typeof(ISellShop<TShopData>).IsAssignableFrom(typeof(TShop)))
            {
                SubscribeButtonClick(UIElements.ShopItemSellButtons.Pattern, OnShopSellClicked, true);
            }

            SendUIEffect();

            await ShowShopTitle(balance);
            await ShowShopIcon();
            await ShowShopSearchText();

            await ShowCategories(categories);

            await SetCursor(true);

            await UniTask.SwitchToThreadPool();

            if (shops != null)
            {
                await ShowShops(shops);
            }
        }

        protected override async UniTask OnEndAsync()
        {
            await UniTask.SwitchToMainThread();

            await SetCursor(false);

            ClearEffect();
        }

        private async UniTask ShowShopTitle(decimal? balance = null)
        {
            balance ??= await EconomyProvider.GetBalanceAsync(User.Id, User.Type);
            
            await UniTask.SwitchToMainThread();

            SendText(UIElements.HeaderText,
                StringLocalizer["ui:header",
                    new {EconomyProvider.CurrencyName, EconomyProvider.CurrencySymbol, Balance = balance.Value}]);
        }

        private async UniTask ShowShopIcon()
        {
            await UniTask.SwitchToMainThread();

            if (!string.IsNullOrEmpty(Configuration.Instance.UI.LogoUrl))
            {
                SendImage(UIElements.HeaderImage, Configuration.Instance.UI.LogoUrl!);
            }
        }

        private async UniTask ShowShopSearchText()
        {
            await UniTask.SwitchToMainThread();

            SendText("ShopSearchPlaceholder", StringLocalizer["ui:search"]);
        }

        private async Task<IEnumerable<IShopCategory<TShopData>>> GetCategories()
        {
            var categories = (await ShopDirectory.GetCategoriesWithShopData()).ToList();

            var defaultCategory = await GetDefaultCategory();

            if (defaultCategory != null)
            {
                categories.Insert(0, defaultCategory);
            }

            if (!Configuration.Instance.UI.ShowEmptyCategories)
            {
                categories.RemoveAll(x => x.Shops is { Count: 0 });
            }

            return categories;
        }

        private async UniTask ShowCategories(IEnumerable<IShopCategory<TShopData>> categories)
        {
            await _categoriesUIList.UpdateContents(categories);
        }

        private void DisplayCategory(IShopCategory<TShopData> element, int index)
        {
            SendVisibility(UIElements.ShopCategoryButtons[index], true);

            SendText(UIElements.ShopCategoryTexts[index], StringLocalizer["ui:category", new {Category = element}]);
        }

        private void DisplayNoCategory(int index)
        {
            SendVisibility(UIElements.ShopCategoryButtons[index], false);
        }

        private async Task<IEnumerable<ShopInfo>> GetShopInfos(IEnumerable<TShopData> itemShops)
        {
            var shopInfos = new List<ShopInfo>();

            foreach (var shopData in itemShops)
            {
                var asset = await GetAsset(shopData.Id);

                if (asset == null)
                {
                    continue;
                }

                var imageUrl = await GetImageUrl(shopData.Id);

                var assetName = GetAssetName(asset);

                shopInfos.Add(new ShopInfo(shopData, asset, assetName.ToLower(), imageUrl));
            }

            return shopInfos;
        }

        private async UniTask ShowShops(IEnumerable<ShopInfo> itemShops)
        {
            _currentShopsList = itemShops.ToList();

            _currentShopsList.RemoveAll(x => !ShouldShowShop(x.ShopData));

            Logger.LogDebug($"Showing shops (Count: {_currentShopsList.Count})");

            if (_searchFilter == null)
            {
                await _shopsUIList.UpdateContents(_currentShopsList);
            }
            else
            {
                await _shopsUIList.UpdateContents(_currentShopsList.Where(x =>
                    x.SearchName.IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0));
            }
        }

        private void DisplayNoShopItem(int index) 
        {
            SendVisibility(UIElements.ShopItemResetters[index], true);
        }

        protected async UniTask DisplayNotification(string message, bool isError = false)
        {
            // ReSharper disable VariableHidesOuterVariable
            void ShowNotification(string message, bool isError = false)
            {
                string notification;
                string notificationText;

                if (isError)
                {
                    notification = UIElements.ShopErrorNotifications[_notificationErrorIndex];
                    notificationText = UIElements.ShopErrorNotificationsText[_notificationErrorIndex];

                    _notificationErrorIndex = (_notificationErrorIndex + 1) % UIElements.MaxNotifications;
                }
                else
                {
                    notification = UIElements.ShopSuccessNotifications[_notificationSuccessIndex];
                    notificationText = UIElements.ShopSuccessNotificationsText[_notificationSuccessIndex];

                    _notificationSuccessIndex = (_notificationSuccessIndex + 1) % UIElements.MaxNotifications;
                }

                SendVisibility(notification, true);
                SendText(notificationText, message);
            }

            await UniTask.SwitchToMainThread();

            if (_shownNotifications < UIElements.MaxNotifications)
            {
                _shownNotifications++;

                ShowNotification(message, isError);

                UniTask.RunOnThreadPool(async () =>
                {
                    await UniTask.Delay(UIElements.ShopNotificationDuration);

                    await UniTask.SwitchToMainThread();

                    _shownNotifications--;

                    while (_shownNotifications < 8 && _notificationsWaiting.Count > 0)
                    {
                        var (message, isError) = _notificationsWaiting.Dequeue();

                        _shownNotifications++;
                        ShowNotification(message, isError);
                    }
                }).Forget();
            }
            else
            {
                _notificationsWaiting.Enqueue((message, isError));
            }
        }

        private static bool ShouldContinue(ref DateTime lastAction, float delaySeconds)
        {
            if ((DateTime.Now - lastAction).TotalSeconds < delaySeconds)
            {
                return false;
            }

            lastAction = DateTime.Now;
            return true;
        }

        private async UniTask OnCategoryClicked(string buttonName)
        {
            if (!ShouldContinue(ref _lastCategory, Configuration.Instance.UI.CategoryButtonDelay))
            {
                return;
            }

            await UniTask.SwitchToThreadPool();

            var index = UIElements.ShopCategoryButtons.GetIndex(buttonName);

            var category = _categoriesUIList[index];

            if (category.Shops == null)
            {
                Logger.LogWarning("Category shops were not retrieved when creating UI session");
                return;
            }

            var shops = await GetShopInfos(category.Shops);

            await ShowShops(shops);
        }

        private async UniTask OnShopBuyClicked(string buttonName)
        {
            if (!typeof(IBuyShop<TShopData>).IsAssignableFrom(typeof(TShop)))
            {
                return;
            }

            if (!ShouldContinue(ref _lastBuy, Configuration.Instance.UI.ButtonDelay))
            {
                return;
            }

            await UniTask.SwitchToThreadPool();

            var index = UIElements.ShopItemBuyButtons.GetIndex(buttonName);

            var shopInfo = _shopsUIList[index];

            var shop = await ShopDirectory.GetShop(shopInfo.ShopData);

            if (!((IBuyShop<TShopData>)shop).CanBuy())
            {
                return;
            }

            try
            {
                await OnShopBuy(shop, shopInfo.Asset);
            }
            catch (UserFriendlyException ex)
            {
                await DisplayNotification(ex.Message, true);
            }
        }

        private async UniTask OnShopSellClicked(string buttonName)
        {
            if (!typeof(ISellShop<TShopData>).IsAssignableFrom(typeof(TShop)))
            {
                return;
            }

            if (!ShouldContinue(ref _lastSell, Configuration.Instance.UI.ButtonDelay))
            {
                return;
            }

            await UniTask.SwitchToThreadPool();

            var index = UIElements.ShopItemSellButtons.GetIndex(buttonName);

            var shopInfo = _shopsUIList[index];

            var shop = await ShopDirectory.GetShop(shopInfo.ShopData);

            if (!((ISellShop<TShopData>)shop).CanSell())
            {
                return;
            }

            try
            {
                await OnShopSell(shop, shopInfo.Asset);
            }
            catch (UserFriendlyException ex)
            {
                await DisplayNotification(ex.Message, true);
            }
        }

        private async UniTask OnSearchInput(string textInputName, string text)
        {
            await UniTask.SwitchToThreadPool();

            _searchFilter = text;

            await ShowShops(_currentShopsList);
        }

        public async UniTask HandleEventAsync(object? sender, BalanceUpdatedEvent @event)
        {
            if (@event.OwnerType != User.Type || @event.OwnerId != User.Id)
            {
                return;
            }

            await ShowShopTitle(@event.NewBalance);
        }
    }
}
