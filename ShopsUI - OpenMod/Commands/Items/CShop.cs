using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using ShopsUI.API.UI;
using ShopsUI.UI;
using SilK.Unturned.Extras.UI;
using System;

namespace ShopsUI.Commands.Items
{
    [Command("shop", Priority = Priority.High)]
    [CommandAlias("shops")]
    [CommandAlias("ishop")]
    [CommandAlias("ishops")]
    [CommandAlias("itemshop")]
    [CommandAlias("itemshops")]
    [CommandDescription("Opens the shop UI.")]
    public class CShop : UnturnedCommand
    {
        private readonly IUIManager _uiManager;

        public CShop(IUIManager uiManager,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _uiManager = uiManager;
        }

        protected override async UniTask OnExecuteAsync()
        {
            await UniTask.SwitchToMainThread();

            if (Context.Actor is not UnturnedUser user)
                throw new UserFriendlyException("This command can only be called by a player");

            var session = await _uiManager.StartSession<ShopsUISession>(user);

            await session.SetTab(UITab.Items);
        }
    }
}
