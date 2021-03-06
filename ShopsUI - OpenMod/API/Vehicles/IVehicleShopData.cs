namespace ShopsUI.API.Vehicles
{
    public interface IVehicleShopData
    {
        public ushort VehicleId { get; }

        public decimal BuyPrice { get; }
    }
}
