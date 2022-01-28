using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.Database;
using System;

namespace ShopsUI.Commands.Vehicles
{
    [Command("order", Priority = Priority.High)]
    [CommandSyntax("<item> <order>")]
    [CommandDescription("Sets the order of vehicles in the shop ui.")]
    [CommandParent(typeof(CVShop))]
    public class CVShopOrder : ShopCommand
    {
        private readonly ShopsDbContext _dbContext;

        public CVShopOrder(ShopsDbContext dbContext,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _dbContext = dbContext;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetVehicleAsset(0);
            var order = await Context.Parameters.GetAsync<int>(1);

            var shop = await _dbContext.VehicleShops.FindAsync(ushort.Parse(asset.VehicleAssetId)) ??
                       throw new UserFriendlyException(
                           StringLocalizer["commands:errors:no_vehicle_shop", new { VehicleAsset = asset }]);

            shop.Order = order;

            _dbContext.VehicleShops.Update(shop);

            await _dbContext.SaveChangesAsync();

            await PrintAsync(
                StringLocalizer["commands:success:shop_order:vehicle", new { VehicleAsset = asset, Order = order }]);
        }
    }
}