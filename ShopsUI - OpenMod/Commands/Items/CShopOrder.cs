using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.Database;
using System;

namespace ShopsUI.Commands.Items
{
    [Command("order", Priority = Priority.High)]
    [CommandSyntax("<item> <order>")]
    [CommandDescription("Sets the order of items in the shop ui.")]
    [CommandParent(typeof(CShop))]
    public class CShopOrder : ShopCommand
    {
        private readonly ShopsDbContext _dbContext;

        public CShopOrder(ShopsDbContext dbContext,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _dbContext = dbContext;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetItemAsset(0);
            var order = await Context.Parameters.GetAsync<int>(1);

            var shop = await _dbContext.ItemShops.FindAsync(int.Parse(asset.ItemAssetId)) ??
                       throw new UserFriendlyException(
                           StringLocalizer["commands:errors:no_item_shop", new {ItemAsset = asset}]);

            shop.Order = order;

            _dbContext.ItemShops.Update(shop);

            await _dbContext.SaveChangesAsync();

            await PrintAsync(
                StringLocalizer["commands:success:shop_order:item", new {ItemAsset = asset, Order = order}]);
        }
    }
}