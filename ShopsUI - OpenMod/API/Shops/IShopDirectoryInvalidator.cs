using System.Threading.Tasks;

namespace ShopsUI.API.Shops
{
    public interface IShopDirectoryInvalidator
    {
        Task Invalidate(bool reload = false);
    }
}
