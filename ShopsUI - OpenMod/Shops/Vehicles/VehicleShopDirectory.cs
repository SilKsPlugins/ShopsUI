using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using ShopsUI.API.Shops;
using ShopsUI.API.Shops.Vehicles;
using ShopsUI.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopsUI.Shops.Vehicles
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class VehicleShopDirectory : ShopDirectoryBase<VehicleShop, IVehicleShop, IVehicleShopData>, IVehicleShopDirectory
    {
        public VehicleShopDirectory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async Task<IEnumerable<IShopCategory<IVehicleShopData>>> GetAllCategories(ShopsDbContext dbContext)
        {
            return await dbContext.VehicleCategories
                .Include(category => category.VehicleShops)
                .ThenInclude(shopCategory => shopCategory.VehicleShop)
                .ThenInclude(shop => shop.AuthGroups)
                .ToListAsync();
        }

        protected override async Task<IEnumerable<IVehicleShopData>> GetAllShops(ShopsDbContext dbContext)
        {
            return await dbContext.VehicleShops.Include(shop => shop.AuthGroups)
                .ToListAsync();
        }
    }
}
