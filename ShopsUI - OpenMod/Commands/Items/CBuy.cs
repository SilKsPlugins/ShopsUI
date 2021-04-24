using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using System;

namespace ShopsUI.Commands.Items
{
    [Command("buy", Priority = Priority.High)]
    [CommandAlias("ibuy")]
    [CommandAlias("itembuy")]
    [CommandAlias("buyi")]
    [CommandAlias("buyitem")]
    [CommandSyntax("<item> [amount]")]
    [CommandDescription("Buys the item from the shop.")]
    [CommandActor(typeof(UnturnedUser))]
    public class CBuy : ShopCommand
    {
        public CBuy(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async UniTask OnExecuteAsync()
        {
            var asset = await GetItemAsset(0);
            var amount = await GetAmount(1);

            var shop = await ShopManager.GetItemShop(ushort.Parse(asset.ItemAssetId));

            if (shop == null || !shop.CanBuy())
                throw new UserFriendlyException(
                    StringLocalizer["commands:errors:no_buyable_item_shop", new {ItemAsset = asset}]);

            var balance = await shop.Buy(GetUnturnedUser(), amount);

            await PrintAsync(StringLocalizer["commands:success:item_bought",
                new
                {
                    Amount = amount,
                    ItemAsset = asset,
                    Price = shop.ShopData.BuyPrice * amount,
                    Balance = balance,
                    EconomyProvider.CurrencySymbol,
                    EconomyProvider.CurrencyName
                }]);
        }
    }
}
