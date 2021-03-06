using Cysharp.Threading.Tasks;
using OpenMod.API.Ioc;
using OpenMod.Unturned.Users;

namespace ShopsUI.API.UI
{
    [Service]
    public interface IUIManager
    {
        UniTask StartSession(UnturnedUser user, UITab tab = UITab.Items);
        UniTask EndSession(UnturnedUser user);
    }
}
