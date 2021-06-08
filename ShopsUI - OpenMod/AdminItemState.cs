using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Unturned.Items;

namespace ShopsUI
{
    public class AdminItemState : IItemState
    {
        public AdminItemState(UnturnedItemAsset itemAsset)
        {
            ItemQuality = itemAsset.MaxQuality ?? 100;
            ItemDurability = itemAsset.MaxDurability ?? 100;
            ItemAmount = itemAsset.MaxAmount ?? 1;
        }

        public double ItemQuality { get; }

        public double ItemDurability { get; }

        public double ItemAmount { get; }

        public byte[]? StateData { get; }
    }
}
