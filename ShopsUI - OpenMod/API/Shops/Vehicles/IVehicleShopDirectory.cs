using OpenMod.API.Ioc;

namespace ShopsUI.API.Shops.Vehicles
{
    [Service]
    public interface IVehicleShopDirectory : IShopDirectory<IVehicleShop, IVehicleShopData>
    {
    }
}
