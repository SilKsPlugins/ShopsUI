using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using System;

namespace ShopsUI.Commands.Vehicles.Whitelist
{
    [Command("remove", Priority = Priority.High)]
    [CommandAlias("rem")]
    [CommandAlias("r")]
    [CommandAlias("-")]
    [CommandSyntax("<vehicle> <permission>")]
    [CommandDescription("Remove a vehicle shop whitelist.")]
    [CommandParent(typeof(CVShopWhitelist))]
    public class CVShopWhitelistRemove : ShopCommand
    {
        public CVShopWhitelistRemove(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetVehicleAsset(0);
            var permission = await Context.Parameters.GetAsync<string>(1);

            if (await ShopManager.RemoveVehicleWhitelist(ushort.Parse(asset.VehicleAssetId), permission))
            {
                await PrintAsync(StringLocalizer["commands:success:shop_whitelist:removed:vehicle",
                    new {VehicleAsset = asset, Permission = permission}]);
            }
            else
            {
                throw new UserFriendlyException(StringLocalizer["commands:errors:shop_whitelist:removed:vehicle",
                    new {VehicleAsset = asset, Permission = permission}]);
            }
        }
    }
}
