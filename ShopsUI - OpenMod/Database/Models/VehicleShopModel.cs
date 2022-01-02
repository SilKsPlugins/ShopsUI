using ShopsUI.API.Shops.Vehicles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopsUI.Database.Models
{
    [Serializable]
    public class VehicleShopModel : IVehicleShopData
    {
        [Key]
        public ushort Id { get; set; }

        [Column(TypeName = "decimal(24,2)")]
        public decimal BuyPrice { get; set; }
        
        public int Order { get; set; }

        public virtual ICollection<VehicleGroupModel> AuthGroups { get; set; } = new List<VehicleGroupModel>();

        public ICollection<VehicleShopCategoryModel>? Categories { get; set; }

        public VehicleShopModel()
        {
            Id = 0;
            BuyPrice = 0;
            Order = 0;
        }
    }
}
