using Cysharp.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
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
        private readonly IOpenModComponent _openModComponent;

        public CShop(IUIManager uiManager,
            IOpenModComponent openModComponent,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _uiManager = uiManager;
            _openModComponent = openModComponent;
        }

        protected override async UniTask OnExecuteAsync()
        {
            await UniTask.SwitchToMainThread();

            if (Context.Actor is not UnturnedUser user)
                throw new UserFriendlyException("This command can only be called by a player");

            var session =
                await _uiManager.StartSession<ShopsUISession>(user, lifetimeScope: _openModComponent.LifetimeScope);

            await session.SetTab(UITab.Items);
        }
    }
}
