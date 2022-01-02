using Cysharp.Threading.Tasks;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Items;
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
        private readonly IItemShopEditor _shopEditor;

        public CShopAddBuy(IServiceProvider serviceProvider,
            IItemShopEditor shopEditor) : base(serviceProvider)
        {
            _shopEditor = shopEditor;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetItemAsset(0);
            var price = await GetPrice(1);

            await _shopEditor.AddItemShopBuyable(ushort.Parse(asset.ItemAssetId), price);

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
