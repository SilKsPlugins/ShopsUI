using Autofac;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.API.Ioc;
using OpenMod.API.Plugins;
using OpenMod.API.Prioritization;
using OpenMod.Core.Ioc;
using OpenMod.Unturned.Players.Life.Events;
using OpenMod.Unturned.Users;
using OpenMod.Unturned.Users.Events;
using SDG.Unturned;
using ShopsUI.API.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI.UI
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class UIManager : IUIManager, IAsyncDisposable
    {
        private readonly IPluginAccessor<ShopsUIPlugin> _pluginAccessor;
        private readonly IUnturnedUserDirectory _unturnedUserDirectory;

        private readonly Dictionary<UnturnedUser, UISession> _sessions;
        private bool _isDisposing;

        private ILifetimeScope GetPluginScope() => _pluginAccessor.Instance?.LifetimeScope ??
                                                   throw new Exception("ShopsUI plugin is not loaded");

        public UIManager(
            IPluginAccessor<ShopsUIPlugin> pluginAccessor,
            IUnturnedUserDirectory unturnedUserDirectory,
            IRuntime runtime, IEventBus eventBus)
        {
            _pluginAccessor = pluginAccessor;
            _unturnedUserDirectory = unturnedUserDirectory;

            _sessions = new Dictionary<UnturnedUser, UISession>();

            eventBus.Subscribe(runtime, (EventCallback<UnturnedUserDisconnectedEvent>) OnUserDisconnected);
            eventBus.Subscribe(runtime, (EventCallback<UnturnedPlayerDeathEvent>) OnPlayerDeath);
            // todo: implement this event in next OM version
            //eventBus.Subscribe(runtime, (EventCallback<UnturnedPlayerButtonClickedEvent>)OnPlayerButtonClicked);
            EffectManager.onEffectButtonClicked += OnEffectButtonClicked;
        }

        public async ValueTask DisposeAsync()
        {
            if (_isDisposing) return;
            _isDisposing = true;

            EffectManager.onEffectButtonClicked -= OnEffectButtonClicked;

            for (var i = _sessions.Count - 1; i >= 0; i--)
            {
                var pair = _sessions.ElementAt(i);

                await pair.Value.EndSession();
            }
        }

        public async UniTask StartSession(UnturnedUser user, UITab tab = UITab.Items)
        {
            await UniTask.SwitchToMainThread();

            if (_sessions.ContainsKey(user))
                await EndSession(user);

            var session = ActivatorUtilitiesEx.CreateInstance<UISession>(GetPluginScope());
            
            session.OnSessionEnded += OnSessionEnded;

            _sessions.Add(user, session);

            await session.StartSession(user, tab);
        }

        private void OnSessionEnded(UnturnedUser user, UISession session)
        {
            if (_sessions.ContainsKey(user))
                _sessions.Remove(user);
        }

        public async UniTask EndSession(UnturnedUser user)
        {
            await UniTask.SwitchToMainThread();

            if (_sessions.TryGetValue(user, out var session))
            {
                await session.EndSession();
            }
        }

        private Task OnUserDisconnected(IServiceProvider serviceProvider, object? sender,
            UnturnedUserDisconnectedEvent @event) => EndSession(@event.User).AsTask();

        private Task OnPlayerDeath(IServiceProvider serviceProvider, object? sender, UnturnedPlayerDeathEvent @event)
        {
            var user = _unturnedUserDirectory.GetUser(@event.Player.Player);

            return EndSession(user).AsTask();
        }

        private void OnEffectButtonClicked(Player player, string buttonName)
        {
            var user = _unturnedUserDirectory.GetUser(player);

            if (_sessions.TryGetValue(user, out var session))
            {
                session.OnButtonClicked(buttonName).Forget();
            }
        }
    }
}
