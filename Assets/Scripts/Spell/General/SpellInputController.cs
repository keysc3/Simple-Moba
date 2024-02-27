using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*   Purpose: Implements spell input functionality using a spell input interface.
*
*   @author: Colin Keys
*/
public class SpellInputController
{
    private ISpellInput spellInput;

    /*
    *   SpellInput - Sets up new SpellInput.
    *   @param spellInput - ISpellInput to with methods.
    */
    public SpellInputController(ISpellInput spellInput){
        this.spellInput = spellInput;
    }

    /*
    *   SpellButtonPressed - Casts or readies a spell when its button or input is pressed.
    *   @param buttonPressed - KeyCode of the input used for the spell pressed.
    *   @param  spellPressed - Spell of the spell pressed.
    */
    public void SpellButtonPressed(KeyCode buttonPressed, ISpell spellPressed){
        // Only attempt to cast if learned.
        if((spellPressed.IsSummonerSpell || spellInput.SpellLevels[spellPressed.SpellNum] > 0)){
            // Hide cast if a spell was readied and new button pressed is different than last.
            if(spellInput.LastSpellPressed != null && spellInput.LastButtonPressed != buttonPressed)
                spellInput.LastSpellPressed.HideCast();
            // If pressed spell is a castable spell.
            if(spellPressed is IHasCast || spellPressed is IHasTargetedCast){
                // If spell is not cast on press.
                if(!spellPressed.IsQuickCast){
                    // If Last button press is different new than new button press, ready the spell.
                    if(spellInput.LastButtonPressed != buttonPressed){
                        ReadySpell(buttonPressed, spellPressed);
                    }
                }
                // Cast the spell since it is cast on press.
                else{
                    if(!((IHasCast) spellPressed).Cast())
                        CantCastMessage(spellPressed);
                    UnreadySpell();
                }
            }
        }
        else{
            CantCastMessage(spellPressed);
        }
    }

    private void CantCastMessage(ISpell spellPressed){
        Debug.Log("Can't cast " + spellPressed + " yet!");
    }

    /*
    *   LeftClick - Handles the actions to take when a left click is inputted.
    *   @param mainCamera - Camera to use for ray casting.
    */
    public void LeftClick(Ray ray){
        // If a spell is readied and the input is not from a button click.
        if(spellInput.LastSpellPressed != null && !spellInput.ButtonClick){
            // Get GameObject the player wants to cast on. 
            if(spellInput.LastSpellPressed is IHasTargetedCast){
                RaycastHit hitInfo;
                // If the player click hit a GameObject.
                if(Physics.Raycast(ray, out hitInfo)){
                    // Handle Unit checking in the spell.
                    IUnit hit = hitInfo.collider.gameObject.GetComponent<IUnit>();
                    if(hit != null)
                        ((IHasTargetedCast) spellInput.LastSpellPressed).AttemptCast(hit);
                    else
                        Debug.Log("No unit selected.");
                }
            }
            // Cast spell.
            else if(spellInput.LastSpellPressed is IHasCast){
                if(!((IHasCast) spellInput.LastSpellPressed).Cast())
                    CantCastMessage(spellInput.LastSpellPressed);
            }
            // Unready spell.
            UnreadySpell();
        }
    }

    /*
    *   CheckForUnready - Unready spell if any input is pressed besides the readied spell.
    */
    public void CheckForUnready(){
        // If a spell has been readied and the last spell pressed is not a quick cast, then unready the spell.
        if(spellInput.LastButtonPressed != KeyCode.None){
            UnreadySpell();
        }
    }

    private void ReadySpell(KeyCode buttonPressed, ISpell spellPressed){
        Debug.Log("readied");
        spellPressed.DisplayCast();
        spellInput.LastButtonPressed = buttonPressed;
        spellInput.LastSpellPressed = spellPressed;
    }

    public void UnreadySpell(){
        if(spellInput.LastSpellPressed != null){
            //if(!spellInput.LastSpellPressed.IsQuickCast)
            spellInput.LastSpellPressed.HideCast();
            if(!(spellInput.LastSpellPressed is IHasTargetedCast))
                spellInput.LastSpellPressed = null;
            spellInput.LastButtonPressed = KeyCode.None;
        }
    }
}
