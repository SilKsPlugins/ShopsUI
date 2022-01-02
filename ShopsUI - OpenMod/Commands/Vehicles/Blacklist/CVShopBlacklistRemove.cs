using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Vehicles.Whitelist;
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
        private readonly IVehicleShopWhitelistEditor _whitelistEditor;

        public CVShopBlacklistRemove(IServiceProvider serviceProvider,
            IVehicleShopWhitelistEditor whitelistEditor) : base(serviceProvider)
        {
            _whitelistEditor = whitelistEditor;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetVehicleAsset(0);
            var permission = await Context.Parameters.GetAsync<string>(1);

            if (await _whitelistEditor.RemoveBlacklist(ushort.Parse(asset.VehicleAssetId), permission))
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
