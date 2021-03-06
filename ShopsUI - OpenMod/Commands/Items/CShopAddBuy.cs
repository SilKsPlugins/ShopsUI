using Cysharp.Threading.Tasks;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using System;

namespace ShopsUI.Commands.Items
{
    [Command("buy", Priority = Priority.High)]
    [CommandAlias("b")]
    [CommandSyntax("<item> <price>")]
    [CommandDescription("Adds the item to the shop to be bought.")]
    [CommandParent(typeof(CShopAdd))]
    public class CShopAddBuy : ShopCommand
    {
        public CShopAddBuy(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetItemAsset(0);
            var price = await GetPrice(1);

            await ShopManager.AddItemShopBuyable(ushort.Parse(asset.ItemAssetId), price);

            await PrintAsync(
                StringLocalizer["commands:success:shop_added:buyable_item",
                    new
                    {
                        ItemAsset = asset,
                        Price = price,
                        EconomyProvider.CurrencySymbol,
                        EconomyProvider.CurrencyName
                    }]);
        }
    }
}
