namespace ShopsUI.API.Shops
{
    public interface IShop<out TShopData> where TShopData : IShopData
    {
        ushort Id { get; }

        TShopData ShopData { get; }
    }
}