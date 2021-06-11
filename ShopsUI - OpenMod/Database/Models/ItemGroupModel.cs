using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopsUI.Database.Models
{
    [Table("ItemGroup")]
    public class ItemGroupModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Permission { get; set; }

        public bool IsWhitelist { get; set; }

        public virtual ItemShopModel ItemShop { get; }

        public ItemGroupModel()
        {
            Id = 0;
            Permission = "";
            IsWhitelist = true;
            ItemShop = null!;
        }
    }
}
