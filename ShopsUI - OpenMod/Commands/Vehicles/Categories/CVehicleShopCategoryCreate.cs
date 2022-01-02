using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Vehicles;
using ShopsUI.Commands.Base.Categories;
using System;

namespace ShopsUI.Commands.Vehicles.Categories
{
    [Command("create", Priority = Priority.High)]
    [CommandAlias("c")]
    [CommandSyntax("<category name>")]
    [CommandDescription("Create a shop category.")]
    [CommandParent(typeof(CVehicleShopCategory))]
    public class CVehicleShopCategoryCreate : ShopCategoryCreateCommand<IVehicleCategoryEditor>
    {
        public CVehicleShopCategoryCreate(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override string GetSuccessLocalizationKey()
        {
            return "commands:success:shop_category:created:vehicle";
        }
    }
}
