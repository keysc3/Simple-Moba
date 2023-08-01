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
    public Spell spell { get; private set; }
    public KeyCode keyCode { get; private set; }
    public PlayerSpellInput playerSpellInput { get; private set; }

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

    /*
    *   SetSpell - Sets the spell this button is for.
    *   @param spell - Spell to set to.
    */
    public void SetSpell(Spell spell){
        this.spell = spell;
    }

    /*
    *   SetKeyCode - Sets the KeyCode the spell this button is for uses.
    *   @param keyCode - KeyCode to set to.
    */
    public void SetKeyCode(KeyCode keyCode){
        this.keyCode = keyCode;
    }

    /*
    *   SetPlayerSpellInput - Sets the PlayerSpellInput of this buttons associated GameObject.
    *   @param playerSpellInput - PlayerSpellInput to set to.
    */
    public void SetPlayerSpellInput(PlayerSpellInput playerSpellInput){
        this.playerSpellInput = playerSpellInput;
    }
}
