using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Vehicles;
using System;

namespace ShopsUI.Commands.Vehicles
{
    [Command("remove", Priority = Priority.High)]
    [CommandAlias("rem")]
    [CommandAlias("r")]
    [CommandAlias("-")]
    [CommandSyntax("<vehicle>")]
    [CommandDescription("Removes the buyable vehicle from the shop.")]
    [CommandParent(typeof(CVShop))]
    public class CVShopRem : ShopCommand
    {
        private readonly IVehicleShopEditor _shopEditor;

        public CVShopRem(IServiceProvider serviceProvider,
            IVehicleShopEditor shopEditor) : base(serviceProvider)
        {
            _shopEditor = shopEditor;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetVehicleAsset(0);

            if (await _shopEditor.RemoveVehicleShopBuyable(ushort.Parse(asset.VehicleAssetId)))
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