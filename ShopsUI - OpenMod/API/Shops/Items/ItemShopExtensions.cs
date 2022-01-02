using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Players;
using System;

namespace ShopsUI.API.Shops.Items
{
    public static class ItemShopExtensions
    {
        public static async UniTask<decimal> Sell<TShopData, TPlayer>(
            this ISellShop<TShopData> shop, IPlayerUser<TPlayer> user, int amount = 1)
            where TShopData : IShopData
            where TPlayer : IPlayer, IHasInventory
        {
            var inventory = user.Player.Inventory ?? throw new Exception("Player has no inventory");

            return await shop.Sell(user, inventory, amount);
        }
    }
}
