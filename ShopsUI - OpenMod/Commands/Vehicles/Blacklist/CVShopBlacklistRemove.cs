using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using System;

namespace ShopsUI.Commands.Vehicles.Blacklist
{
    [Command("remove", Priority = Priority.High)]
    [CommandAlias("rem")]
    [CommandAlias("r")]
    [CommandAlias("-")]
    [CommandSyntax("<vehicle> <permission>")]
    [CommandDescription("Remove a vehicle shop blacklist.")]
    [CommandParent(typeof(CVShopBlacklist))]
    public class CVShopBlacklistRemove : ShopCommand
    {
        public CVShopBlacklistRemove(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetVehicleAsset(0);
            var permission = await Context.Parameters.GetAsync<string>(1);

            if (await ShopManager.RemoveVehicleBlacklist(ushort.Parse(asset.VehicleAssetId), permission))
            {
                await PrintAsync(StringLocalizer["commands:success:shop_blacklist:removed:vehicle",
                    new {VehicleAsset = asset, Permission = permission}]);
            }
            else
            {
                throw new UserFriendlyException(StringLocalizer["commands:errors:shop_blacklist:removed:vehicle",
                    new {VehicleAsset = asset, Permission = permission}]);
            }
        }
    }
}
