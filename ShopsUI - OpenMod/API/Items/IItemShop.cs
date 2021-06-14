using OpenMod.Unturned.Users;
using System.Threading.Tasks;
using OpenMod.API.Permissions;

namespace ShopsUI.API.Items
{
    public interface IItemShop
    {
        IItemShopData ShopData { get; }
        
        bool CanBuy();
        Task<decimal> Buy(UnturnedUser user, int amount = 1);

        bool CanSell();
        Task<decimal> Sell(UnturnedUser user, int amount = 1);

        Task<bool> IsPermitted(IPermissionActor user);
    }
}
