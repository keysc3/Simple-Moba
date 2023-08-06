using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
* Purpose: Handles a spell level up buttons actions.
*
* @author: Colin Keys
*/
public class SpellLevelUpButton : MonoBehaviour, IPointerDownHandler
{
    public Player player;
    public Player Player {
        get => player;
        set {
            if(value.playerSpellInput != null)
                player = value; 
        }
    }
    public string spell { get; set; }

    /*
    *   OnPointerDown - Called when the mouse is clicked over the button.
    */
    public void OnPointerDown(PointerEventData eventData){
        player.playerSpellInput.buttonClick = true;
        player.levelManager.SpellLevelUp(spell);
    }
}
