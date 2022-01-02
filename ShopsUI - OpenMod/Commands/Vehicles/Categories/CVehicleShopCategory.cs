using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.Commands.Base.Categories;
using System;

namespace ShopsUI.Commands.Vehicles.Categories
{
    [Command("category", Priority = Priority.High)]
    [CommandAlias("c")]
    [CommandDescription("Manage shop categories.")]
    [CommandParent(typeof(CVShop))]
    public class CVehicleShopCategory : ShopCategoryCommand
    {
        protected CVehicleShopCategory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
