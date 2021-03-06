﻿using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using System;

namespace ShopsUI.Commands.Items
{
    [Command("cost", Priority = Priority.High)]
    [CommandAlias("icost")]
    [CommandAlias("itemcost")]
    [CommandAlias("costi")]
    [CommandAlias("costitem")]
    [CommandSyntax("<item> [amount]")]
    [CommandDescription("Checks the price of an item in the shop.")]
    public class CCost : ShopCommand
    {
        public CCost(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetItemAsset(0);
            var amount = await GetAmount(1);

            var shop = await ShopManager.GetItemShopData(ushort.Parse(asset.ItemAssetId));

            if (shop == null || shop.BuyPrice == null && shop.SellPrice == null)
                throw new UserFriendlyException(
                    StringLocalizer["commands:errors:no_item_shop", new {ItemAsset = asset}]);

            if (shop.BuyPrice != null && shop.SellPrice != null)
            {
                // Buy and sell
                await PrintAsync(StringLocalizer["commands:success:item_cost:buy_and_sell",
                    new
                    {
                        ItemAsset = asset,
                        Amount = amount,
                        BuyPrice = shop.BuyPrice * amount,
                        SellPrice = shop.SellPrice * amount,
                        EconomyProvider.CurrencySymbol,
                        EconomyProvider.CurrencyName
                    }]);
            }
            else if (shop.BuyPrice != null)
            {
                // Buy
                await PrintAsync(StringLocalizer["commands:success:item_cost:buy",
                    new
                    {
                        ItemAsset = asset,
                        Amount = amount,
                        BuyPrice = shop.BuyPrice * amount,
                        EconomyProvider.CurrencySymbol,
                        EconomyProvider.CurrencyName
                    }]);
            }
            else
            {
                // Sell
                await PrintAsync(StringLocalizer["commands:success:item_cost:sell",
                    new
                    {
                        ItemAsset = asset,
                        Amount = amount,
                        SellPrice = shop.SellPrice * amount,
                        EconomyProvider.CurrencySymbol,
                        EconomyProvider.CurrencyName
                    }]);
            }
        }
    }
}
