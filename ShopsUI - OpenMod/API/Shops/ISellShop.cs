using Cysharp.Threading.Tasks;
using OpenMod.API.Users;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Players;

namespace ShopsUI.API.Shops
{
    public interface ISellShop<out TShopData> : IShop<TShopData> where TShopData : IShopData
    {
        bool CanSell();

        UniTask<decimal> Sell(IUser user, IInventory inventory, int amount = 1);
    }
}
