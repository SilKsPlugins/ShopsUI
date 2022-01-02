using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Vehicles.Whitelist;
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
        private readonly IVehicleShopWhitelistEditor _whitelistEditor;

        public CVShopWhitelistRemove(IServiceProvider serviceProvider,
            IVehicleShopWhitelistEditor whitelistEditor) : base(serviceProvider)
        {
            _whitelistEditor = whitelistEditor;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetVehicleAsset(0);
            var permission = await Context.Parameters.GetAsync<string>(1);

            if (await _whitelistEditor.RemoveWhitelist(ushort.Parse(asset.VehicleAssetId), permission))
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
