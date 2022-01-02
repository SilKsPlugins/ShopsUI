using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using ShopsUI.API.Shops;
using ShopsUI.API.Shops.Items;
using ShopsUI.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopsUI.Shops.Items
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class ItemShopDirectory : ShopDirectoryBase<ItemShop, IItemShop, IItemShopData>, IItemShopDirectory
    {
        public ItemShopDirectory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async Task<IEnumerable<IShopCategory<IItemShopData>>> GetAllCategories(ShopsDbContext dbContext)
        {
            return await dbContext.ItemCategories
                .Include(category => category.ItemShops)
                .ThenInclude(shopCategory => shopCategory.ItemShop)
                .ThenInclude(shop => shop.AuthGroups)
                .ToListAsync();
        }

        protected override async Task<IEnumerable<IItemShopData>> GetAllShops(ShopsDbContext dbContext)
        {
            return await dbContext.ItemShops.Include(shop => shop.AuthGroups)
                .ToListAsync();
        }
    }
}
