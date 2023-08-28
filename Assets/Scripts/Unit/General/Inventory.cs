using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements an inventory for a player.
*
* @author: Colin Keys
*/
public class Inventory {

    public int space { get; } = 6;
    public Dictionary<int, Item> myItems { get; } = new Dictionary<int, Item>();

    /*
    *   AddItem - Adds an item to the champions inventory and calls the inventory changed handler.
    *   @param item - Item object of the item wanting to be added to the inventory.
    */
    public int AddItem(Item item){
        // If inventory isn't full
        if(myItems.Count < space){
            int itemSlot;
            // Iterate over the inventory and find the open slot.
            for(int i = 1; i < 7; i++){
                // Add item to the open slot and adjust stats.
                if(!myItems.ContainsKey(i)){
                    itemSlot = i;
                    myItems.Add(itemSlot, item);
                    return itemSlot;
                }
            } 
        }
        else{
            Debug.Log("Inventory Full");
        }
        return -1;
    }

    /*
    *   RemoveItem - Removes an item from the champions inventory and calls the inventory changed handler.
    *   @param slot - int of the item slot in the inventory to remove an item from.
    */
    public Item RemoveItem(int slot){
        Item removedItem = null;
        // If that inventory slot has an item remove it and adjust stats.
        if(myItems.ContainsKey(slot)){
            removedItem = myItems[slot];
            myItems.Remove(slot);
        }
        else{
            Debug.Log("No Item to sell in selected slot");
        }
        return removedItem;
    }
}
