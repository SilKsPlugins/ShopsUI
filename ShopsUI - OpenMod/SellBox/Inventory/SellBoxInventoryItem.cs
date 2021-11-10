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
        public SellBoxInventoryItem(SellBoxInventory inventory, ItemJar itemJar)
        {
            Inventory = inventory;
            ItemJar = itemJar;
            Item = new UnturnedItem(itemJar.item, DestroyAsync);
        }

        public SellBoxInventory Inventory { get; }

        public ItemJar ItemJar { get; }

        IItem IItemInstance.Item => Item;

        public UnturnedItem Item { get; }

        public Task<bool> DestroyAsync()
        {
            async UniTask<bool> DestroyTask()
            {
                await UniTask.SwitchToMainThread();

                var items = Inventory.Storage.items;

                var index = items.items.IndexOf(ItemJar);

                if (index < 0)
                {
                    return false;
                }

                items.removeItem((byte)index);

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