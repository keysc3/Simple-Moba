using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
* Purpose: Handles a spell buttons actions.
*
* @author: Colin Keys
*/
public class SpellButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    //TODO? could change to properties with validation on spell and keycode based on constant values.
    public Spell spell;
    public KeyCode keyCode;
    public PlayerSpellInput playerSpellInput;

    /*
    *   OnPointerEnter - Called when the cursor enters the rect area of this button.
    */
    public void OnPointerEnter(PointerEventData eventData){
        if(DrawGizmos.instance.drawMethod == null)
            spell.DisplayCast();
    }

    /*
    *   OnPointerExit - Called when the cursor exits the rect area of this button.
    */
    public void OnPointerExit(PointerEventData eventData){
        if(playerSpellInput.lastSpellPressed != spell && spell.isDisplayed)
            spell.HideCast();
    }

    /*
    *   OnPointerDown - Called when the mouse is clicked over the button.
    */
    public void OnPointerDown(PointerEventData eventData){
        if(spell.spellNum != "Passive"){
            playerSpellInput.buttonClick = true;
            playerSpellInput.SpellButtonPressed(keyCode, spell);
        }
    }
}
