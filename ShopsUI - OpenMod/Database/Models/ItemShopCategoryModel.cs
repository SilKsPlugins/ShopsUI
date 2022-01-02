using System;

namespace ShopsUI.Database.Models
{
    [Serializable]
    public class ItemShopCategoryModel
    {
        public ushort ItemShopId { get; set; }

        public ItemShopModel ItemShop { get; set; } = null!;

        public int ItemCategoryId { get; set; }

        public ItemCategoryModel ItemCategory { get; set; } = null!;
    }
}
