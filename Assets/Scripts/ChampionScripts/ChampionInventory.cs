using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Manages a champions inventory and updates necessary systems.
*
* @author: Colin Keys
*/
public class ChampionInventory : MonoBehaviour
{

    //private List<string> displayItemsInsp = new List<string>();
    private UIManager uiManager;
    private ChampionStats championStats;
    public Inventory inventory { get; private set; }
    
    // Called when the script instance is being loaded.
    private void Awake(){
        //inventory = new Inventory();
        uiManager = GetComponent<UIManager>();
        championStats = GetComponent<ChampionStats>();
    }

    /*public void BoughtItem(Item item){
        int inventorySlot= inventory.AddItem(item);
        if(inventorySlot > -1){
            AddItemStats(item); 
            uiManager.AddItem(inventorySlot, item.icon);
            championStats.UpdateAttackSpeed();
            uiManager.UpdateAllStats();
            //DisplayItems();
        }
    }*/

    /*public void SoldItem(int inventorySlot){
        Item itemSold = inventory.RemoveItem(inventorySlot);
        if(itemSold != null){
            RemoveItemStats(itemSold);
            uiManager.RemoveItem(inventorySlot);
            championStats.UpdateAttackSpeed();
            uiManager.UpdateAllStats();
            //DisplayItems();
        }
    }*/

    /*
    *  DisplayItems - Display the items in the inspector by adding dictionary values to a list.
    */
    /*private void DisplayItems(){
        displayItemsInsp.Clear();
        for(int i = 1; i < 7; i++){
            if(myItems.ContainsKey(i))
                displayItemsInsp.Add(myItems[i].name);
            else
                displayItemsInsp.Add("No Item");
        }
    }*/

    /*
    *   AddItemStats - Add an items stats to the champions stats.
    *   @param item - Item to that was added to the inventory.
    */
    /*private void AddItemStats(Item item){
        if(item != null){
            // Bought an item so add the items stats.
            championStats.magicDamage.AddModifier(item.magicDamage);
            championStats.physicalDamage.AddModifier(item.physicalDamage);
            // Increase current health by items health amount.
            if(item.health != 0){
                championStats.maxHealth.AddModifier(item.health);
                if(!championStats.isDead){
                    championStats.SetHealth(championStats.currentHealth + item.health);
                    GetComponent<UIManager>().UpdateHealthBar();
                }
            }
            // Increase current mana by items mana amount.
            if(item.mana != 0){
                championStats.maxMana.AddModifier(item.mana);
                if(!championStats.isDead){
                    championStats.SetMana(championStats.currentMana + item.mana);
                    GetComponent<UIManager>().UpdateManaBar();
                }
            }
            championStats.speed.AddModifier(item.speed);
            championStats.bonusAttackSpeed.AddModifier(item.attackSpeed);
        }
    }*/

    /*
    *   RemoveItemStats - Remove an items stats from the champions stats.
    *   @param item - Item to that was added to the inventory.
    */
    /*private void RemoveItemStats(Item item){
        if(item != null){
            // Selling an item so remove the items stats.
            championStats.magicDamage.RemoveModifier(item.magicDamage);
            championStats.physicalDamage.RemoveModifier(item.physicalDamage);
            //If the items hp hasn't been used yet, remove the amount that hasn't been used from current health.
            if(item.health != 0){
                if(championStats.currentHealth > (championStats.maxHealth.GetValue() - item.health))
                    championStats.SetHealth(championStats.currentHealth - (item.health - (championStats.maxHealth.GetValue() - championStats.currentHealth))); 
                championStats.maxHealth.RemoveModifier(item.health);
                GetComponent<UIManager>().UpdateHealthBar();
            }
            //If the items mana hasn't been used yet, remove the amount that hasn't been used from the current mana.
            if(item.mana != 0){
                if(championStats.currentMana > (championStats.maxMana.GetValue() - item.mana))
                    championStats.SetMana(championStats.currentMana - (item.mana - (championStats.maxMana.GetValue() - championStats.currentMana)));
                championStats.maxMana.RemoveModifier(item.mana);
                GetComponent<UIManager>().UpdateManaBar();
            }
            championStats.speed.RemoveModifier(item.speed);
            championStats.bonusAttackSpeed.RemoveModifier(item.attackSpeed);
        }
    }*/
}
