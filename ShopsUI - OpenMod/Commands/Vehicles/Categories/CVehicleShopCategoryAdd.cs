using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using SDG.Unturned;
using ShopsUI.API.Shops.Vehicles;
using ShopsUI.Commands.Base.Categories;
using System;

namespace ShopsUI.Commands.Vehicles.Categories
{
    [Command("add", Priority = Priority.High)]
    [CommandAlias("a")]
    [CommandAlias("+")]
    [CommandSyntax("<category> <shops>")]
    [CommandDescription("Add a shop to a category.")]
    [CommandParent(typeof(CVehicleShopCategory))]
    public class CVehicleShopCategoryAdd : ShopCategoryAddCommand<IVehicleCategoryEditor>
    {
        public CVehicleShopCategoryAdd(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override bool CheckValidId(ushort id)
        {
            return Assets.find(EAssetType.VEHICLE, id) != null;
        }

        protected override string GetSuccessLocalizationKey()
        {
            return "commands:success:shop_category:added:vehicle";
        }
    }
}
