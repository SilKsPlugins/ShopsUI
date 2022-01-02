using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Items.Whitelist;
using System;

namespace ShopsUI.Commands.Items.Whitelist
{
    [Command("remove", Priority = Priority.High)]
    [CommandAlias("rem")]
    [CommandAlias("r")]
    [CommandAlias("-")]
    [CommandSyntax("<item> <permission>")]
    [CommandDescription("Remove an item shop whitelist.")]
    [CommandParent(typeof(CShopWhitelist))]
    public class CShopWhitelistRemove : ShopCommand
    {
        private readonly IItemShopWhitelistEditor _whitelistEditor;

        public CShopWhitelistRemove(IServiceProvider serviceProvider,
            IItemShopWhitelistEditor whitelistEditor) : base(serviceProvider)
        {
            _whitelistEditor = whitelistEditor;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetItemAsset(0);
            var permission = await Context.Parameters.GetAsync<string>(1);

            if (await _whitelistEditor.RemoveWhitelist(ushort.Parse(asset.ItemAssetId), permission))
            {
                await PrintAsync(StringLocalizer["commands:success:shop_whitelist:removed:item",
                    new {ItemAsset = asset, Permission = permission}]);
            }
            else
            {
                throw new UserFriendlyException(StringLocalizer["commands:errors:shop_whitelist:removed:item",
                    new {ItemAsset = asset, Permission = permission}]);
            }
        }
    }
}
