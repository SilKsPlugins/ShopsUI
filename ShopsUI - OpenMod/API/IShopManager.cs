using System;
using System.Collections.Generic;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using ShopsUI.API.Items;
using ShopsUI.API.Vehicles;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI.API
{
    [Service]
    public interface IShopManager
    {
        Task<IItemShopData?> GetItemShopData(ushort id);
        Task<IVehicleShopData?> GetVehicleShopData(ushort id);

        Task<ICollection<IItemShopData>> GetItemShopDatasAsync(
            Func<IQueryable<IItemShopData>, IQueryable<IItemShopData>> query);
        Task<ICollection<IVehicleShopData>> GetVehicleShopDatasAsync(
            Func<IQueryable<IVehicleShopData>, IQueryable<IVehicleShopData>> query);

        Task<IItemShop?> GetItemShop(ushort id);
        Task<IVehicleShop?> GetVehicleShop(ushort id);

        Task<IItemShop> AddItemShopBuyable(ushort id, decimal price);
        Task<IItemShop> AddItemShopSellable(ushort id, decimal price);
        Task<IVehicleShop> AddVehicleShopBuyable(ushort id, decimal price);

        Task<bool> RemoveItemShopBuyable(ushort id);
        Task<bool> RemoveItemShopSellable(ushort id);
        Task<bool> RemoveVehicleShopBuyable(ushort id);

        Task<bool> AddItemWhitelist(ushort id, string permission);
        Task<bool> RemoveItemWhitelist(ushort id, string permission);

        Task<bool> AddItemBlacklist(ushort id, string permission);
        Task<bool> RemoveItemBlacklist(ushort id, string permission);

        Task<bool> AddVehicleWhitelist(ushort id, string permission);
        Task<bool> RemoveVehicleWhitelist(ushort id, string permission);

        Task<bool> AddVehicleBlacklist(ushort id, string permission);
        Task<bool> RemoveVehicleBlacklist(ushort id, string permission);
    }
}
