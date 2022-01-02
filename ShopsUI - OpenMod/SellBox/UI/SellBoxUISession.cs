using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Commands;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using ShopsUI.API.Shops;
using ShopsUI.API.Shops.Items;
using ShopsUI.Configuration;
using ShopsUI.SellBox.Inventory;
using SilK.Unturned.Extras.Configuration;
using SilK.Unturned.Extras.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShopsUI.SellBox.UI
{
    public class SellBoxUISession : SingleEffectUISession
    {
        public override string Id => "SilK.ShopsUI.SellBox";

        public override ushort EffectId { get; }

        private readonly SellBoxInstance _sellBoxInstance;
        private readonly IConfigurationParser<ShopsUIConfig> _configuration;
        private readonly IItemShopDirectory _shopDirectory;
        private readonly ILogger<SellBoxUISession> _logger;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly IEconomyProvider _economyProvider;
        private readonly IItemDirectory _itemDirectory;

        private SellBoxInventory? _sellBoxInventory;
        private int _totalReturned;

        private readonly Dictionary<ushort, IItemShop> _itemShops = new();

        public SellBoxUISession(UnturnedUser user, IServiceProvider serviceProvider,
            SellBoxInstance sellBoxInstance,
            IConfigurationParser<ShopsUIConfig> configuration,
            IItemShopDirectory shopDirectory,
            ILogger<SellBoxUISession> logger,
            IStringLocalizer stringLocalizer,
            IEconomyProvider economyProvider,
            IItemDirectory itemDirectory) : base(user, serviceProvider)
        {
            _sellBoxInstance = sellBoxInstance;
            _configuration = configuration;
            _shopDirectory = shopDirectory;
            _logger = logger;
            _stringLocalizer = stringLocalizer;
            _economyProvider = economyProvider;
            _itemDirectory = itemDirectory;

            EffectId = _configuration.Instance.UI.SellBoxEffect;
        }

        protected override async UniTask OnStartAsync()
        {
            _sellBoxInventory = GetSellBoxInventory();

            var (output, totalSellPrice) = await PrepareSellItems();

            SubscribeButtonClick("SellItemsButton", OnSellItemsClick);
            SubscribeButtonClick("CloseButton", async _ => await EndAsync());
            SubscribeButtonClick("CancelButton", async _ => await EndAsync());

            await UniTask.SwitchToMainThread();

            SendUIEffect();

            SendText("HeaderText", _stringLocalizer["ui:sellbox:header"]);

            if (!string.IsNullOrEmpty(_configuration.Instance.UI.LogoUrl))
            {
                SendImage("HeaderImage", _configuration.Instance.UI.LogoUrl!);
            }

            SendText("SellBoxText", output);

            SendText("SellItemsText",
                _stringLocalizer["ui:sellbox:confirm",
                    new { _economyProvider.CurrencyName, _economyProvider.CurrencySymbol, TotalSellPrice = totalSellPrice }]);

            SendText("CancelText", _stringLocalizer["ui:sellbox:cancel"]);

            await SetCursor(true);
        }

        protected override async UniTask OnEndAsync()
        {
            await UniTask.SwitchToMainThread();

            ClearEffect();

            await SetCursor(false);

            try
            {
                if (_sellBoxInventory == null)
                {
                    throw new Exception("Sell box inventory is null");
                }

                await ReturnItems(_sellBoxInventory.Items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Error occurred when attempting to return left over items.");
            }
        }

        private SellBoxInventory GetSellBoxInventory()
        {
            if (_sellBoxInstance.Storage == null)
            {
                throw new Exception("Sell box's storage instance is null");
            }

            var items = _sellBoxInstance.Storage.items.items.Select(x => x.item).ToList();

            if (items.Count == 0)
            {
                throw new Exception("No items in sell box");
            }

            return new SellBoxInventory(items);
        }

        private async UniTask ReturnItems(ICollection<Item> items, StringBuilder? output = null)
        {
            if (_sellBoxInventory == null)
            {
                throw new Exception("Sell box inventory is null");
            }

            await UniTask.SwitchToMainThread();
            
            if (items.Count == 0)
            {
                output?.AppendLine(_stringLocalizer["ui:sellbox:returned_items:none"]);
                return;
            }

            output?.AppendLine(_stringLocalizer["ui:sellbox:returned_items:header"]);

            foreach (var itemsGroup in items.GroupBy(x => x.id))
            {
                try
                {
                    foreach (var item in itemsGroup)
                    {
                        if (_sellBoxInventory.Items.Remove(item))
                        {
                            User.Player.Player.inventory.forceAddItem(item, false);
                            _totalReturned++;
                        }
                        else
                        {
                            _logger.LogWarning(
                                $"Item was attempted to be returned despite not being in sell box inventory (Player: {User.SteamId}, Item ID: {item.id}).");
                        }
                    }

                    if (output != null)
                    {
                        var itemAsset = await _itemDirectory.FindByIdAsync(itemsGroup.Key.ToString());

                        output.AppendLine(_stringLocalizer["ui:sellbox:returned_items:line",
                            new { Amount = itemsGroup.Count(), ItemAsset = itemAsset }]);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error occurred when returning items of ID {itemsGroup.Key}");

                    output?.AppendLine($"- Error returning item with ID {itemsGroup.Key}");
                }
            }
        }

        private async UniTask<(string Output, decimal TotalSellPrice)> PrepareSellItems()
        {
            if (_sellBoxInventory == null)
            {
                throw new Exception("Sell box inventory is null");
            }

            var output = new StringBuilder();

            var itemsToSell = new List<(int Amount, IItemAsset ItemAsset, decimal SellPrice)>();
            var returnedItems = new List<Item>();

            decimal totalSellPrice = 0;

            foreach (var grouping in _sellBoxInventory.Items.GroupBy(x => x.id).ToList())
            {
                var shop = await _shopDirectory.GetShop(grouping.Key);

                if (shop != null && shop.CanSell() && await shop.IsPermitted(User))
                {
                    _itemShops[shop.Id] = shop;

                    var itemAsset = await _itemDirectory.FindByIdAsync(grouping.Key.ToString()) ??
                                    throw new InvalidOperationException(
                                        $"Item asset could not be found for item ID {grouping.Key}");

                    var amount = grouping.Count();

                    itemsToSell.Add((amount, itemAsset, shop.ShopData.SellPrice!.Value));

                    totalSellPrice += shop.ShopData.SellPrice!.Value * amount;
                }
                else
                {
                    returnedItems.AddRange(grouping);
                }
            }

            await ReturnItems(returnedItems, output);

            output.AppendLine();

            if (itemsToSell.Count == 0)
            {
                output.AppendLine(_stringLocalizer["ui:sellbox:selling_items:none"]);
            }
            else
            {
                output.AppendLine(_stringLocalizer["ui:sellbox:selling_items:header"]);

                foreach (var (amount, itemAsset, sellPrice) in itemsToSell)
                {
                    output.AppendLine(_stringLocalizer["ui:sellbox:selling_items:line",
                        new
                        {
                            Amount = amount, ItemAsset = itemAsset,
                            SellPrice = sellPrice * amount, UnitSellPrice = sellPrice,
                            _economyProvider.CurrencyName, _economyProvider.CurrencySymbol
                        }]);
                }
            }

            return (output.ToString(), totalSellPrice);
        }

        private bool _isSelling;

        private async UniTask OnSellItemsClick(string buttonName)
        {
            if (_isSelling)
            {
                return;
            }

            _isSelling = true;

            try
            {
                if (_sellBoxInventory == null)
                {
                    throw new Exception("Sell box inventory is null");
                }

                try
                {
                    decimal totalPrice = 0;
                    var totalAmount = 0;

                    foreach (var items in _sellBoxInventory.Items.GroupBy(x => x.id).ToList())
                    {
                        var id = items.Key;
                        var amount = items.Count();
                        var shop = _itemShops[id];

                        try
                        {
                            await shop.Sell(User, _sellBoxInventory, amount);

                            totalPrice += shop.ShopData.SellPrice!.Value * amount;
                            totalAmount += amount;
                        }
                        catch (UserFriendlyException ex)
                        {
                            _logger.LogWarning(ex, "User friendly error occurred when selling sellbox items.");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error occurred when selling sellbox items.");
                        }
                    }

                    await User.PrintMessageAsync(_stringLocalizer["commands:success:sellbox_sold",
                        new
                        {
                            TotalAmount = totalAmount,
                            TotalPrice = totalPrice,
                            ReturnedAmount = _totalReturned,
                            _economyProvider.CurrencySymbol,
                            _economyProvider.CurrencyName
                        }]);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred when attempting to sell items.");
                }
            }
            finally
            {
                try
                {
                    await EndAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred when closing UI session when selling items.");
                }
            }
        }
    }
}
