using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Vehicles.Whitelist;
using System;

namespace ShopsUI.Commands.Vehicles.Whitelist
{
    [Command("add", Priority = Priority.High)]
    [CommandAlias("a")]
    [CommandAlias("+")]
    [CommandSyntax("<vehicle> <permission>")]
    [CommandDescription("Add a vehicle shop whitelist.")]
    [CommandParent(typeof(CVShopWhitelist))]
    public class CVShopWhitelistAdd : ShopCommand
    {
        private readonly IVehicleShopWhitelistEditor _whitelistEditor;

        public CVShopWhitelistAdd(IServiceProvider serviceProvider,
            IVehicleShopWhitelistEditor whitelistEditor) : base(serviceProvider)
        {
            _whitelistEditor = whitelistEditor;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetVehicleAsset(0);
            var permission = await Context.Parameters.GetAsync<string>(1);

            if (await _whitelistEditor.AddWhitelist(ushort.Parse(asset.VehicleAssetId), permission))
            {
                await PrintAsync(StringLocalizer["commands:success:shop_whitelist:added:vehicle",
                    new {VehicleAsset = asset, Permission = permission}]);
            }
            else
            {
                throw new UserFriendlyException(StringLocalizer["commands:errors:shop_whitelist:added:vehicle",
                    new {VehicleAsset = asset, Permission = permission}]);
            }
        }
    }
}
