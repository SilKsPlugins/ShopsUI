using ShopsUI.API.Shops;
using System.Collections.Generic;

namespace ShopsUI.Shops
{
    public class BasicShopCategory<TShopData> : IShopCategory<TShopData> where TShopData : IShopData
    {
        public int Id => int.MinValue;

        public string Name { get; }

        public IReadOnlyCollection<TShopData> Shops { get; }

        public BasicShopCategory(string name, IReadOnlyCollection<TShopData> shops)
        {
            Name = name;
            Shops = shops;
        }
    }
}
