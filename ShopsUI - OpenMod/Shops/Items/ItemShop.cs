using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Unturned.Items;
using ShopsUI.API.Items;
using ShopsUI.Database.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI.Shops.Items
{
    public class ItemShop : IItemShop
    {
        private readonly IEconomyProvider _economyProvider;
        private readonly IItemDirectory _itemDirectory;
        private readonly IItemSpawner _itemSpawner;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IPermissionRegistry _permissionRegistry;
        private readonly IConfiguration _configuration;
        private readonly IOpenModComponent _openModComponent;
        private readonly ILogger<ItemShop> _logger;

        public ItemShopModel ShopData { get; }

        IItemShopData IItemShop.ShopData => ShopData;

        public const string PermissionFormat = "groups.{0}";

        public ItemShop(
            IEconomyProvider economyProvider,
            IItemDirectory itemDirectory,
            IItemSpawner itemSpawner,
            IStringLocalizer stringLocalizer,
            IPermissionChecker permissionChecker,
            IPermissionRegistry permissionRegistry,
            IConfiguration configuration,
            IOpenModComponent openModComponent,
            ILogger<ItemShop> logger,
            ItemShopModel itemShopModel)
        {
            _economyProvider = economyProvider;
            _itemDirectory = itemDirectory;
            _itemSpawner = itemSpawner;
            _stringLocalizer = stringLocalizer;
            _permissionChecker = permissionChecker;
            _permissionRegistry = permissionRegistry;
            _configuration = configuration;
            _openModComponent = openModComponent;
            _logger = logger;

            ShopData = itemShopModel;
        }

        private async Task<UnturnedItemAsset> GetItemAsset()
        {
            var id = ShopData.ItemId.ToString();

            var itemAsset = await _itemDirectory.FindByIdAsync(id);

            if (itemAsset is not UnturnedItemAsset unturnedItemAsset || unturnedItemAsset.ItemAsset.isPro)
                throw new Exception($"Item id '{id}' has no asset");

            return unturnedItemAsset;
        }

        private void VerifyAmount(int amount)
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount was out of range");
        }

        public bool CanBuy() => ShopData.BuyPrice != null;

        public async Task<decimal> Buy(IUser user, IInventory inventory, int amount = 1)
        {
            if (!CanBuy())
                throw new InvalidOperationException("No buying at this shop");

            VerifyAmount(amount);
            
            var itemAsset = await GetItemAsset();

            await UniTask.SwitchToMainThread();

            var price = ShopData.BuyPrice!.Value * amount;

            var balance = await _economyProvider.UpdateBalanceAsync(user.Id, user.Type, -price,
                _stringLocalizer["transactions:items:bought",
                    new
                    {
                        ItemAsset = itemAsset,
                        Amount = amount,
                        Price = price,
                        _economyProvider.CurrencySymbol,
                        _economyProvider.CurrencyName
                    }]);

            for (var i = 0; i < amount; i++)
            {
                var itemState = new AdminItemState(itemAsset);

                var item = await _itemSpawner.GiveItemAsync(inventory, itemAsset, itemState);

                if (item == null)
                {
                    throw new Exception($"Could not give item with id '{itemAsset.ItemAssetId}' to player");
                }
            }

            return balance;
        }

        public bool CanSell() => ShopData.SellPrice != null;

        public async Task<decimal> Sell(IUser user, IInventory inventory, int amount = 1)
        {
            if (!CanSell())
                throw new InvalidOperationException("No selling at this shop");

            VerifyAmount(amount);

            var itemAsset = await GetItemAsset();

            await UniTask.SwitchToMainThread();

            var id = ShopData.ItemId.ToString();

            var count = inventory.Pages.SelectMany(x => x.Items)
                .Count(inventoryItem => inventoryItem.Item.Asset.ItemAssetId == id);

            if (count < amount)
                throw new UserFriendlyException(_stringLocalizer["commands:errors:not_enough_items",
                    new {CurrentAmount = count, NeededAmount = amount}]);
            
            var deleted = 0;

            foreach (var page in inventory.Pages)
            {
                var items = page.Items.ToList();

                for (var i = items.Count - 1; i >= 0 && deleted < amount; i--)
                {
                    if (items[i].Item.Asset.ItemAssetId != id) continue;

                    await items[i].DestroyAsync();
                    deleted++;
                }

                if (deleted >= amount) break;
            }

            if (deleted != amount)
                throw new Exception($"Deleted items count ({deleted}) is not equal to sell amount ({amount})");

            var price = ShopData.SellPrice!.Value * amount;

            return await _economyProvider.UpdateBalanceAsync(user.Id, user.Type, price,
                _stringLocalizer["transactions:items:sold",
                    new
                    {
                        ItemAsset = itemAsset,
                        Amount = amount,
                        Price = price,
                        _economyProvider.CurrencySymbol,
                        _economyProvider.CurrencyName
                    }]);
        }

        public async Task<bool> IsPermitted(IPermissionActor user)
        {
            var blacklistEnabled = _configuration.GetValue("shops:blacklistEnabled", false);
            var whitelistEnabled = _configuration.GetValue("shops:whitelistEnabled", false);

            if (!blacklistEnabled && !whitelistEnabled) return true;

            var groups = ShopData.AuthGroups.ToList();

            if (groups.Count == 0) return true;

            var blacklistedGroups = groups.Where(x => !x.IsWhitelist).ToList();
            var whitelistedGroups = groups.Where(x => x.IsWhitelist).ToList();

            if (whitelistEnabled && whitelistedGroups.Count > 0)
            {
                if (blacklistEnabled && blacklistedGroups.Count > 0)
                {
                    _logger.LogWarning(
                        $"Item shop with id {ShopData.ItemId} has both a whitelist and a blacklist. Defaulting to using whitelist.");
                }

                foreach (var group in whitelistedGroups)
                {
                    var permission = string.Format(PermissionFormat, group.Permission);

                    if (_permissionRegistry.FindPermission(_openModComponent, permission) == null)
                    {
                        _permissionRegistry.RegisterPermission(_openModComponent, permission,
                            description: "Provides/denies access to a shop.");
                    }

                    if (await _permissionChecker.CheckPermissionAsync(user, permission) == PermissionGrantResult.Grant)
                    {
                        return true;
                    }
                }

                return false;
            }

            if (blacklistEnabled && blacklistedGroups.Count > 0)
            {
                foreach (var group in blacklistedGroups)
                {
                    var permission = string.Format(PermissionFormat, group.Permission);

                    if (_permissionRegistry.FindPermission(_openModComponent, permission) == null)
                    {
                        _permissionRegistry.RegisterPermission(_openModComponent, permission,
                            description: "Provides/denies access to a shop.");
                    }

                    if (await _permissionChecker.CheckPermissionAsync(user, permission) == PermissionGrantResult.Grant)
                    {
                        return false;
                    }
                }

                return true;
            }

            return true;
        }
    }
}
