using Cysharp.Threading.Tasks;
using OpenMod.Unturned.Users;

namespace ShopsUI.API.UI
{
    public interface IUISession
    {
        UnturnedUser User { get; }

        UniTask StartSession(UnturnedUser user, UITab tab = UITab.Items);
        UniTask EndSession();
    }
}
