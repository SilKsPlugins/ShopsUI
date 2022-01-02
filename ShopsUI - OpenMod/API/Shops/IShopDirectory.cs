using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopsUI.API.Shops
{
    public interface IShopDirectory<TShop, TShopData> : IShopDirectoryInvalidator
        where TShopData : IShopData
        where TShop : IShop<TShopData>
    {
        Task<TShopData?> GetShopData(ushort id);

        Task<TShop> GetShop(TShopData? shopData);

        Task<IReadOnlyCollection<TShopData>> GetShops();

        Task<IShopCategory<TShopData>?> GetCategory(int id);

        Task<IShopCategory<TShopData>?> GetCategory(string name);

        Task<IReadOnlyCollection<IShopCategory<TShopData>>> GetCategories();

        Task<IReadOnlyCollection<IShopCategory<TShopData>>> GetCategoriesWithShopData();
    }
}
