using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using ShopsUI.API.UI;
using System;

namespace ShopsUI.Commands.Vehicles
{
    [Command("vshop", Priority = Priority.High)]
    [CommandAlias("vshops")]
    [CommandAlias("vehicleshop")]
    [CommandAlias("vehicleshops")]
    [CommandDescription("Opens the vehicle shop UI.")]
    public class CVShop : UnturnedCommand
    {
        private readonly IUIManager _uiManager;

        public CVShop(IUIManager uiManager,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _uiManager = uiManager;
        }

        protected override async UniTask OnExecuteAsync()
        {
            await UniTask.SwitchToMainThread();

            if (Context.Actor is not UnturnedUser user)
                throw new UserFriendlyException("This command can only be called by a player");

            await _uiManager.StartSession(user, UITab.Vehicles);
        }
    }
}
