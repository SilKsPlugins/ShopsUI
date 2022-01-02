using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Items;
using ShopsUI.Commands.Base.Categories;
using System;

namespace ShopsUI.Commands.Items.Categories
{
    [Command("remove", Priority = Priority.High)]
    [CommandAlias("rem")]
    [CommandAlias("r")]
    [CommandAlias("-")]
    [CommandSyntax("<category name> <shops>")]
    [CommandDescription("Removes the specified item shops from the category.")]
    [CommandParent(typeof(CItemShopCategory))]
    public class CItemShopCategoryRemove : ShopCategoryRemoveCommand<IItemCategoryEditor>
    {
        public CItemShopCategoryRemove(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override string GetSuccessLocalizationKey()
        {
            return "commands:success:shop_category:removed:item";
        }
    }
}