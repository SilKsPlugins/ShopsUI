﻿using OpenMod.Extensions.Games.Abstractions.Items;
using SDG.Unturned;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ShopsUI.SellBox.Inventory
{
    public class SellBoxInventoryPage : IInventoryPage
    {
        public SellBoxInventory Inventory { get; set; }

        IInventory IInventoryPage.Inventory => Inventory;

        public List<ItemJar> InventoryItems => Inventory.Storage.items.items;


        public SellBoxInventoryPage(SellBoxInventory inventory)
        {
            Inventory = inventory;
        }

        public string Name => "Sell Box Storage";

        public int Count => Items.Count;

        public bool IsReadOnly => false;

        public IReadOnlyCollection<IInventoryItem> Items =>
            InventoryItems.Select(itemJar => new SellBoxInventoryItem(Inventory, itemJar)).ToList();

        public IEnumerator<IInventoryItem> GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Capacity => throw new NotImplementedException();

        public bool IsFull => throw new NotImplementedException();
    }
}
