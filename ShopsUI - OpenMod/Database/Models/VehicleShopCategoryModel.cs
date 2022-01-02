using System;

namespace ShopsUI.Database.Models
{
    [Serializable]
    public class VehicleShopCategoryModel
    {
        public ushort VehicleShopId { get; set; }

        public VehicleShopModel VehicleShop { get; set; } = null!;

        public int VehicleCategoryId { get; set; }

        public VehicleCategoryModel VehicleCategory { get; set; } = null!;
    }
}
