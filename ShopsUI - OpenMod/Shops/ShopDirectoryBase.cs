using Microsoft.Extensions.DependencyInjection;
using ShopsUI.API.Shops;
using ShopsUI.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopsUI.Shops
{
    public abstract class ShopDirectoryBase<TShopImpl, TShop, TShopData> : IShopDirectory<TShop, TShopData>
        where TShopImpl : TShop
        where TShop : IShop<TShopData>
        where TShopData : IShopData
    {
        private readonly IServiceProvider _serviceProvider;

        private bool _shopsLoaded;
        private TaskCompletionSource<bool>? _shopsLoadingTcs;

        private List<IShopCategory<TShopData>> _categories = new();
        private List<TShopData> _shops = new();

        protected ShopDirectoryBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected abstract Task<IEnumerable<IShopCategory<TShopData>>> GetAllCategories(ShopsDbContext dbContext);

        protected abstract Task<IEnumerable<TShopData>> GetAllShops(ShopsDbContext dbContext);

        public async Task Invalidate(bool reload = false)
        {
            if (!reload)
            {
                if (_shopsLoadingTcs != null)
                {
                    return;
                }

                _shopsLoaded = false;

                return;
            }

            if (_shopsLoadingTcs != null)
            {
                await _shopsLoadingTcs.Task;
                return;
            }

            _shopsLoaded = false;
            _shopsLoadingTcs = new TaskCompletionSource<bool>();

            try
            {
                await using var dbContext = _serviceProvider.GetRequiredService<ShopsDbContext>();

                _categories = (await GetAllCategories(dbContext)).ToList();
                _shops = (await GetAllShops(dbContext)).ToList();

                _shopsLoaded = true;
                _shopsLoadingTcs.SetResult(true);
            }
            catch (Exception ex)
            {
                _shopsLoadingTcs.SetException(ex);
            }
            finally
            {
                _shopsLoadingTcs = null;
            }
        }

        private async Task InvalidateIfNotLoaded()
        {
            if (!_shopsLoaded)
            {
                await Invalidate(true);
            }
        }

        public Task<IReadOnlyCollection<IShopCategory<TShopData>>> GetCategories() => GetCategoriesWithShopData();

        public async Task<IReadOnlyCollection<IShopCategory<TShopData>>> GetCategoriesWithShopData()
        {
            await InvalidateIfNotLoaded();

            return _categories.AsReadOnly();
        }

        public async Task<IShopCategory<TShopData>?> GetCategory(int id)
        {
            await InvalidateIfNotLoaded();

            return _categories.FirstOrDefault(x => x.Id == id);
        }

        public async Task<IShopCategory<TShopData>?> GetCategory(string name)
        {
            await InvalidateIfNotLoaded();

            return _categories.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public Task<TShop> GetShop(TShopData? shopData)
        {
            var shop = ActivatorUtilities.CreateInstance<TShopImpl>(_serviceProvider, shopData);

            return Task.FromResult<TShop>(shop);
        }

        public async Task<TShopData?> GetShopData(ushort id)
        {
            await InvalidateIfNotLoaded();

            return _shops.FirstOrDefault(x => x.Id == id);
        }

        public async Task<IReadOnlyCollection<TShopData>> GetShops()
        {
            await InvalidateIfNotLoaded();

            return _shops.AsReadOnly();
        }
    }
}
