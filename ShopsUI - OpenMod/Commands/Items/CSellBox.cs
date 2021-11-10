using Cysharp.Threading.Tasks;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using ShopsUI.API.SellBox;
using System;

namespace ShopsUI.Commands.Items
{
    [Command("sellbox", Priority = Priority.High)]
    [CommandAlias("sbox")]
    [CommandDescription("Open a virtual inventory where you can quickly sell items.")]
    [CommandActor(typeof(UnturnedUser))]
    public class CSellBox : UnturnedCommand
    {
        private readonly ISellBoxManager _sellBoxManager;

        public CSellBox(IServiceProvider serviceProvider,
            ISellBoxManager sellBoxManager) : base(serviceProvider)
        {
            _sellBoxManager = sellBoxManager;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var user = (UnturnedUser)Context.Actor;

            await _sellBoxManager.OpenSellBox(user);
        }
    }
}
