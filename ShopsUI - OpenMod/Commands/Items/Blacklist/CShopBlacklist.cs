using Cysharp.Threading.Tasks;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using System;

namespace ShopsUI.Commands.Items.Blacklist
{
    [Command("blacklist", Priority = Priority.High)]
    [CommandAlias("bl")]
    [CommandSyntax("<add | rem> <item> <permission>")]
    [CommandDescription("Manage item shop blacklists.")]
    [CommandParent(typeof(CShop))]
    public class CShopBlacklist : UnturnedCommand
    {
        public CShopBlacklist(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override UniTask OnExecuteAsync()
        {
            throw new CommandWrongUsageException(Context);
        }
    }
}
