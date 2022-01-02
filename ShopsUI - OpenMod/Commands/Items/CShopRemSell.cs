using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Items;
using System;

namespace ShopsUI.Commands.Items
{
    [Command("sell")]
    [CommandAlias("s")]
    [CommandSyntax("<item>")]
    [CommandDescription("Removes the sellable item from the shop.")]
    [CommandParent(typeof(CShopRem))]
    public class CShopRemSell : ShopCommand
    {
        private readonly IItemShopEditor _shopEditor;

        public CShopRemSell(IServiceProvider serviceProvider,
            IItemShopEditor shopEditor) : base(serviceProvider)
        {
            _shopEditor = shopEditor;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetItemAsset(0);

            if (await _shopEditor.RemoveItemShopSellable(ushort.Parse(asset.ItemAssetId)))
            {
                await PrintAsync(StringLocalizer["commands:success:shop_removed:sellable_item", new { ItemAsset = asset }]);
            }
            else
            {
                throw new UserFriendlyException(
                    StringLocalizer["commands:errors:no_sellable_item_shop", new {ItemAsset = asset}]);
            }
        }
    }
}