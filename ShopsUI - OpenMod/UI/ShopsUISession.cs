using Cysharp.Threading.Tasks;
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
using SilK.Unturned.Extras.UI;
using System;
using System.Linq;

namespace ShopsUI.UI
{
    public class ShopsUISession : SingleEffectUISession
    {
        private readonly IConfiguration _configuration;
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

        public override string Id => "SilK.ShopsUI";

        public override ushort EffectId => _configuration.GetValue<ushort>("ui:mainEffect", 29150);

        public string? LogoUrl => _configuration.GetValue("ui:logoUrl", "https://i.imgur.com/t6HbFTN.png");

        public ShopsUISession(
            UnturnedUser user,
            IConfiguration configuration,
            IStringLocalizer stringLocalizer,
            IEconomyProvider economyProvider,
            IShopManager shopManager,
            IItemDirectory itemDirectory,
            IVehicleDirectory vehicleDirectory,
            IServiceProvider serviceProvider) : base(user, serviceProvider)
        {
            _configuration = configuration;
            _stringLocalizer = stringLocalizer;
            _economyProvider = economyProvider;
            _shopManager = shopManager;
            _itemDirectory = itemDirectory;
            _vehicleDirectory = vehicleDirectory;

            _currentPage = -1;
            _elementsShown = 0;
            _shownIds = new ushort[MaxItemsPerPage];
        }

        public UITab CurrentTab { get; private set; }

        protected override async UniTask OnStartAsync()
        {
            var balance = await _economyProvider.GetBalanceAsync(User.Id, User.Type);

            await UniTask.SwitchToMainThread();

            CurrentTab = UITab.None;


            await SetCursor(true);

            SendUIEffect(
                _stringLocalizer["ui:header"],
                _stringLocalizer["ui:items:header"],
                _stringLocalizer["ui:vehicles:header"],
                _stringLocalizer["ui:balance", new
                {
                    Balance = balance,
                    _economyProvider.CurrencyName,
                    _economyProvider.CurrencySymbol
                }]);

            var logoUrl = LogoUrl;

            if (!string.IsNullOrEmpty(logoUrl))
            {
                SendImage("ShopLogo", logoUrl!);
            }
        }

        public async UniTask EndSession()
        {
            await UniTask.SwitchToMainThread();

            ClearEffect();
        }

        private UniTask ShowBalance(decimal balance)
        {
            SendText("ShopBalance", _stringLocalizer["ui:balance", new
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
                (await _shopManager.GetItemShopDatasAsync(x => x
                    .Skip(page * MaxItemsPerPage)
                    .Take(MaxItemsPerPage + 1)))
                .ToList();

            _currentPage = page;

            SendVisibility("PrevPage", _currentPage > 0);
            SendVisibility("NextPage", shops.Count > MaxItemsPerPage);

            _elementsShown = shops.Count > MaxItemsPerPage ? MaxItemsPerPage : shops.Count;

            var i = 0;

            for (; i < _elementsShown; i++)
            {
                var shop = shops[i];

                _shownIds[i] = shops[i].ItemId;

                var asset = await _itemDirectory.FindByIdAsync(shop.ItemId.ToString());

                if (asset == null) throw new Exception($"No item with id '{shop.ItemId}' exists");

                SendText($"ItemName ({i})", _stringLocalizer["ui:items:listing:content", new { ItemAsset = asset }]);

                if (shop.BuyPrice != null)
                {
                    SendText($"BuyItem ({i})", _stringLocalizer["ui:items:listing:buy", new
                    {
                        shop.BuyPrice,
                        _economyProvider.CurrencyName,
                        _economyProvider.CurrencySymbol
                    }]);
                    SendVisibility($"BuyItem ({i})", true);
                }
                else
                {
                    SendVisibility($"BuyItem ({i})", false);
                }

                if (shop.SellPrice != null)
                {
                    SendText($"SellItem ({i})", _stringLocalizer["ui:items:listing:sell", new
                    {
                        shop.SellPrice,
                        _economyProvider.CurrencyName,
                        _economyProvider.CurrencySymbol
                    }]);
                    SendVisibility($"SellItem ({i})", true);
                }
                else
                {
                    SendVisibility($"SellItem ({i})", false);
                }

                SendVisibility($"Item ({i})", true);
            }

            for (; i < MaxItemsPerPage; i++)
            {
                SendVisibility($"Item ({i})", false);
                SendVisibility($"BuyItem ({i})", false);
                SendVisibility($"SellItem ({i})", false);
            }

            SendVisibility("ItemBody", true);
        }

        private async UniTask ShowVehicleShopPage(int page)
        {
            var shops =
                (await _shopManager.GetVehicleShopDatasAsync(x => x
                    .Skip(page * MaxItemsPerPage)
                    .Take(MaxItemsPerPage + 1))).ToList();

            _currentPage = page;

            SendVisibility("PrevPage", _currentPage > 0);
            SendVisibility("NextPage", shops.Count > MaxItemsPerPage);

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

                SendText($"VehicleName ({i})", _stringLocalizer["ui:vehicles:listing:content", new { VehicleAsset = asset }]);

                SendText($"BuyVehicle ({i})", _stringLocalizer["ui:vehicles:listing:buy", new
                {
                    shop.BuyPrice,
                    _economyProvider.CurrencyName,
                    _economyProvider.CurrencySymbol
                }]);
                SendVisibility($"BuyVehicle ({i})", true);

                SendVisibility($"Vehicle ({i})", true);
            }

            for (; i < MaxItemsPerPage; i++)
            {
                SendVisibility($"Vehicle ({i})", false);
                SendVisibility($"BuyVehicle ({i})", false);
                SendVisibility($"SellVehicle ({i})", false);
            }

            SendVisibility("VehicleBody", true);
        }

        public async UniTask SetTab(UITab tab)
        {
            if (CurrentTab == tab) return;

            // Clean up current tab
            if (CurrentTab == UITab.Items || CurrentTab == UITab.Vehicles)
            {
                var tabName = CurrentTab == UITab.Items ? "Item" : "Vehicle";

                SendVisibility($"{tabName}Body", false);
                for (var i = 0; i < _elementsShown; i++)
                {
                    SendVisibility($"{tabName} ({i})", false);
                    SendVisibility($"Buy{tabName} ({i})", false);

                    if (CurrentTab == UITab.Items)
                        SendVisibility($"Sell{tabName} ({i})", false);
                }

                if (_currentPage > 0)
                    SendVisibility("PrevPage", false);
                SendVisibility("NextPage", false);

                _elementsShown = 0;
            }

            // Show new tab
            CurrentTab = tab;

            if (CurrentTab == UITab.Items)
            {
                await ShowItemShopPage(0);
                SendVisibility("ItemBody", true);
            }
            else
            {
                await ShowVehicleShopPage(0);
                SendVisibility("VehicleBody", true);
            }
        }

        public UniTask ShowAlert(string message, bool isSuccess)
        {
            var key = isSuccess ? "AlertSuccessText" : "AlertFailureText";

            SendVisibility("AlertSuccessText", false);
            SendVisibility("AlertFailureText", false);
            SendText(key, message);
            SendVisibility(key, true);

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

                            await ShowAlert(_stringLocalizer["ui:items:bought"], true);
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

                            await ShowAlert(_stringLocalizer["ui:items:sold"], true);
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
