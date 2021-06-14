using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using System;

namespace ShopsUI.Commands.Vehicles
{
    [Command("vcost", Priority = Priority.High)]
    [CommandAlias("vehiclecost")]
    [CommandAlias("costv")]
    [CommandAlias("costvehicle")]
    [CommandSyntax("<vehicle>")]
    [CommandDescription("Checks the price of a vehicle in the shop.")]
    public class CVCost : ShopCommand
    {
        public CVCost(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetVehicleAsset(0);

            var shop = await ShopManager.GetVehicleShop(ushort.Parse(asset.VehicleAssetId));

            if (shop == null)
                throw new UserFriendlyException(
                    StringLocalizer["commands:errors:no_vehicle_shop", new { ItemAsset = asset }]);

            if (!await shop.IsPermitted(Context.Actor))
            {
                throw new UserFriendlyException(StringLocalizer["commands:errors:not_permitted_vehicle",
                    new { VehicleAsset = asset }]);
            }

            await PrintAsync(StringLocalizer["commands:success:vehicle_cost:buy",
                new
                {
                    VehicleAsset = asset,
                    shop.ShopData.BuyPrice,
                    EconomyProvider.CurrencySymbol,
                    EconomyProvider.CurrencyName
                }]);
        }
    }
}
