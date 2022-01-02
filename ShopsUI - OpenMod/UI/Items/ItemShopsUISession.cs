using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Unturned.Items;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using ShopsUI.API.Shops;
using ShopsUI.API.Shops.Items;
using ShopsUI.Shops;
using System;
using System.Threading.Tasks;
using UnturnedImages.API.Items;

namespace ShopsUI.UI.Items
{
    public class ItemShopsUISession : ShopsUISessionBase<IItemShopData, IItemShop, IItemShopDirectory, IItemAsset>
    {
        private readonly IItemDirectory _itemDirectory;
        private readonly IItemImageDirectoryAsync _itemImageDirectory;

        public ItemShopsUISession(UnturnedUser user, IServiceProvider serviceProvider,
            IItemDirectory itemDirectory,
            IItemImageDirectoryAsync itemImageDirectory) : base(user, serviceProvider)
        {
            _itemDirectory = itemDirectory;
            _itemImageDirectory = itemImageDirectory;
        }


        protected override async Task<IItemAsset?> GetAsset(ushort assetId)
        {
            return await _itemDirectory.FindByIdAsync(assetId.ToString());
        }

        protected override string GetAssetName(IItemAsset asset)
        {
            return asset.ItemName;
        }

        protected override async Task<string?> GetImageUrl(ushort assetId)
        {
            return await _itemImageDirectory.GetItemImageUrlAsync(assetId);
        }

        protected override void DisplayShopItem(ShopInfo shopInfo, int index)
        {
            SendVisibility(UIElements.ShopItems[index], true);

            Logger.LogDebug($"Shop item image URL: {shopInfo.ImageUrl}");

            if (!string.IsNullOrEmpty(shopInfo.ImageUrl))
            {
                SendVisibility(UIElements.ShopItemImages[index], true);
                SendImage(UIElements.ShopItemImages[index], shopInfo.ImageUrl!);
            }

            SendText(UIElements.ShopItemNames[index],
                StringLocalizer["ui:items:listing:content",
                    new
                    {
                        ItemAsset = shopInfo.Asset,
                        EconomyProvider.CurrencyName,
                        EconomyProvider.CurrencySymbol
                    }]);

            if (shopInfo.ShopData.BuyPrice.HasValue)
            {
                SendVisibility(UIElements.ShopItemBuyButtons[index], true);
                SendText(UIElements.ShopItemBuyPriceTexts[index],
                    StringLocalizer["ui:items:listing:buy",
                        new
                        {
                            ItemAsset = shopInfo.Asset,
                            EconomyProvider.CurrencyName,
                            EconomyProvider.CurrencySymbol,
                            BuyPrice = shopInfo.ShopData.BuyPrice.Value
                        }]);
            }

            if (shopInfo.ShopData.SellPrice.HasValue)
            {
                SendVisibility(UIElements.ShopItemSellButtons[index], true);
                SendText(UIElements.ShopItemSellPriceTexts[index],
                    StringLocalizer["ui:items:listing:sell",
                        new
                        {
                            ItemAsset = shopInfo.Asset,
                            EconomyProvider.CurrencyName,
                            EconomyProvider.CurrencySymbol,
                            SellPrice = shopInfo.ShopData.SellPrice.Value
                        }]);
            }

            if (Configuration.Instance.UI.BackgroundsEnabled && shopInfo.Asset is UnturnedItemAsset itemAsset)
            {
                var background = itemAsset.ItemAsset.rarity switch
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

        protected override async Task<IShopCategory<IItemShopData>?> GetDefaultCategory()
        {
            if (!Configuration.Instance.Shops.DefaultCategory.Items.Enabled)
            {
                return null;
            }

            var shops = await ShopDirectory.GetShops();

            var name = Configuration.Instance.Shops.DefaultCategory.Items.Name;

            return new BasicShopCategory<IItemShopData>(name, shops);
        }

        protected override bool ShouldShowShop(IItemShopData shopData)
        {
            return Configuration.Instance.UI.ShowSellOnlyShops || shopData.SellPrice.HasValue;
        }

        protected override async UniTask OnShopBuy(IItemShop shop, IItemAsset asset)
        {
            await shop.Buy(User);

            var amount = 1;

            await DisplayNotification(StringLocalizer["ui:items:bought", new {ItemAsset = asset, Amount = amount}]);
        }

        protected override async UniTask OnShopSell(IItemShop shop, IItemAsset asset)
        {
            await shop.Sell(User);

            var amount = 1;

            await DisplayNotification(StringLocalizer["ui:items:sold", new {ItemAsset = asset, Amount = amount}]);
        }
    }
}
