using Cysharp.Threading.Tasks;
using OpenMod.API.Permissions;

namespace ShopsUI.API.Shops
{
    public interface IPermissionShop<out TShopData> : IShop<TShopData> where TShopData : IShopData
    {
        UniTask<bool> IsPermitted(IPermissionActor user);
    }
}
