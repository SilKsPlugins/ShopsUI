using OpenMod.API.Ioc;
using System.Threading.Tasks;

namespace ShopsUI.API.Shops.Vehicles
{
    [Service]
    public interface IVehicleShopEditor
    {
        Task<IVehicleShopData> AddVehicleShopBuyable(ushort id, decimal price);
        Task<bool> RemoveVehicleShopBuyable(ushort id);
    }
}
