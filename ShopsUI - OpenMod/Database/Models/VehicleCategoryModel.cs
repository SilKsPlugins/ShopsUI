using ShopsUI.API.Shops;
using ShopsUI.API.Shops.Vehicles;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ShopsUI.Database.Models
{
    public class VehicleCategoryModel : IShopCategory<IVehicleShopData>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = "";
        
        public ICollection<VehicleShopCategoryModel>? VehicleShops { get; set; }

        [NotMapped]
        public IReadOnlyCollection<IVehicleShopData>? Shops => VehicleShops?.Select(x => x.VehicleShop).ToList();
    }
}
