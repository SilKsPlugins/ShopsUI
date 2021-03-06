using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using System;
using OpenMod.API.Commands;

namespace ShopsUI.Commands.Items
{
    [Command("buy")]
    [CommandAlias("b")]
    [CommandSyntax("<item>")]
    [CommandDescription("Removes the buyable item from the shop.")]
    [CommandParent(typeof(CShopRem))]
    public class CShopRemBuy : ShopCommand
    {
        public CShopRemBuy(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetItemAsset(0);

            if (await ShopManager.RemoveItemShopBuyable(ushort.Parse(asset.ItemAssetId)))
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