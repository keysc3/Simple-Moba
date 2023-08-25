using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellInput
{
    private ISpellInput spellInputInterface;

    public SpellInput(ISpellInput spellInputInterface){
        this.spellInputInterface = spellInputInterface;
    }

    /*
    *   SpellButtonPressed - Casts or readies a spell when its button or input is pressed.
    *   @param buttonPressed - KeyCode of the input used for the spell pressed.
    *   @param  spellPressed - Spell of the spell pressed.
    */
    public void SpellButtonPressed(KeyCode buttonPressed, ISpell spellPressed){
        // Only attempt to cast if learned.
        if(spellInputInterface.SpellLevels[spellPressed.SpellNum] > 0 && !spellPressed.OnCd){
            // Hide cast if a spell was prepped and new button pressed is different than last.
            if(spellInputInterface.LastSpellPressed != null && spellInputInterface.LastButtonPressed != buttonPressed)
                spellInputInterface.LastSpellPressed.HideCast();
            // If pressed spell is a castable spell.
            if(spellPressed is IHasCast){
                // If spell is not cast on press.
                if(!spellPressed.IsQuickCast){
                    // If Last button press is different new than new button press, prep the spell.
                    if(spellInputInterface.LastButtonPressed != buttonPressed){
                        spellPressed.DisplayCast();
                        spellInputInterface.LastButtonPressed = buttonPressed;
                        spellInputInterface.LastSpellPressed = spellPressed;
                    }
                }
                // Cast the spell since it is cast on press.
                else{
                    ((IHasCast) spellPressed).Cast();
                    spellInputInterface.LastSpellPressed = null;
                    spellInputInterface.LastButtonPressed = KeyCode.None;
                }
            }
        }
        else{
            Debug.Log("Can't cast " + spellPressed + " yet!");
        }
    }

    /*
    *   LeftClick - Handles the actions to take when a left click is inputted.
    */
    public void LeftClick(Camera mainCamera){
        // If a spell is prepped and the input is not from a button click.
        if(spellInputInterface.LastSpellPressed != null && !spellInputInterface.ButtonClick){
            // If prepped spell is not instant cast then hide its cast.
            if(!spellInputInterface.LastSpellPressed.IsQuickCast){
                spellInputInterface.LastSpellPressed.HideCast();
            }
            // Get GameObject the player wants to cast on. 
            if(spellInputInterface.LastSpellPressed is IHasTargetedCast){
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                // If the player click hit a GameObject.
                if(Physics.Raycast(ray, out hitInfo))
                    // Handle GamObject checking in the spell.
                    ((IHasTargetedCast) spellInputInterface.LastSpellPressed).Cast(hitInfo.collider.gameObject.GetComponent<IUnit>());
            }
            // Cast spell.
            else{
                ((IHasCast) spellInputInterface.LastSpellPressed).Cast();
            }
            // Unprepare spell.
            spellInputInterface.LastSpellPressed = null;
            spellInputInterface.LastButtonPressed = KeyCode.None;
        }
    }

    public void AnyInput(){
        // If the input detected is different than the last button press and not left click.
        if(!Input.GetKeyDown(spellInputInterface.LastButtonPressed) && !Input.GetMouseButtonDown(0)){
            // If a spell has been prepped and the last spell pressed is not a quick cast, then unprepare the spell.
            if(spellInputInterface.LastSpellPressed != null && !spellInputInterface.LastSpellPressed.IsQuickCast){
                spellInputInterface.LastSpellPressed.HideCast();
                spellInputInterface.LastSpellPressed = null;
                spellInputInterface.LastButtonPressed = KeyCode.None;
            }
        }
    }
}
