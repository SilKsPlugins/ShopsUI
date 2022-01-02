using Microsoft.Extensions.DependencyInjection;
using ShopsUI.API.Shops.Items;
using ShopsUI.API.Shops.Vehicles;
using System;

namespace ShopsUI.Commands.Base.Categories
{
    public abstract class ShopCategoryCommandBase : ShopCommand
    {
        protected readonly IItemCategoryEditor ItemCategoryEditor;
        protected readonly IVehicleCategoryEditor VehicleCategoryEditor;

        protected ShopCategoryCommandBase(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            ItemCategoryEditor = serviceProvider.GetRequiredService<IItemCategoryEditor>();
            VehicleCategoryEditor = serviceProvider.GetRequiredService<IVehicleCategoryEditor>();
        }
    }
}
