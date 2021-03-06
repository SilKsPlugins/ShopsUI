using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using System;

namespace ShopsUI.Commands.Vehicles
{
    [Command("vbuy", Priority = Priority.High)]
    [CommandAlias("vehiclebuy")]
    [CommandAlias("buyv")]
    [CommandAlias("buyvehicle")]
    [CommandSyntax("<vehicle>")]
    [CommandDescription("Buys the item from the shop.")]
    public class CVBuy : ShopCommand
    {
        public CVBuy(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetVehicleAsset(0);

            var shop = await ShopManager.GetVehicleShop(ushort.Parse(asset.VehicleAssetId))
                       ?? throw new UserFriendlyException(
                           StringLocalizer["commands:errors:no_buyable_vehicle_shop", new { VehicleAsset = asset }]);

            var balance = await shop.Buy(GetUnturnedUser());

            await PrintAsync(StringLocalizer["commands:success:vehicle_bought",
                new
                {
                    VehicleAsset = asset,
                    Price = shop.ShopData.BuyPrice,
                    Balance = balance,
                    EconomyProvider.CurrencySymbol,
                    EconomyProvider.CurrencyName
                }]);
        }
    }
}
