namespace ShopsUI.API.Shops.Items
{
    public interface IItemShopData : IShopData
    {
        public decimal? BuyPrice { get; }

        public decimal? SellPrice { get; }
    }
}
