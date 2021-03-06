using Cysharp.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using OpenMod.API.Commands;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using ShopsUI.API;
using ShopsUI.API.UI;
using Steamworks;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI.UI
{
    public class UISession : IUISession, IAsyncDisposable
    {
        private readonly IStringLocalizer _stringLocalizer;
        private readonly IEconomyProvider _economyProvider;
        private readonly IShopManager _shopManager;
        private readonly IItemDirectory _itemDirectory;
        private readonly IVehicleDirectory _vehicleDirectory;

        private bool _isDisposing;

        private int _currentPage;
        private int _elementsShown;

        private readonly ushort[] _shownIds;

        private const int MaxItemsPerPage = 12;
        
        public ushort EffectId { get; }

        public short EffectKey { get; }

        public UISession(
            IConfiguration configuration,
            IStringLocalizer stringLocalizer,
            IEconomyProvider economyProvider,
            IShopManager shopManager,
            IItemDirectory itemDirectory,
            IVehicleDirectory vehicleDirectory)
        {
            _stringLocalizer = stringLocalizer;
            _economyProvider = economyProvider;
            _shopManager = shopManager;
            _itemDirectory = itemDirectory;
            _vehicleDirectory = vehicleDirectory;

            _currentPage = -1;
            _elementsShown = 0;
            _shownIds = new ushort[MaxItemsPerPage];

            EffectId = configuration.GetValue<ushort>("effects:ui", 29150);
            EffectKey = (short) EffectId;
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposing) return;
            _isDisposing = true;
            
            await EndSession();
        }

        public UnturnedUser User { get; private set; } = null!;

        public CSteamID SteamId => User.SteamId;

        public UITab CurrentTab { get; private set; }

        public delegate void SessionEnded(UnturnedUser user, UISession session);

        public event SessionEnded? OnSessionEnded;

        public async UniTask StartSession(UnturnedUser user, UITab tab = UITab.Items)
        {
            await UniTask.SwitchToMainThread();

            User = user;
            CurrentTab = UITab.None;

            var balance = await _economyProvider.GetBalanceAsync(user.Id, user.Type);

            User.Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal | EPluginWidgetFlags.ForceBlur, true);

            Console.WriteLine($"Showing {EffectId}");

            EffectManager.sendUIEffect(EffectId, EffectKey, SteamId, true,
                _stringLocalizer["ui:header"],
                _stringLocalizer["ui:items:header"],
                _stringLocalizer["ui:vehicles:header"],
                _stringLocalizer["ui:balance", new
                {
                    Balance = balance,
                    _economyProvider.CurrencyName,
                    _economyProvider.CurrencySymbol
                }]);

            await SetTab(tab);
        }

        public async UniTask EndSession()
        {
            await UniTask.SwitchToMainThread();

            EffectManager.askEffectClearByID(EffectId, SteamId);

            User.Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal | EPluginWidgetFlags.ForceBlur, false);

            OnSessionEnded?.Invoke(User, this);
        }

        private void SetVisibility(string childName, bool visible) =>
            EffectManager.sendUIEffectVisibility(EffectKey, SteamId, true, childName, visible);

        private void SetText(string childName, string text) =>
            EffectManager.sendUIEffectText(EffectKey, SteamId, true, childName, text);

        private void SetImage(string childName, string url, bool shouldCache = true, bool forceRefresh = false) =>
            EffectManager.sendUIEffectImageURL(EffectKey, SteamId, true, childName, url, shouldCache, forceRefresh);

        private UniTask ShowBalance(decimal balance)
        {
            SetText("ShopBalance", _stringLocalizer["ui:balance", new
            {
                Balance = balance,
                _economyProvider.CurrencyName,
                _economyProvider.CurrencySymbol
            }]);

            return UniTask.CompletedTask;
        }

        private async UniTask ShowItemShopPage(int page)
        {
            var shops =
                await _shopManager.GetItemShopDatas()
                    .Skip(page * MaxItemsPerPage)
                    .Take(MaxItemsPerPage + 1)
                    .ToListAsync();

            _currentPage = page;

            SetVisibility("PrevPage", _currentPage > 0);
            SetVisibility("NextPage", shops.Count > MaxItemsPerPage);

            _elementsShown = shops.Count > MaxItemsPerPage ? MaxItemsPerPage : shops.Count;

            var i = 0;

            for (; i < _elementsShown; i++)
            {
                var shop = shops[i];

                _shownIds[i] = shops[i].ItemId;

                var asset = await _itemDirectory.FindByIdAsync(shop.ItemId.ToString());

                if (asset == null) throw new Exception($"No item with id '{shop.ItemId}' exists");

                SetText($"ItemName ({i})", _stringLocalizer["ui:items:listing:content", new { ItemAsset = asset }]);

                if (shop.BuyPrice != null)
                {
                    SetText($"BuyItem ({i})", _stringLocalizer["ui:items:listing:buy", new
                    {
                        shop.BuyPrice,
                        _economyProvider.CurrencyName,
                        _economyProvider.CurrencySymbol
                    }]);
                    SetVisibility($"BuyItem ({i})", true);
                }
                else
                {
                    SetVisibility($"BuyItem ({i})", false);
                }

                if (shop.SellPrice != null)
                {
                    SetText($"SellItem ({i})", _stringLocalizer["ui:items:listing:sell", new
                    {
                        shop.SellPrice,
                        _economyProvider.CurrencyName,
                        _economyProvider.CurrencySymbol
                    }]);
                    SetVisibility($"SellItem ({i})", true);
                }
                else
                {
                    SetVisibility($"SellItem ({i})", false);
                }

                SetVisibility($"Item ({i})", true);
            }

            for (; i < MaxItemsPerPage; i++)
            {
                SetVisibility($"Item ({i})", false);
                SetVisibility($"BuyItem ({i})", false);
                SetVisibility($"SellItem ({i})", false);
            }

            SetVisibility("ItemBody", true);
        }

        private async UniTask ShowVehicleShopPage(int page)
        {
            var shops =
                await _shopManager.GetVehicleShopDatas()
                    .Skip(page * MaxItemsPerPage)
                    .Take(MaxItemsPerPage + 1)
                    .ToListAsync();

            _currentPage = page;

            SetVisibility("PrevPage", _currentPage > 0);
            SetVisibility("NextPage", shops.Count > MaxItemsPerPage);

            _elementsShown = shops.Count > MaxItemsPerPage ? MaxItemsPerPage : shops.Count;

            var i = 0;

            for (; i < _elementsShown; i++)
            {
                var shop = shops[i];

                _shownIds[i] = shops[i].VehicleId;

                var idStr = shop.VehicleId.ToString();
                var asset = (await _vehicleDirectory.GetVehicleAssetsAsync()).FirstOrDefault(x =>
                    x.VehicleAssetId.Equals(idStr));

                if (asset == null) throw new Exception($"No vehicle with id '{shop.VehicleId}' exists");

                SetText($"VehicleName ({i})", _stringLocalizer["ui:vehicles:listing:content", new { VehicleAsset = asset }]);

                SetText($"BuyVehicle ({i})", _stringLocalizer["ui:vehicles:listing:buy", new
                {
                    shop.BuyPrice,
                    _economyProvider.CurrencyName,
                    _economyProvider.CurrencySymbol
                }]);
                SetVisibility($"BuyVehicle ({i})", true);

                SetVisibility($"Vehicle ({i})", true);
            }

            for (; i < MaxItemsPerPage; i++)
            {
                SetVisibility($"Vehicle ({i})", false);
                SetVisibility($"BuyVehicle ({i})", false);
                SetVisibility($"SellVehicle ({i})", false);
            }

            SetVisibility("VehicleBody", true);
        }

        public async UniTask SetTab(UITab tab)
        {
            if (CurrentTab == tab) return;

            Console.WriteLine("Cleaning up " + CurrentTab + " " + _elementsShown);

            // Clean up current tab
            if (CurrentTab == UITab.Items || CurrentTab == UITab.Vehicles)
            {
                var tabName = CurrentTab == UITab.Items ? "Item" : "Vehicle";

                SetVisibility($"{tabName}Body", false);
                for (var i = 0; i < _elementsShown; i++)
                {
                    SetVisibility($"{tabName} ({i})", false);
                    SetVisibility($"Buy{tabName} ({i})", false);

                    if (CurrentTab == UITab.Items)
                        SetVisibility($"Sell{tabName} ({i})", false);
                }

                if (_currentPage > 0)
                    SetVisibility("PrevPage", false);
                SetVisibility("NextPage", false);

                _elementsShown = 0;
            }

            // Show new tab
            CurrentTab = tab;

            Console.WriteLine("Setting tab " + CurrentTab);

            if (CurrentTab == UITab.Items)
            {
                await ShowItemShopPage(0);
                SetVisibility("ItemBody", true);
            }
            else
            {
                await ShowVehicleShopPage(0);
                SetVisibility("VehicleBody", true);
            }
        }

        public UniTask ShowAlert(string message, bool isSuccess)
        {
            var key = isSuccess ? "AlertSuccessText" : "AlertFailureText";

            SetVisibility(key, false);
            SetText(key, message);
            SetVisibility(key, true);

            return UniTask.CompletedTask;
        }

        private static bool TryGetIndex(string prefix, string buttonName, out int index)
        {
            index = -1;

            if (!buttonName.StartsWith(prefix)) return false;

            var unparsed = "";

            for (var i = prefix.Length; i < buttonName.Length; i++)
            {
                if (!char.IsNumber(buttonName[i])) break;

                unparsed += buttonName[i];
            }

            return int.TryParse(unparsed, out index);
        }

        private bool _processingButtonClick;

        public async UniTask OnButtonClicked(string buttonName)
        {
            if (_processingButtonClick) return;
            _processingButtonClick = true;

            switch (buttonName)
            {
                case "ExitButton":
                    User.Player.Player.setPluginWidgetFlag(
                        EPluginWidgetFlags.Modal | EPluginWidgetFlags.ForceBlur, false);

                    await UniTask.Delay(350);

                    await EndSession();
                    break;

                case "ItemShops":
                    await SetTab(UITab.Items);
                    break;

                case "VehicleShops":
                    await SetTab(UITab.Vehicles);
                    break;

                case "NextPage":
                    if (CurrentTab == UITab.Items)
                    {
                        await ShowItemShopPage(_currentPage + 1);
                    }
                    else if (CurrentTab == UITab.Vehicles)
                    {
                        await ShowVehicleShopPage(_currentPage + 1);
                    }
                    break;

                case "PrevPage":
                    if (CurrentTab == UITab.Items)
                    {
                        if (_currentPage > 0)
                            await ShowItemShopPage(_currentPage - 1);
                    }
                    else if (CurrentTab == UITab.Vehicles)
                    {
                        if (_currentPage > 0)
                            await ShowVehicleShopPage(_currentPage - 1);
                    }
                    break;
            }

            try
            {
                if (TryGetIndex("BuyItem (", buttonName, out var index))
                {
                    if (CurrentTab == UITab.Items && index < _elementsShown)
                    {
                        var shop = await _shopManager.GetItemShop(_shownIds[index]);

                        if (shop != null && shop.CanBuy())
                        {
                            var balance = await shop.Buy(User);

                            await ShowBalance(balance);
                        }
                    }
                }
                else if (TryGetIndex("SellItem (", buttonName, out index))
                {
                    if (CurrentTab == UITab.Items && index < _elementsShown)
                    {
                        var shop = await _shopManager.GetItemShop(_shownIds[index]);

                        if (shop != null && shop.CanSell())
                        {
                            var balance = await shop.Sell(User);

                            await ShowBalance(balance);
                        }
                    }
                }
                else if (TryGetIndex("BuyVehicle (", buttonName, out index))
                {
                    if (CurrentTab == UITab.Vehicles && index < _elementsShown)
                    {
                        var shop = await _shopManager.GetVehicleShop(_shownIds[index]);

                        if (shop != null)
                        {
                            var balance = await shop.Buy(User);

                            await ShowBalance(balance);

                            await EndSession();
                        }
                    }
                }
            }
            catch (UserFriendlyException ex)
            {
                await ShowAlert(ex.Message, false);
            }

            _processingButtonClick = false;
        }
    }
}
