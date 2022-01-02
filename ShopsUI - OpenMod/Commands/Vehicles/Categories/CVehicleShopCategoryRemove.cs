using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Vehicles;
using ShopsUI.Commands.Base.Categories;
using System;

namespace ShopsUI.Commands.Vehicles.Categories
{
    [Command("remove", Priority = Priority.High)]
    [CommandAlias("rem")]
    [CommandAlias("r")]
    [CommandAlias("-")]
    [CommandSyntax("<category name> <shops>")]
    [CommandDescription("Removes the specified vehicle shops from the category.")]
    [CommandParent(typeof(CVehicleShopCategory))]
    public class CVehicleShopCategoryRemove : ShopCategoryRemoveCommand<IVehicleCategoryEditor>
    {
        public CVehicleShopCategoryRemove(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override string GetSuccessLocalizationKey()
        {
            return "commands:success:shop_category:removed:vehicle";
        }
    }
}