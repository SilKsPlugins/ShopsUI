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
        public ushort ItemId { get; set; }

        public decimal? BuyPrice { get; set; }

        public decimal? SellPrice { get; set; }
        
        public int Order { get; set; }

        public ItemShopModel()
        {
            ItemId = 0;
            BuyPrice = null;
            SellPrice = null;
            Order = 0;
        }
    }
}
