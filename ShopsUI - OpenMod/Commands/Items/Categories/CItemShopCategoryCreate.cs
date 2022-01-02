using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.API.Shops.Items;
using ShopsUI.Commands.Base.Categories;
using System;

namespace ShopsUI.Commands.Items.Categories
{
    [Command("create", Priority = Priority.High)]
    [CommandAlias("c")]
    [CommandSyntax("<category name>")]
    [CommandDescription("Create a shop category.")]
    [CommandParent(typeof(CItemShopCategory))]
    public class CItemShopCategoryCreate : ShopCategoryCreateCommand<IItemCategoryEditor>
    {
        public CItemShopCategoryCreate(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override string GetSuccessLocalizationKey()
        {
            return "commands:success:shop_category:created:item";
        }
    }
}
