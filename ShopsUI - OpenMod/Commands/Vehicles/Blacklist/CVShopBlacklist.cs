using Cysharp.Threading.Tasks;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using System;

namespace ShopsUI.Commands.Vehicles.Blacklist
{
    [Command("blacklist", Priority = Priority.High)]
    [CommandAlias("bl")]
    [CommandSyntax("<add | rem> <vehicle> <permission>")]
    [CommandParent(typeof(CVShop))]
    public class CVShopBlacklist : UnturnedCommand
    {
        public CVShopBlacklist(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override UniTask OnExecuteAsync()
        {
            throw new CommandWrongUsageException(Context);
        }
    }
}
