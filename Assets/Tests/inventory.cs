using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class inventory
{
    // A Test behaves as an ordinary method
    [Test]
    public void adds_item_to_slot_1()
    {
        // Arrange
        Inventory inventory = new Inventory();
        Item item = ScriptableObject.CreateInstance<Item>();

        // Act
        int itemSlot = inventory.AddItem(item);

        // Assert
        Assert.AreEqual(1, itemSlot);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void remove_item_from_inventory()
    {
        // Arrange
        Inventory inventory = new Inventory();
        Item item = ScriptableObject.CreateInstance<Item>();
        int itemSlot = inventory.AddItem(item);

        // Act
        Item removedItem = inventory.RemoveItem(itemSlot);

        // Assert
        Assert.AreEqual(item, removedItem);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void add_more_items_than_max_inventory_size()
    {
        // Arrange
        Inventory inventory = new Inventory();
        for(int i = 0; i < inventory.space; i++){
            inventory.AddItem(ScriptableObject.CreateInstance<Item>());
        }

        // Act
        int itemSlot = inventory.AddItem(ScriptableObject.CreateInstance<Item>());

        // Assert
        Assert.AreEqual(-1, itemSlot);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void remove_item_from_inventory_slot_without_item()
    {
        // Arrange
        Inventory inventory = new Inventory();
        inventory.AddItem(ScriptableObject.CreateInstance<Item>());

        // Act
        Item removedItem = inventory.RemoveItem(4);

        // Assert
        Assert.AreEqual(null, removedItem);
    }
}
