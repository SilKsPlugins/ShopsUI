using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using System;

namespace ShopsUI.Commands.Items
{
    [Command("sell", Priority = Priority.High)]
    [CommandAlias("isell")]
    [CommandAlias("itemsell")]
    [CommandAlias("selli")]
    [CommandAlias("sellitem")]
    [CommandSyntax("<item> [amount]")]
    [CommandDescription("Sells an item to the shop.")]
    [CommandActor(typeof(UnturnedUser))]
    public class CSell : ShopCommand
    {
        public CSell(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetItemAsset(0);
            var amount = await GetAmount(1);

            var shop = await ShopManager.GetItemShop(ushort.Parse(asset.ItemAssetId));

            if (shop == null || !shop.CanSell())
                throw new UserFriendlyException(
                    StringLocalizer["commands:errors:no_sellable_item_shop", new { ItemAsset = asset }]);

            if (!await shop.IsPermitted(Context.Actor))
            {
                throw new UserFriendlyException(StringLocalizer["commands:errors:not_permitted_item",
                    new { ItemAsset = asset }]);
            }

            var balance = await shop.Sell(GetUnturnedUser(), amount);

            await PrintAsync(StringLocalizer["commands:success:item_sold",
                new
                {
                    Amount = amount,
                    ItemAsset = asset,
                    Price = shop.ShopData.SellPrice * amount,
                    Balance = balance,
                    EconomyProvider.CurrencySymbol,
                    EconomyProvider.CurrencyName
                }]);
        }
    }
}
