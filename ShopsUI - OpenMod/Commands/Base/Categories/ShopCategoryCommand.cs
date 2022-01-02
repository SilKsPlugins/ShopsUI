using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using System;

namespace ShopsUI.Commands.Base.Categories
{
    public abstract class ShopCategoryCommand : UnturnedCommand
    {
        protected ShopCategoryCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override UniTask OnExecuteAsync()
        {
            return UniTask.FromException(new CommandWrongUsageException(Context));
        }
    }
}
