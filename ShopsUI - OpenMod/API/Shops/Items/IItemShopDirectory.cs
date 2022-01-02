using OpenMod.API.Ioc;

namespace ShopsUI.API.Shops.Items
{
    [Service]
    public interface IItemShopDirectory : IShopDirectory<IItemShop, IItemShopData>
    {
    }
}
