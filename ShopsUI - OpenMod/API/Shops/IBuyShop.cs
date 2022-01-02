using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Extensions.Games.Abstractions.Players;

namespace ShopsUI.API.Shops
{
    public interface IBuyShop<out TShopData> : IShop<TShopData> where TShopData : IShopData
    {
        bool CanBuy();

        UniTask<decimal> Buy<TPlayer>(IPlayerUser<TPlayer> user, int amount = 1) where TPlayer : IPlayer, IHasInventory;
    }
}
