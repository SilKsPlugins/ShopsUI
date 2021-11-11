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
using ShopsUI.API;
using ShopsUI.API.Items;
using ShopsUI.API.SellBox;
using ShopsUI.SellBox.Inventory;
using SilK.Unturned.Extras.Events;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly Dictionary<UnturnedUser, SellBoxInstance> _sellBoxes = new();

        public SellBoxManager(IServiceProvider serviceProvider,
            ILogger<SellBoxManager> logger,
            IStringLocalizer stringLocalizer,
            IEconomyProvider economyProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _stringLocalizer = stringLocalizer;
            _economyProvider = economyProvider;

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
                if (sellBox.Storage == null)
                {
                    return;
                }

                var items = sellBox.Storage.items.items.Select(x => x.item).ToList();

                if (sellBox.Storage.items.items.Count == 0)
                {
                    return;
                }

                await UniTask.SwitchToThreadPool();

                var shopManager = _serviceProvider.GetRequiredService<IShopManager>();

                var itemShops = new List<(IItemShop shop, int amount)>();

                foreach (var grouping in items.GroupBy(x => x.id))
                {
                    var shop = await shopManager.GetItemShop(grouping.Key);

                    if (shop != null && shop.CanSell())
                    {
                        itemShops.Add((shop, grouping.Count()));
                    }
                }

                var inventory = new SellBoxInventory(sellBox.Storage);

                decimal totalPrice = 0;
                var totalAmount = 0;

                foreach (var pair in itemShops.ToList())
                {
                    try
                    {
                        var shop = pair.shop;
                        var amount = pair.amount;

                        await shop.Sell(user, inventory, amount);

                        totalPrice += shop.ShopData.SellPrice!.Value;
                        totalAmount += amount;

                        itemShops.Remove(pair);
                    }
                    catch (UserFriendlyException ex)
                    {
                        _logger.LogDebug(ex, "User friendly exception occurred during selling");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred when selling item");
                    }
                }

                var totalReturned = 0;

                await UniTask.SwitchToMainThread();

                foreach (var itemJar in inventory.Storage.items.items)
                {
                    var item = itemJar.item;

                    user.Player.Player.inventory.forceAddItem(item, false);

                    totalReturned++;
                }

                await user.PrintMessageAsync(_stringLocalizer["commands:success:sellbox_sold",
                    new
                    {
                        TotalAmount = totalAmount,
                        TotalPrice = totalPrice,
                        ReturnedAmount = totalReturned,
                        _economyProvider.CurrencySymbol,
                        _economyProvider.CurrencyName
                    }]);

            }).Forget();
        }
    }
}
