using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using System;

namespace ShopsUI.Commands.Items
{
    [Command("remove")]
    [CommandAlias("rem")]
    [CommandAlias("r")]
    [CommandAlias("-")]
    [CommandSyntax("<buy | sell> <item>")]
    [CommandDescription("Removes a shop")]
    [CommandParent(typeof(CShop))]
    public class CShopRem : UnturnedCommand
    {
        public CShopRem(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override UniTask OnExecuteAsync()
        {
            throw new CommandWrongUsageException(Context);
        }
    }
}