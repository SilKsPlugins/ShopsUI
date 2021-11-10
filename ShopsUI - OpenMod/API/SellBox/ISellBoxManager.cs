using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.Unturned.Users;

namespace ShopsUI.API.SellBox
{
    [Service]
    public interface ISellBoxManager
    {
        UniTask OpenSellBox(UnturnedUser user);
    }
}
