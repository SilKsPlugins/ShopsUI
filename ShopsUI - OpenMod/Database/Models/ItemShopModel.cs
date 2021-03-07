using ShopsUI.API.Items;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopsUI.Database.Models
{
    [Serializable]
    public class ItemShopModel : IItemShopData
    {
        [Key]
        public int ItemShopId { get; set; }

        [NotMapped]
        ushort IItemShopData.ItemId => (ushort) ItemShopId;

        public decimal? BuyPrice { get; set; }

        public decimal? SellPrice { get; set; }
        
        public int Order { get; set; }

        public ItemShopModel()
        {
            ItemShopId = 0;
            BuyPrice = null;
            SellPrice = null;
            Order = 0;
        }
    }
}
