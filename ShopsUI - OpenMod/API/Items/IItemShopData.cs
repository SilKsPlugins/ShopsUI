using System.ComponentModel.DataAnnotations.Schema;

namespace ShopsUI.API.Items
{
    public interface IItemShopData
    {
        public ushort ItemId { get; }

        public decimal? BuyPrice { get; }

        public decimal? SellPrice { get; }
    }
}
