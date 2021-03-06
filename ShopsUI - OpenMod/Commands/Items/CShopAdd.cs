using Cysharp.Threading.Tasks;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using System;

namespace ShopsUI.Commands.Items
{
    [Command("add", Priority = Priority.High)]
    [CommandAlias("a")]
    [CommandAlias("+")]
    [CommandSyntax("<buy | sell> <item> <price>")]
    [CommandParent(typeof(CShop))]
    public class CShopAdd : UnturnedCommand
    {
        public CShopAdd(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override UniTask OnExecuteAsync()
        {
            throw new CommandWrongUsageException(Context);
        }
    }
}
