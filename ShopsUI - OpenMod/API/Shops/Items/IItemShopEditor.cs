using OpenMod.API.Ioc;
using System.Threading.Tasks;

namespace ShopsUI.API.Shops.Items
{
    [Service]
    public interface IItemShopEditor
    {
        Task<IItemShopData> AddItemShopBuyable(ushort id, decimal price);
        Task<IItemShopData> AddItemShopSellable(ushort id, decimal price);

        Task<bool> RemoveItemShopBuyable(ushort id);
        Task<bool> RemoveItemShopSellable(ushort id);
    }
}
