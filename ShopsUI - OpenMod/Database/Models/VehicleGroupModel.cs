using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopsUI.Database.Models
{
    [Table("VehicleGroup")]
    public class VehicleGroupModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Permission { get; set; }

        public bool IsWhitelist { get; set; }

        public virtual VehicleShopModel VehicleShop { get; }

        public VehicleGroupModel()
        {
            Id = 0;
            Permission = "";
            IsWhitelist = true;
            VehicleShop = null!;
        }
    }
}
