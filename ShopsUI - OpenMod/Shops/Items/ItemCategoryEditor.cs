extern alias JetBrainsAnnotations;
using JetBrainsAnnotations::JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using ShopsUI.API.Shops.Items;
using ShopsUI.Database;
using ShopsUI.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShopsUI.Shops.Items
{
    [UsedImplicitly]
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class ItemCategoryEditor : CategoryEditorBase<ItemCategoryModel, IItemShopData, ItemShopModel, ItemShopCategoryModel>, IItemCategoryEditor
    {
        public ItemCategoryEditor(IItemShopDirectory shopDirectory,
            IServiceProvider serviceProvider) : base(shopDirectory, serviceProvider)
        {
        }

        protected override ItemCategoryModel CreateCategoryModel(string displayName)
        {
            return new()
            {
                Name = displayName
            };
        }

        protected override ItemShopCategoryModel CreateShopCategoryModel(int categoryId, ushort shopId)
        {
            return new()
            {
                ItemCategoryId = categoryId,
                ItemShopId = shopId
            };
        }

        protected override void RemoveShopFromCategoryModel(ItemCategoryModel category, ushort shopId)
        {
            var shop = category.ItemShops?.FirstOrDefault(x => x.ItemShopId == shopId);

            if (shop != null)
            {
                category.ItemShops!.Remove(shop);
            }
        }

        protected override IIncludableQueryable<ItemCategoryModel, ICollection<ItemShopCategoryModel>?> GetIncludeShopsQueryable(ShopsDbContext dbContext)
        {
            return dbContext.Set<ItemCategoryModel>()
                .Include(x => x.ItemShops);
        }
    }
}
