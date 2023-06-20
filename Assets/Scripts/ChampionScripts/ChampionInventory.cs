using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements an inventory.
*
* @author: Colin Keys
*/
public class ChampionInventory : MonoBehaviour
{
    private int space = 6;
    private Dictionary<int, Item> myItems = new Dictionary<int, Item>();
    private List<string> displayItemsInsp = new List<string>();
    private UIManager uiManager;
    private ChampionStats championStats;

    //public static ChampionInventory instance;

    /*public delegate void InventoryChanged(Item item, string changeType);
    public static event InventoryChanged inventoryChangedCallback;*/
    
    // Called when the script instance is being loaded.
    private void Awake(){
        uiManager = GetComponent<UIManager>();
        championStats = GetComponent<ChampionStats>();
        /*if(instance != null)
            Debug.LogWarning("Inventory instance already active");
        else
            instance = this;*/
    
    }

    /*
    *   AddItem - Adds an item to the champions inventory and calls the inventory changed handler.
    *   @param item - Item object of the item wanting to be added to the inventory.
    */
    public void AddItem(Item item){
        // If inventory isn't full
        if(myItems.Count < space){
            int itemSlot = 1;
            // Iterate over the inventory and find the open slot.
            for(int i = 1; i < 7; i++){
                // Add item to the open slot and adjust stats.
                if(!myItems.ContainsKey(i)){
                    itemSlot = i;
                    myItems.Add(itemSlot, item);
                    championStats.InventoryChanged(item, "Buy"); 
                    uiManager.AddItem(itemSlot, item.icon);
                    championStats.UpdateAttackSpeed();
                    uiManager.UpdateAllStats();
                    DisplayItems();
                    return;
                }
            } 
        }
        else{
            Debug.Log("Inventory Full");
        }
    }

    /*
    *   RemoveItem - Removes an item from the champions inventory and calls the inventory changed handler.
    *   @param slot - int of the item slot in the inventory to remove an item from.
    */
    public void RemoveItem(int slot){
        // If that inventory slot has an item remove it and adjust stats.
        if(myItems.ContainsKey(slot)){
            Item item = myItems[slot];
            myItems.Remove(slot);
            championStats.InventoryChanged(item, "Sell");
            uiManager.RemoveItem(slot);
            championStats.UpdateAttackSpeed();
            uiManager.UpdateAllStats();
            DisplayItems();
        }
        else{
            Debug.Log("No Item to sell in selected slot");
        }
    }

    /*
    *  DisplayItems - Display the items in the inspector by adding dictionary values to a list.
    */
    private void DisplayItems(){
        displayItemsInsp.Clear();
        for(int i = 1; i < 7; i++){
            if(myItems.ContainsKey(i))
                displayItemsInsp.Add(myItems[i].name);
            else
                displayItemsInsp.Add("No Item");
        }
    }
}
