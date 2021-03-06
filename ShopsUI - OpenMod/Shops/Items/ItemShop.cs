﻿using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OpenMod.API.Commands;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Unturned.Items;
using OpenMod.Unturned.Users;
using SDG.Unturned;
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

        public IItemShopData ShopData { get; }

        public ItemShop(
            IEconomyProvider economyProvider,
            IItemDirectory itemDirectory,
            IItemSpawner itemSpawner,
            IStringLocalizer stringLocalizer,
            ItemShopModel itemShopModel)
        {
            _economyProvider = economyProvider;
            _itemDirectory = itemDirectory;
            _itemSpawner = itemSpawner;
            _stringLocalizer = stringLocalizer;

            ShopData = itemShopModel;
        }

        private async Task<IItemAsset> GetItemAsset()
        {
            var id = ShopData.ItemId.ToString();

            var itemAsset = await _itemDirectory.FindByIdAsync(id);

            if (itemAsset == null || itemAsset is UnturnedItemAsset unturnedItemAsset && unturnedItemAsset.ItemAsset.isPro)
                throw new Exception($"Item id '{id}' has no asset");

            return itemAsset;
        }

        private void VerifyAmount(int amount)
        {
            if (amount < 1)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount was out of range");
        }

        public bool CanBuy() => ShopData.BuyPrice != null;

        public async Task<decimal> Buy(UnturnedUser user, int amount = 1)
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
                //var item = await _itemSpawner.GiveItemAsync(user.Player.Inventory, itemAsset);
                var success = ItemTool.tryForceGiveItem(user.Player.Player, ushort.Parse(itemAsset.ItemAssetId), 1);

                //if (item == null)
                if (!success)
                    throw new Exception($"Could not give item with id '{itemAsset.ItemAssetId}' to player");
            }

            return balance;
        }

        public bool CanSell() => ShopData.SellPrice != null;

        public async Task<decimal> Sell(UnturnedUser user, int amount = 1)
        {
            if (!CanSell())
                throw new InvalidOperationException("No selling at this shop");

            VerifyAmount(amount);

            var itemAsset = await GetItemAsset();

            await UniTask.SwitchToMainThread();

            var id = ShopData.ItemId.ToString();

            var count = user.Player.Inventory.Pages.SelectMany(x => x.Items)
                .Count(inventoryItem => inventoryItem.Item.Asset.ItemAssetId == id);

            if (count < amount)
                throw new UserFriendlyException(_stringLocalizer["commands:errors:not_enough_items",
                    new {CurrentAmount = count, NeededAmount = amount}]);

            var deleted = 0;

            foreach (var page in user.Player.Inventory.Pages)
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
    }
}
