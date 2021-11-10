using OpenMod.Unturned.Users;
using System.Threading.Tasks;

namespace ShopsUI.API.Items
{
    public static class ItemShopExtensions
    {
        public static Task<decimal> Buy(this IItemShop shop, UnturnedUser user, int amount = 1)
        {
            return shop.Buy(user, user.Player.Inventory, amount);
        }

        public static Task<decimal> Sell(this IItemShop shop, UnturnedUser user, int amount = 1)
        {
            return shop.Sell(user, user.Player.Inventory, amount);
        }

        public static async Task<bool> TryBuy(this IItemShop shop, UnturnedUser user)
        {
            if (shop.CanBuy()) return false;

            await shop.Buy(user, user.Player.Inventory);

            return true;
        }

        public static async Task<bool> TrySell(this IItemShop shop, UnturnedUser user)
        {
            if (shop.CanSell()) return false;

            await shop.Sell(user);

            return true;
        }
    }
}
