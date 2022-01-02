using System.Threading.Tasks;

namespace ShopsUI.API.Shops.Whitelist
{
    public interface IShopWhitelistEditor
    {
        Task<bool> AddWhitelist(ushort id, string permission);
        Task<bool> RemoveWhitelist(ushort id, string permission);

        Task<bool> AddBlacklist(ushort id, string permission);
        Task<bool> RemoveBlacklist(ushort id, string permission);
    }
}
