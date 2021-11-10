using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Extensions.Games.Abstractions.Items;
using System.Threading.Tasks;

namespace ShopsUI.API.Items
{
    public interface IItemShop
    {
        IItemShopData ShopData { get; }
        
        bool CanBuy();
        Task<decimal> Buy(IUser user, IInventory inventory, int amount = 1);

        bool CanSell();
        Task<decimal> Sell(IUser user, IInventory inventory, int amount = 1);

        Task<bool> IsPermitted(IPermissionActor user);
    }
}
