using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using ShopsUI.Commands.Base.Categories;
using System;

namespace ShopsUI.Commands.Items.Categories
{
    [Command("category", Priority = Priority.High)]
    [CommandAlias("c")]
    [CommandSyntax("<add | remove | create | delete>")]
    [CommandDescription("Manage shop categories.")]
    [CommandParent(typeof(CShop))]
    public class CItemShopCategory : ShopCategoryCommand
    {
        public CItemShopCategory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
