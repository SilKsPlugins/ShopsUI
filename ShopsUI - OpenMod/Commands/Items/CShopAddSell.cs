using Cysharp.Threading.Tasks;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Items;
using System;

namespace ShopsUI.Commands.Items
{
    [Command("sell", Priority = Priority.High)]
    [CommandAlias("s")]
    [CommandSyntax("<item> <price>")]
    [CommandDescription("Adds the item to the shop to be sold.")]
    [CommandParent(typeof(CShopAdd))]
    public class CShopAddSell : ShopCommand
    {
        private readonly IItemShopEditor _shopEditor;

        public CShopAddSell(IServiceProvider serviceProvider,
            IItemShopEditor shopEditor) : base(serviceProvider)
        {
            _shopEditor = shopEditor;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetItemAsset(0);
            var price = await GetPrice(1);

            await _shopEditor.AddItemShopSellable(ushort.Parse(asset.ItemAssetId), price);

            await PrintAsync(
                StringLocalizer["commands:success:shop_added:sellable_item",
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