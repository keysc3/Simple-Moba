using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Handles input for item buying and selling.
*
* @author: Colin Keys
*/
public class BuyItem : MonoBehaviour
{
    [SerializeField] private Item[] allItems;

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            if(Input.GetKey(KeyCode.LeftShift))
                SellItem(1);
            else
                PurchaseItem(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            if(Input.GetKey(KeyCode.LeftShift))
                SellItem(2);
            else
            PurchaseItem(2);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            if(Input.GetKey(KeyCode.LeftShift))
                SellItem(3);
            else
            PurchaseItem(3);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)){
            if(Input.GetKey(KeyCode.LeftShift))
                SellItem(4);
            else
             PurchaseItem(4);
        }
        if(Input.GetKeyDown(KeyCode.Alpha5)){
            if(Input.GetKey(KeyCode.LeftShift))
                SellItem(5);
            else
                PurchaseItem(5);
        }
        if(Input.GetKeyDown(KeyCode.Alpha6)){
            if(Input.GetKey(KeyCode.LeftShift))
                SellItem(6);
            else
                PurchaseItem(6);
        }
    }

    /*
    *   PurchaseItem - Purchases an item for the champion.
    *   @param itemNumber - int of the index in allItems for the item to purchase.
    */
    private void PurchaseItem(int itemNumber){
        Player player = ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion].GetComponent<Player>();
        Item addItem = allItems[itemNumber-1];
        int itemSlot = player.inventory.AddItem(addItem);
        if(itemSlot != -1){
            ChampionStats championStats = (ChampionStats) player.unitStats;
            championStats.AddItemStats(addItem);
            if(!player.isDead){
                UIManager.instance.UpdateHealthBar(player, player.playerUI, player.playerBar);
                UIManager.instance.UpdateManaBar(championStats, player.playerUI, player.playerBar);
            }
            UIManager.instance.AddItem(itemSlot, addItem.icon, player.playerUI);
            UIManager.instance.UpdateAllStats(championStats, player.playerUI);
        }
    }

    /*
    *   SellItem - Sells an item for the champion.
    *   @param itemNumber - int of the index in allItems for the item to sell.
    */
    private void SellItem(int itemSlot){
        Player player = ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion].GetComponent<Player>();
        Item removeItem = player.inventory.RemoveItem(itemSlot);
        if(removeItem != null){
            ChampionStats championStats = (ChampionStats) player.unitStats;
            championStats.RemoveItemStats(removeItem);
            UIManager.instance.UpdateHealthBar(player, player.playerUI, player.playerBar);
            UIManager.instance.UpdateManaBar(championStats, player.playerUI, player.playerBar);
            UIManager.instance.RemoveItem(itemSlot, player.playerUI);
            UIManager.instance.UpdateAllStats(championStats, player.playerUI);
        }
    }
}
