extern alias JetBrainsAnnotations;
using JetBrainsAnnotations::JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using ShopsUI.API.Shops.Vehicles;
using ShopsUI.Database;
using ShopsUI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopsUI.Shops.Vehicles
{
    [UsedImplicitly]
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class VehicleCategoryEditor : CategoryEditorBase<VehicleCategoryModel, IVehicleShopData, VehicleShopModel, VehicleShopCategoryModel>, IVehicleCategoryEditor
    {
        public VehicleCategoryEditor(IVehicleShopDirectory shopDirectory,
            IServiceProvider serviceProvider) : base(shopDirectory, serviceProvider)
        {
        }

        protected override VehicleCategoryModel CreateCategoryModel(string displayName)
        {
            return new()
            {
                Name = displayName
            };
        }

        protected override VehicleShopCategoryModel CreateShopCategoryModel(int categoryId, ushort shopId)
        {
            return new()
            {
                VehicleCategoryId = categoryId,
                VehicleShopId = shopId
            };
        }

        protected override void RemoveShopFromCategoryModel(VehicleCategoryModel category, ushort shopId)
        {
            var shop = category.VehicleShops?.FirstOrDefault(x => x.VehicleShopId == shopId);

            if (shop != null)
            {
                category.VehicleShops!.Remove(shop);
            }
        }

        protected override IIncludableQueryable<VehicleCategoryModel, ICollection<VehicleShopCategoryModel>?> GetIncludeShopsQueryable(ShopsDbContext dbContext)
        {
            return dbContext.Set<VehicleCategoryModel>()
                .Include(x => x.VehicleShops);
        }
    }
}
