using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops;
using ShopsUI.API.Shops.Vehicles;
using ShopsUI.Commands.Base.Categories;
using System;
using System.Threading.Tasks;

namespace ShopsUI.Commands.Vehicles.Categories
{
    [Command("delete", Priority = Priority.High)]
    [CommandAlias("d")]
    [CommandSyntax("<category name> [-f|--force]")]
    [CommandDescription("Deletes the specified category (use -f to not have to confirm).")]
    [CommandParent(typeof(CVehicleShopCategory))]
    public class CVehicleShopCategoryDelete : ShopCategoryDeleteCommand<IVehicleCategoryEditor, IVehicleShopData>
    {
        public CVehicleShopCategoryDelete(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async Task<IShopCategory<IVehicleShopData>?> GetCategory(string name)
        {
            return await VehicleShopDirectory.GetCategory(name);
        }

        protected override string GetSuccessLocalizationKey()
        {
            return "commands:success:shop_category:deleted:vehicle";
        }

        protected override string GetConfirmationLocalizationKey()
        {
            return "commands:success:shop_category:deleted:confirm:vehicle";
        }
    }
}
