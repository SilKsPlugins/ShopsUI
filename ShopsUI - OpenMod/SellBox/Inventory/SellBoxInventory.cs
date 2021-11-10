using OpenMod.Extensions.Games.Abstractions.Items;
using SDG.Unturned;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ShopsUI.SellBox.Inventory
{
    public class SellBoxInventory : IInventory
    {
        public InteractableStorage Storage { get; }

        public SellBoxInventory(InteractableStorage storage)
        {
            Storage = storage;
        }

        public IReadOnlyCollection<IInventoryPage> Pages => new[] {new SellBoxInventoryPage(this)};

        public bool IsFull => throw new NotImplementedException();

        public int Count => Pages.Count;

        public IEnumerator<IInventoryPage> GetEnumerator()
        {
            return Pages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
