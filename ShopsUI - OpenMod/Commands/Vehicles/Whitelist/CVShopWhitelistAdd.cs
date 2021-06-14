using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using System;

namespace ShopsUI.Commands.Vehicles.Whitelist
{
    [Command("add", Priority = Priority.High)]
    [CommandAlias("a")]
    [CommandAlias("+")]
    [CommandSyntax("<vehicle> <permission>")]
    [CommandParent(typeof(CVShopWhitelist))]
    public class CVShopWhitelistAdd : ShopCommand
    {
        public CVShopWhitelistAdd(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetVehicleAsset(0);
            var permission = await Context.Parameters.GetAsync<string>(1);

            if (await ShopManager.AddVehicleWhitelist(ushort.Parse(asset.VehicleAssetId), permission))
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
