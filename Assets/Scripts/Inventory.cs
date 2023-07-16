using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

    private int space = 6;
    private Dictionary<int, Item> myItems = new Dictionary<int, Item>();
    private ChampionStats championStats;
    private UIManager uiManager;
    private Player player;

    public Inventory(Player player){
        this.player = player;
        this.championStats = (ChampionStats) player.unitStats;
        //this.uiManager = player.uiManager;
    }

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
                    AddItemStats(item); 
                    UIManager.instance.AddItem(itemSlot, item.icon, player.playerUI);
                    championStats.UpdateAttackSpeed();
                    UIManager.instance.UpdateAllStats(championStats, player.playerUI);
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
            RemoveItemStats(removedItem);
            UIManager.instance.RemoveItem(slot, player.playerUI);
            championStats.UpdateAttackSpeed();
            UIManager.instance.UpdateAllStats(championStats, player.playerUI);
        }
        else{
            Debug.Log("No Item to sell in selected slot");
        }
        return removedItem;
    }

    /*
    *   AddItemStats - Add an items stats to the champions stats.
    *   @param item - Item to that was added to the inventory.
    */
    private void AddItemStats(Item item){
        if(item != null){
            // Bought an item so add the items stats.
            championStats.magicDamage.AddModifier(item.magicDamage);
            championStats.physicalDamage.AddModifier(item.physicalDamage);
            // Increase current health by items health amount.
            if(item.health != 0){
                championStats.maxHealth.AddModifier(item.health);
                if(!player.isDead){
                    championStats.SetHealth(championStats.currentHealth + item.health);
                    UIManager.instance.UpdateHealthBar(player, player.playerUI, player.playerBar);
                }
            }
            // Increase current mana by items mana amount.
            if(item.mana != 0){
                championStats.maxMana.AddModifier(item.mana);
                if(!player.isDead){
                    championStats.SetMana(championStats.currentMana + item.mana);
                    UIManager.instance.UpdateManaBar(championStats, player.playerUI, player.playerBar);
                }
            }
            championStats.speed.AddModifier(item.speed);
            championStats.bonusAttackSpeed.AddModifier(item.attackSpeed);
        }
    }

    /*
    *   RemoveItemStats - Remove an items stats from the champions stats.
    *   @param item - Item to that was added to the inventory.
    */
    private void RemoveItemStats(Item item){
        if(item != null){
            // Selling an item so remove the items stats.
            championStats.magicDamage.RemoveModifier(item.magicDamage);
            championStats.physicalDamage.RemoveModifier(item.physicalDamage);
            //If the items hp hasn't been used yet, remove the amount that hasn't been used from current health.
            if(item.health != 0){
                if(championStats.currentHealth > (championStats.maxHealth.GetValue() - item.health))
                    championStats.SetHealth(championStats.currentHealth - (item.health - (championStats.maxHealth.GetValue() - championStats.currentHealth))); 
                championStats.maxHealth.RemoveModifier(item.health);
                UIManager.instance.UpdateHealthBar(player, player.playerUI, player.playerBar);
            }
            //If the items mana hasn't been used yet, remove the amount that hasn't been used from the current mana.
            if(item.mana != 0){
                if(championStats.currentMana > (championStats.maxMana.GetValue() - item.mana))
                    championStats.SetMana(championStats.currentMana - (item.mana - (championStats.maxMana.GetValue() - championStats.currentMana)));
                championStats.maxMana.RemoveModifier(item.mana);
                UIManager.instance.UpdateManaBar(championStats, player.playerUI, player.playerBar);
            }
            championStats.speed.RemoveModifier(item.speed);
            championStats.bonusAttackSpeed.RemoveModifier(item.attackSpeed);
        }
    }
}
