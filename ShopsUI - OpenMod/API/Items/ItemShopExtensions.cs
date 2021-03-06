using OpenMod.Unturned.Users;

namespace ShopsUI.API.Items
{
    public static class ItemShopExtensions
    {
        public static bool TryBuy(this IItemShop shop, UnturnedUser user, int amount = 1)
        {
            if (shop.CanBuy()) return false;

            shop.Buy(user, amount);

            return true;
        }

        public static bool TrySell(this IItemShop shop, UnturnedUser user, int amount = 1)
        {
            if (shop.CanSell()) return false;

            shop.Sell(user, amount);

            return true;
        }
    }
}
