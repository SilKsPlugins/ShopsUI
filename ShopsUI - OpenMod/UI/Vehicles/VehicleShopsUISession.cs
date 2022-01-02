using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Vehicles;
using OpenMod.Unturned.Users;
using OpenMod.Unturned.Vehicles;
using SDG.Unturned;
using ShopsUI.API.Shops;
using ShopsUI.API.Shops.Vehicles;
using ShopsUI.Shops;
using System;
using System.Threading.Tasks;
using UnturnedImages.API.Vehicles;

namespace ShopsUI.UI.Vehicles
{
    public class VehicleShopsUISession : ShopsUISessionBase<IVehicleShopData, IVehicleShop, IVehicleShopDirectory, IVehicleAsset>
    {
        private readonly IVehicleDirectory _vehicleDirectory;
        private readonly IVehicleImageDirectoryAsync _vehicleImageDirectory;

        public VehicleShopsUISession(UnturnedUser user, IServiceProvider serviceProvider,
            IVehicleDirectory vehicleDirectory,
            IVehicleImageDirectoryAsync vehicleImageDirectory) : base(user, serviceProvider)
        {
            _vehicleDirectory = vehicleDirectory;
            _vehicleImageDirectory = vehicleImageDirectory;
        }

        protected override async Task<IVehicleAsset?> GetAsset(ushort assetId)
        {
            return await _vehicleDirectory.FindByIdAsync(assetId.ToString());
        }

        protected override string GetAssetName(IVehicleAsset asset)
        {
            return asset.VehicleName;
        }

        protected override async Task<string?> GetImageUrl(ushort assetId)
        {
            return await _vehicleImageDirectory.GetVehicleImageUrlAsync(assetId);
        }

        protected override void DisplayShopItem(ShopInfo shopInfo, int index)
        {
            SendVisibility(UIElements.ShopItems[index], true);

            if (!string.IsNullOrEmpty(shopInfo.ImageUrl))
            {
                SendVisibility(UIElements.ShopItemImages[index], true);
                SendImage(UIElements.ShopItemImages[index], shopInfo.ImageUrl!);
            }

            SendText(UIElements.ShopItemNames[index],
                StringLocalizer["ui:vehicles:listing:content",
                    new
                    {
                        VehicleAsset = shopInfo.Asset,
                        EconomyProvider.CurrencyName,
                        EconomyProvider.CurrencySymbol
                    }]);

            SendVisibility(UIElements.ShopItemBuyButtons[index], true);
            SendText(UIElements.ShopItemBuyPriceTexts[index],
                StringLocalizer["ui:vehicles:listing:buy",
                    new
                    {
                        VehicleAsset = shopInfo.Asset,
                        EconomyProvider.CurrencyName,
                        EconomyProvider.CurrencySymbol,
                        shopInfo.ShopData.BuyPrice
                    }]);

            if (Configuration.Instance.UI.BackgroundsEnabled && shopInfo.Asset is UnturnedVehicleAsset vehicleAsset)
            {
                var background = vehicleAsset.VehicleAsset.rarity switch
                {
                    EItemRarity.COMMON => UIElements.ShopItemBackgroundCommon,
                    EItemRarity.UNCOMMON => UIElements.ShopItemBackgroundUncommon,
                    EItemRarity.RARE => UIElements.ShopItemBackgroundRare,
                    EItemRarity.EPIC => UIElements.ShopItemBackgroundEpic,
                    EItemRarity.LEGENDARY => UIElements.ShopItemBackgroundLegendary,
                    EItemRarity.MYTHICAL => UIElements.ShopItemBackgroundMythical,
                    _ => null
                };

                if (background != null)
                {
                    SendVisibility(background[index], true);
                }
            }
        }

        protected override async Task<IShopCategory<IVehicleShopData>?> GetDefaultCategory()
        {
            if (!Configuration.Instance.Shops.DefaultCategory.Vehicles.Enabled)
            {
                return null;
            }

            var shops = await ShopDirectory.GetShops();

            var name = Configuration.Instance.Shops.DefaultCategory.Vehicles.Name;

            return new BasicShopCategory<IVehicleShopData>(name, shops);
        }

        protected override async UniTask OnShopBuy(IVehicleShop shop, IVehicleAsset asset)
        {
            await shop.Buy(User);

            var amount = 1;

            await DisplayNotification(StringLocalizer["ui:vehicles:bought", new { VehicleAsset = asset, Amount = amount }]);
        }
    }
}
