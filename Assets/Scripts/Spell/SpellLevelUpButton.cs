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
    public Player player { get; private set; }
    public string spell { get; private set; }
    public PlayerSpellInput playerSpellInput { get; private set; }

    /*
    *   OnPointerDown - Called when the mouse is clicked over the button.
    */
    public void OnPointerDown(PointerEventData eventData){
        playerSpellInput.SetButtonClick(true);
        player.levelManager.SpellLevelUp(spell);
    }

    /*
    *   SetPlayer - Sets the player this button is for.
    *   @param player - Player to set to.
    */
    public void SetPlayer(Player player){
        this.player = player;
    }

    /*
    *   SetSpell - Sets the spell number this button is for.
    *   @param spell - string to set to.
    */
    public void SetSpell(string spell){
        this.spell = spell;
    }

    /*
    *   SetPlayerSpellInput - Sets the PlayerSpellInput of this buttons associated GameObject.
    *   @param playerSpellInput - PlayerSpellInput to set to.
    */
    public void SetPlayerSpellInput(PlayerSpellInput playerSpellInput){
        this.playerSpellInput = playerSpellInput;
    }
}
