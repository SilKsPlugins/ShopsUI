using ShopsUI.API.Shops;
using ShopsUI.API.Shops.Items;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ShopsUI.Database.Models
{
    public class ItemCategoryModel : IShopCategory<IItemShopData>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = "";
        
        public ICollection<ItemShopCategoryModel>? ItemShops { get; set; }

        [NotMapped]
        public IReadOnlyCollection<IItemShopData>? Shops => ItemShops?.Select(x => x.ItemShop).ToList();
    }
}
