using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using System;

namespace ShopsUI.Commands.Vehicles
{
    [Command("rem", Priority = Priority.High)]
    [CommandAlias("r")]
    [CommandAlias("remove")]
    [CommandAlias("-")]
    [CommandSyntax("<vehicle>")]
    [CommandDescription("Removes the buyable vehicle from the shop.")]
    [CommandParent(typeof(CVShop))]
    public class CVShopRem : ShopCommand
    {
        public CVShopRem(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetVehicleAsset(0);

            if (await ShopManager.RemoveVehicleShopBuyable(ushort.Parse(asset.VehicleAssetId)))
            {
                await PrintAsync(
                    StringLocalizer["commands:success:shop_removed:buyable_vehicle", new {VehicleAsset = asset}]);
            }
            else
            {
                throw new UserFriendlyException(
                    StringLocalizer["commands:errors:no_buyable_vehicle_shop", new {VehicleAsset = asset}]);
            }
        }
    }
}