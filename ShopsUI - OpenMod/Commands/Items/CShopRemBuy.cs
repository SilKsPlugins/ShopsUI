using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Items;
using System;

namespace ShopsUI.Commands.Items
{
    [Command("buy")]
    [CommandAlias("b")]
    [CommandSyntax("<item>")]
    [CommandDescription("Removes the buyable item from the shop.")]
    [CommandParent(typeof(CShopRem))]
    public class CShopRemBuy : ShopCommand
    {
        private readonly IItemShopEditor _shopEditor;

        public CShopRemBuy(IServiceProvider serviceProvider,
            IItemShopEditor shopEditor) : base(serviceProvider)
        {
            _shopEditor = shopEditor;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetItemAsset(0);

            if (await _shopEditor.RemoveItemShopBuyable(ushort.Parse(asset.ItemAssetId)))
            {
                await PrintAsync(
                    StringLocalizer["commands:success:shop_removed:buyable_item", new {ItemAsset = asset}]);
            }
            else
            {
                throw new UserFriendlyException(
                    StringLocalizer["commands:errors:no_buyable_item_shop", new {ItemAsset = asset}]);
            }
        }
    }
}