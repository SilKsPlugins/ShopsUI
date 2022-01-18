using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShopsUI.API.Shops;
using ShopsUI.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace ShopsUI.Shops
{
    public abstract class CategoryEditorBase<TCategoryModel, TShopData, TShopDataModel, TShopCategoryModel> : ICategoryEditor
        where TCategoryModel : class, IShopCategory<TShopData>
        where TShopData : IShopData
        where TShopDataModel : class, IShopData
        where TShopCategoryModel : class
    {
        private readonly IShopDirectoryInvalidator _shopDirectoryInvalidator;
        private readonly ShopsDbContext _dbContext;

        protected CategoryEditorBase(IShopDirectoryInvalidator shopDirectoryInvalidator,
            IServiceProvider serviceProvider)
        {
            _shopDirectoryInvalidator = shopDirectoryInvalidator;
            _dbContext = serviceProvider.GetRequiredService<ShopsDbContext>();
        }

        protected abstract TCategoryModel CreateCategoryModel(string displayName);

        protected abstract TShopCategoryModel CreateShopCategoryModel(int categoryId, ushort shopId);

        protected abstract void RemoveShopFromCategoryModel(TCategoryModel category, ushort shopId);

        protected abstract IIncludableQueryable<TCategoryModel, TShopDataModel> GetIncludeShopsQueryable(
            ShopsDbContext dbContext);

        public async Task<bool> CreateCategory(string displayName)
        {
            var category = await _dbContext.Set<TCategoryModel>()
                .FirstOrDefaultAsync(x => x.Name.Equals(displayName, StringComparison.OrdinalIgnoreCase));

            if (category != null)
            {
                return false;
            }

            category = CreateCategoryModel(displayName);

            await _dbContext.Set<TCategoryModel>().AddAsync(category);

            await _dbContext.SaveChangesAsync();

            _shopDirectoryInvalidator.InvalidateFireAndForget();

            return true;
        }

        public async Task<bool> DeleteCategory(string displayName)
        {
            var category = await _dbContext.Set<TCategoryModel>()
                .FirstOrDefaultAsync(x => x.Name.Equals(displayName, StringComparison.OrdinalIgnoreCase));

            if (category == null)
            {
                return false;
            }
            
            _dbContext.Set<TCategoryModel>().Remove(category);

            await _dbContext.SaveChangesAsync();

            _shopDirectoryInvalidator.InvalidateFireAndForget();

            return true;
        }

        public async Task<int?> AddShopsToCategory(string displayName, IEnumerable<ushort> shopIds)
        {
            var category = await GetIncludeShopsQueryable(_dbContext)
                .FirstOrDefaultAsync(x => x.Name.Equals(displayName, StringComparison.OrdinalIgnoreCase));

            if (category?.Shops == null)
            {
                return null;
            }

            var validShopIds = await _dbContext.Set<TShopDataModel>().Select(x => x.Id).ToListAsync();
            var existingShopIds = category.Shops.Select(x => x.Id).ToList();

            var shopsToAdd =
                shopIds.Where(shopId => validShopIds.Contains(shopId) && !existingShopIds.Contains(shopId));

            foreach (var shopId in shopsToAdd)
            {
                var relationshipModel = CreateShopCategoryModel(category.Id, shopId);

                await _dbContext.Set<TShopCategoryModel>().AddAsync(relationshipModel);
            }

            var results = await _dbContext.SaveChangesAsync();

            _shopDirectoryInvalidator.InvalidateFireAndForget();

            return results;
        }

        public async Task<int?> RemoveShopsFromCategory(string displayName, IEnumerable<ushort> shopIds)
        {
            var category = await GetIncludeShopsQueryable(_dbContext)
                .FirstOrDefaultAsync(x => x.Name.Equals(displayName, StringComparison.OrdinalIgnoreCase));

            if (category?.Shops == null)
            {
                return null;
            }

            foreach (var shopId in shopIds)
            {
                RemoveShopFromCategoryModel(category, shopId);
            }

            _dbContext.Update(category);

            var results = await _dbContext.SaveChangesAsync();

            _shopDirectoryInvalidator.InvalidateFireAndForget();

            return results;
        }
    }
}
