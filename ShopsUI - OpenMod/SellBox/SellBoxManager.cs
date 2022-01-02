using Autofac;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Unturned.Users;
using OpenMod.Unturned.Users.Events;
using ShopsUI.API.SellBox;
using ShopsUI.SellBox.UI;
using SilK.Unturned.Extras.Events;
using SilK.Unturned.Extras.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopsUI.SellBox
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class SellBoxManager : ISellBoxManager, IAsyncDisposable,
        IInstanceEventListener<UnturnedUserDisconnectedEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SellBoxManager> _logger;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly IEconomyProvider _economyProvider;
        private readonly IUIManager _uiManager;
        private readonly ILifetimeScope _lifetimeScope;

        private readonly Dictionary<UnturnedUser, SellBoxInstance> _sellBoxes = new();

        public SellBoxManager(IServiceProvider serviceProvider,
            ILogger<SellBoxManager> logger,
            IStringLocalizer stringLocalizer,
            IEconomyProvider economyProvider,
            IUIManager uiManager,
            ILifetimeScope lifetimeScope)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _stringLocalizer = stringLocalizer;
            _economyProvider = economyProvider;
            _uiManager = uiManager;
            _lifetimeScope = lifetimeScope;

            SellBoxInstance.OnSellBoxClosed += OnSellBoxClosed;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                SellBoxInstance.OnSellBoxClosed -= OnSellBoxClosed;

                await UniTask.SwitchToMainThread();

                await _sellBoxes.Values.DisposeAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred during disposal of {nameof(SellBoxManager)}");
            }
        }

        public async UniTask OpenSellBox(UnturnedUser user)
        {
            await UniTask.SwitchToMainThread();

            if (_sellBoxes.TryGetValue(user, out var sellBox))
            {
                await sellBox.DisposeAsync();
            }

            sellBox = ActivatorUtilities.CreateInstance<SellBoxInstance>(_serviceProvider, user);

            _sellBoxes[user] = sellBox;

            if (!await sellBox.OpenAsync())
            {
                _sellBoxes.Remove(user);

                await sellBox.DisposeAsync();

                throw new UserFriendlyException(_stringLocalizer["commands:errors:no_sellbox"]);
            }
        }

        public UniTask HandleEventAsync(object? sender, UnturnedUserDisconnectedEvent @event)
        {
            _sellBoxes.Remove(@event.User);

            return UniTask.CompletedTask;
        }
        
        private void OnSellBoxClosed(UnturnedUser user, SellBoxInstance sellBox)
        {
            UniTask.RunOnThreadPool(async () =>
            {
                if (sellBox.Storage == null || sellBox.Storage.items.items.Count == 0)
                {
                    return;
                }

                var lifetimeScope = _lifetimeScope.BeginLifetimeScope(builder =>
                {
                    builder.RegisterInstance(sellBox)
                        .SingleInstance()
                        .ExternallyOwned();
                });

                var session = await _uiManager.StartSession<SellBoxUISession>(user,
                    new UISessionOptions { EndOnDeath = true }, lifetimeScope);

                session.OnUISessionEnded += _ => AsyncHelper.RunSync(async () => await lifetimeScope.DisposeAsync());
            }).Forget();
        }
    }
}
