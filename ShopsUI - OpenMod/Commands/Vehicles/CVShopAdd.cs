using Cysharp.Threading.Tasks;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Vehicles;
using System;

namespace ShopsUI.Commands.Vehicles
{
    [Command("add", Priority = Priority.High)]
    [CommandAlias("a")]
    [CommandAlias("+")]
    [CommandSyntax("<vehicle> <price>")]
    [CommandDescription("Adds the vehicle to the shop to be bought.")]
    [CommandParent(typeof(CVShop))]
    public class CVShopAdd : ShopCommand
    {
        private readonly IVehicleShopEditor _shopEditor;

        public CVShopAdd(IServiceProvider serviceProvider,
            IVehicleShopEditor vehicleShopEditor) : base(serviceProvider)
        {
            _shopEditor = vehicleShopEditor;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetVehicleAsset(0);
            var price = await GetPrice(1);

            await _shopEditor.AddVehicleShopBuyable(ushort.Parse(asset.VehicleAssetId), price);

            await PrintAsync(
                StringLocalizer["commands:success:shop_added:buyable_vehicle",
                    new
                    {
                        VehicleAsset = asset,
                        Price = price,
                        EconomyProvider.CurrencySymbol,
                        EconomyProvider.CurrencyName
                    }]);
        }
    }
}