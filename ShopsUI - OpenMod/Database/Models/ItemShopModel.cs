using ShopsUI.API.Shops.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopsUI.Database.Models
{
    [Serializable]
    public class ItemShopModel : IItemShopData
    {
        [Key]
        public ushort Id { get; set; }

        [Column(TypeName = "decimal(24,2)")]
        public decimal? BuyPrice { get; set; }

        [Column(TypeName = "decimal(24,2)")]
        public decimal? SellPrice { get; set; }
        
        public int Order { get; set; }

        public ICollection<ItemGroupModel>? AuthGroups { get; set; }

        public ICollection<ItemShopCategoryModel>? Categories { get; set; }

        public ItemShopModel()
        {
            Id = 0;
            BuyPrice = null;
            SellPrice = null;
            Order = 0;
        }
    }
}
