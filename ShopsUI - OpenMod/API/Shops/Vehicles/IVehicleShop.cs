namespace ShopsUI.API.Shops.Vehicles
{
    public interface IVehicleShop :
        IBuyShop<IVehicleShopData>,
        IPermissionShop<IVehicleShopData>
    {
    }
}
