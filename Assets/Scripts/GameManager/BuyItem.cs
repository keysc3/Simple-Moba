using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        IPlayer player = ActiveChampion.instance.players[ActiveChampion.instance.ActiveChamp];
        Item addItem = allItems[itemNumber-1];
        int itemSlot = player.inventory.AddItem(addItem);
        if(itemSlot != -1){
            ((ChampionStats) player.unitStats).AddItemStats(addItem);
            /*if(!player.isDead){
                UIManager.instance.UpdateHealthBar(player);
                UIManager.instance.UpdateManaBar(player);
            }*/
            UIAddItem(itemSlot, addItem.icon, player.playerUI);
            //UIManager.instance.UpdateAllStats(player);
        }
    }

    /*
    *   SellItem - Sells an item for the champion.
    *   @param itemNumber - int of the index in allItems for the item to sell.
    */
    private void SellItem(int itemSlot){
        IPlayer player = ActiveChampion.instance.players[ActiveChampion.instance.ActiveChamp];
        Item removeItem = player.inventory.RemoveItem(itemSlot);
        if(removeItem != null){
            ((ChampionStats) player.unitStats).RemoveItemStats(removeItem);
            //UIManager.instance.UpdateHealthBar(player);
            //UIManager.instance.UpdateManaBar(player);
            UIRemoveItem(itemSlot, player.playerUI);
            //UIManager.instance.UpdateAllStats(player);
        }
    }

    /*
    *   UIAddItem - Updates the inventory UI with a new item.
    *   @param itemSlot - int of the UI item slot to add the item to.
    *   @param itemSprite - sprite of the item.
    *   @param playerUI - GameObject of the playerUI being updated.
    */
    private void UIAddItem(int itemSlot, Sprite itemSprite, GameObject playerUI){
        if(playerUI != null){
            GameObject itemImage = playerUI.transform.Find("Player/Items/ItemsContainer/Item_" + itemSlot + "_Container/Sprite").gameObject;
            itemImage.GetComponent<Image>().sprite = itemSprite;
            itemImage.SetActive(true);
        }
    }

    /*
    *   UIAddItem - Removes an item from the inventory UI.
    *   @param itemSlot - int of the UI item slot to remove the item from.
    *   @param playerUI - GameObject of the playerUI being updated.
    */
    private void UIRemoveItem(int itemSlot, GameObject playerUI){
        if(playerUI != null){
            GameObject itemImage = playerUI.transform.Find("Player/Items/ItemsContainer/Item_" + itemSlot + "_Container/Sprite").gameObject;
            itemImage.GetComponent<Image>().sprite = null;
            itemImage.SetActive(false);
        }
    }
}
