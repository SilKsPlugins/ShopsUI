using OpenMod.Unturned.Users;
using System.Threading.Tasks;

namespace ShopsUI.API.Items
{
    public static class ItemShopExtensions
    {
        public static async Task<bool> TryBuy(this IItemShop shop, UnturnedUser user)
        {
            if (shop.CanBuy()) return false;

            await shop.Buy(user);

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
