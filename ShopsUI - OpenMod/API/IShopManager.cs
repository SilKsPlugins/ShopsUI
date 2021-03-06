using OpenMod.API.Ioc;
using ShopsUI.API.Items;
using ShopsUI.API.Vehicles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI.API
{
    [Service]
    public interface IShopManager
    {
        IQueryable<IItemShopData> GetItemShopDatas();
        IQueryable<IVehicleShopData> GetVehicleShopDatas();

        Task<IItemShopData?> GetItemShopData(ushort id);
        Task<IVehicleShopData?> GetVehicleShopData(ushort id);

        Task<IItemShop?> GetItemShop(ushort id);
        Task<IVehicleShop?> GetVehicleShop(ushort id);

        Task<IItemShop> AddItemShopBuyable(ushort id, decimal price);
        Task<IItemShop> AddItemShopSellable(ushort id, decimal price);
        Task<IVehicleShop> AddVehicleShopBuyable(ushort id, decimal price);

        Task<bool> RemoveItemShopBuyable(ushort id);
        Task<bool> RemoveItemShopSellable(ushort id);
        Task<bool> RemoveVehicleShopBuyable(ushort id);
    }
}
