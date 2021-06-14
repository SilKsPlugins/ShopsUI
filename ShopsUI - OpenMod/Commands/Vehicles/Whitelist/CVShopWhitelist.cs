using Cysharp.Threading.Tasks;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using System;

namespace ShopsUI.Commands.Vehicles.Whitelist
{
    [Command("whitelist", Priority = Priority.High)]
    [CommandAlias("wl")]
    [CommandSyntax("<add | rem> <vehicle> <permission>")]
    [CommandDescription("Manage vehicle shop whitelists.")]
    [CommandParent(typeof(CVShop))]
    public class CVShopWhitelist : UnturnedCommand
    {
        public CVShopWhitelist(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override UniTask OnExecuteAsync()
        {
            throw new CommandWrongUsageException(Context);
        }
    }
}
