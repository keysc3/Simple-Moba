using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/*
* Purpose: Tests for the Inventory class.
*
* @author: Colin Keys
*/
public class inventory
{
    /*
    *   Adds an item to the inventory. Inventory should have the added item.
    */
    [Test]
    public void adds_item_to_inventory(){
        // Arrange
        Inventory inventory = new Inventory();
        Item item = ScriptableObject.CreateInstance<Item>();

        // Act
        int itemSlot = inventory.AddItem(item);

        // Assert
        Assert.AreEqual(item, inventory.myItems[itemSlot]);
    }

    /*
    *   Removes an item from inventory slot 1. The inventory should have zero items.
    */
    [Test]
    public void removes_item_from_inventory(){
        // Arrange
        Inventory inventory = new Inventory();
        Item item = ScriptableObject.CreateInstance<Item>();
        int itemSlot = inventory.AddItem(item);

        // Act
        Item removedItem = inventory.RemoveItem(itemSlot);

        // Assert
        Assert.AreEqual(0, inventory.myItems.Count);
    }

    /*
    *   Add more items than the inventory can hold. The inventory should return -1 item slot.
    */
    [Test]
    public void adds_more_items_than_max_size(){
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

    /*
    *   Removes an item from inventory slot that doesn't have an item. The inventory shouldn't return an item.
    */
    [Test]
    public void removes_item_from_slot_without_item(){
        // Arrange
        Inventory inventory = new Inventory();
        inventory.AddItem(ScriptableObject.CreateInstance<Item>());

        // Act
        Item removedItem = inventory.RemoveItem(4);

        // Assert
        Assert.AreEqual(null, removedItem);
    }

    /*
    *   Adds an item to the lowest open inventory slot. Should return item slot 2.
    */
    [Test]
    public void adds_item_to_lowest_open_slot_number(){
        // Arrange
        Inventory inventory = new Inventory();
        for(int i = 0; i < inventory.space; i++){
            inventory.AddItem(ScriptableObject.CreateInstance<Item>());
        }
        inventory.RemoveItem(5);
        inventory.RemoveItem(2);

        // Act
        int itemSlot = inventory.AddItem(ScriptableObject.CreateInstance<Item>());

        // Assert
        Assert.AreEqual(2, itemSlot);
    }
}
