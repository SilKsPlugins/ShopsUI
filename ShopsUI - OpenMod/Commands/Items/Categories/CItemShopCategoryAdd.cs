using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using SDG.Unturned;
using ShopsUI.API.Shops;
using ShopsUI.Commands.Base.Categories;
using System;
using ShopsUI.API.Shops.Items;

namespace ShopsUI.Commands.Items.Categories
{
    [Command("add", Priority = Priority.High)]
    [CommandAlias("a")]
    [CommandAlias("+")]
    [CommandSyntax("<category> <shops>")]
    [CommandDescription("Add a shop to a category.")]
    [CommandParent(typeof(CItemShopCategory))]
    public class CItemShopCategoryAdd : ShopCategoryAddCommand<IItemCategoryEditor>
    {
        public CItemShopCategoryAdd(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override bool CheckValidId(ushort id)
        {
            return Assets.find(EAssetType.ITEM, id) != null;
        }

        protected override string GetSuccessLocalizationKey()
        {
            return "commands:success:shop_category:added:item";
        }
    }
}
