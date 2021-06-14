using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using System;

namespace ShopsUI.Commands.Items.Whitelist
{
    [Command("add", Priority = Priority.High)]
    [CommandAlias("a")]
    [CommandAlias("+")]
    [CommandSyntax("<item> <permission>")]
    [CommandDescription("Add an item shop whitelist.")]
    [CommandParent(typeof(CShopWhitelist))]
    public class CShopWhitelistAdd : ShopCommand
    {
        public CShopWhitelistAdd(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetItemAsset(0);
            var permission = await Context.Parameters.GetAsync<string>(1);

            if (await ShopManager.AddItemWhitelist(ushort.Parse(asset.ItemAssetId), permission))
            {
                await PrintAsync(StringLocalizer["commands:success:shop_whitelist:added:item",
                    new {ItemAsset = asset, Permission = permission}]);
            }
            else
            {
                throw new UserFriendlyException(StringLocalizer["commands:errors:shop_whitelist:added:item",
                    new {ItemAsset = asset, Permission = permission}]);
            }
        }
    }
}
