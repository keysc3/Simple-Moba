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

        // Act
        int itemSlot = inventory.AddItem(item);
        Item removedItem = inventory.RemoveItem(itemSlot);

        // Assert
        Assert.AreEqual(item, removedItem);
    }
}
