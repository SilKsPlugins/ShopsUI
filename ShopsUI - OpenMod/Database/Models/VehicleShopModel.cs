using ShopsUI.API.Vehicles;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopsUI.Database.Models
{
    [Serializable]
    public class VehicleShopModel : IVehicleShopData
    {
        [Key]
        public int VehicleShopId { get; set; }

        [NotMapped]
        ushort IVehicleShopData.VehicleId => (ushort) VehicleShopId;
        
        public decimal BuyPrice { get; set; }

        public VehicleShopModel()
        {
            VehicleShopId = 0;
            BuyPrice = 0;
        }
    }
}
