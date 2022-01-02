namespace ShopsUI.API.Shops.Items
{
    public interface IItemShop :
        IBuyShop<IItemShopData>,
        ISellShop<IItemShopData>,
        IPermissionShop<IItemShopData>
    {
    }
}
