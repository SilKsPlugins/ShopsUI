using System.Threading.Tasks;

namespace ShopsUI.API.Shops
{
    public static class ShopDirectoryExtensions
    {
        public static async Task<TShop?> GetShop<TShop, TShopData>(this IShopDirectory<TShop, TShopData> shopDirectory, ushort id)
            where TShopData : IShopData
            where TShop : IShop<TShopData>
        {
            var shopData = await shopDirectory.GetShopData(id);

            return shopData == null ? default : await shopDirectory.GetShop(shopData);
        }

        public static void InvalidateFireAndForget(this IShopDirectoryInvalidator shopDirectoryInvalidator,
            bool reload = false)
        {
            var _ = Task.Run(() => shopDirectoryInvalidator.Invalidate(reload));
        }
    }
}
