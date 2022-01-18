using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using ShopsUI.API.Shops.Items;
using ShopsUI.API.Shops.Vehicles;
using ShopsUI.Commands.Items;

namespace ShopsUI.Commands
{
    [Command("reload", Priority = Priority.High)]
    [CommandDescription("Reloads the shops from the database.")]
    [CommandParent(typeof(CShop))]
    public class CShopReload : UnturnedCommand
    {
        private readonly IItemShopDirectory _itemShopDirectory;
        private readonly IVehicleShopDirectory _vehicleShopDirectory;
        private readonly IStringLocalizer _stringLocalizer;

        public CShopReload(IServiceProvider serviceProvider,
            IItemShopDirectory itemShopDirectory,
            IVehicleShopDirectory vehicleShopDirectory,
            IStringLocalizer stringLocalizer) : base(serviceProvider)
        {
            _itemShopDirectory = itemShopDirectory;
            _vehicleShopDirectory = vehicleShopDirectory;
            _stringLocalizer = stringLocalizer;
        }

        protected override async UniTask OnExecuteAsync()
        {
            await _itemShopDirectory.Invalidate(true);
            await _vehicleShopDirectory.Invalidate(true);

            await PrintAsync(_stringLocalizer["commands:success:shop_reloaded"]);
        }
    }
}
