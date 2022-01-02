using Cysharp.Threading.Tasks;
using OpenMod.Extensions.Games.Abstractions.Items;
using OpenMod.Unturned.Items;
using SDG.Unturned;
using System;
using System.Threading.Tasks;

namespace ShopsUI.SellBox.Inventory
{
    public class SellBoxInventoryItem : IInventoryItem
    {
        public SellBoxInventoryItem(SellBoxInventory inventory, Item item)
        {
            Inventory = inventory;
            NativeItem = item;
            Item = new UnturnedItem(item, DestroyAsync);
        }

        public SellBoxInventory Inventory { get; }

        public Item NativeItem { get; }

        IItem IItemInstance.Item => Item;

        public UnturnedItem Item { get; }

        public Task<bool> DestroyAsync()
        {
            async UniTask<bool> DestroyTask()
            {
                await UniTask.SwitchToMainThread();

                var items = Inventory.Items;

                var index = items.IndexOf(NativeItem);

                if (index < 0)
                {
                    return false;
                }

                items.RemoveAt(index);

                return true;
            }

            return DestroyTask().AsTask();
        }

        public Task DropAsync()
        {
            throw new NotImplementedException();
        }
    }
}