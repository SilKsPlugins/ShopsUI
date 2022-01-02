using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops;
using ShopsUI.API.Shops.Items;
using ShopsUI.Commands.Base.Categories;
using System;
using System.Threading.Tasks;

namespace ShopsUI.Commands.Items.Categories
{
    [Command("delete", Priority = Priority.High)]
    [CommandAlias("d")]
    [CommandSyntax("<category name> [-f|--force]")]
    [CommandDescription("Deletes the specified category (use -f to not have to confirm).")]
    [CommandParent(typeof(CItemShopCategory))]
    public class CItemShopCategoryDelete : ShopCategoryDeleteCommand<IItemCategoryEditor, IItemShopData>
    {
        public CItemShopCategoryDelete(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override async Task<IShopCategory<IItemShopData>?> GetCategory(string name)
        {
            return await ItemShopDirectory.GetCategory(name);
        }

        protected override string GetSuccessLocalizationKey()
        {
            return "commands:success:shop_category:deleted:item";
        }

        protected override string GetConfirmationLocalizationKey()
        {
            return "commands:success:shop_category:deleted:confirm:item";
        }
    }
}
