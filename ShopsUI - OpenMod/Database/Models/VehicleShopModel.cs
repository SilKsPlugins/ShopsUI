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
        public ushort VehicleId { get; set; }

        
        public decimal BuyPrice { get; set; }
        
        public int Order { get; set; }

        public VehicleShopModel()
        {
            VehicleId = 0;
            BuyPrice = 0;
            Order = 0;
        }
    }
}
