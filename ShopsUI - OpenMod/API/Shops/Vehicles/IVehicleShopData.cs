namespace ShopsUI.API.Shops.Vehicles
{
    public interface IVehicleShopData : IShopData
    {
        public decimal BuyPrice { get; }
    }
}
