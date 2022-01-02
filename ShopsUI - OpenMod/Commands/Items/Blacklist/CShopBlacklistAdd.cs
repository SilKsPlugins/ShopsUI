using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Items.Whitelist;
using System;

namespace ShopsUI.Commands.Items.Blacklist
{
    [Command("add", Priority = Priority.High)]
    [CommandAlias("a")]
    [CommandAlias("+")]
    [CommandSyntax("<item> <permission>")]
    [CommandDescription("Add an item shop blacklist.")]
    [CommandParent(typeof(CShopBlacklist))]
    public class CShopBlacklistAdd : ShopCommand
    {
        private readonly IItemShopWhitelistEditor _whitelistEditor;

        public CShopBlacklistAdd(IServiceProvider serviceProvider,
            IItemShopWhitelistEditor whitelistEditor) : base(serviceProvider)
        {
            _whitelistEditor = whitelistEditor;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetItemAsset(0);
            var permission = await Context.Parameters.GetAsync<string>(1);

            if (await _whitelistEditor.AddBlacklist(ushort.Parse(asset.ItemAssetId), permission))
            {
                await PrintAsync(StringLocalizer["commands:success:shop_blacklist:added:item",
                    new {ItemAsset = asset, Permission = permission}]);
            }
            else
            {
                throw new UserFriendlyException(StringLocalizer["commands:errors:shop_blacklist:added:item",
                    new {ItemAsset = asset, Permission = permission}]);
            }
        }
    }
}
