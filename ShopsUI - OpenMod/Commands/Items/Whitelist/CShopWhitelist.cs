using Cysharp.Threading.Tasks;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using System;

namespace ShopsUI.Commands.Items.Whitelist
{
    [Command("whitelist", Priority = Priority.High)]
    [CommandAlias("wl")]
    [CommandSyntax("<add | rem> <item> <permission>")]
    [CommandDescription("Manage item shop whitelists.")]
    [CommandParent(typeof(CShop))]
    public class CShopWhitelist : UnturnedCommand
    {
        public CShopWhitelist(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override UniTask OnExecuteAsync()
        {
            throw new CommandWrongUsageException(Context);
        }
    }
}
