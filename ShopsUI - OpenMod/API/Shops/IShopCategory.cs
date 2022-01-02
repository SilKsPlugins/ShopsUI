using System.Collections.Generic;

namespace ShopsUI.API.Shops
{
    public interface IShopCategory<out TShop> where TShop : IShopData
    {
        int Id { get; }

        string Name { get; }

        IReadOnlyCollection<TShop>? Shops { get; }
    }
}
