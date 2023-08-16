using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSpellInput
{
    private ISpellInput _si;

    public NewSpellInput(ISpellInput si){
        _si = si;
    }

    /*
    *   SpellButtonPressed - Casts or readies a spell when its button or input is pressed.
    *   @param buttonPressed - KeyCode of the input used for the spell pressed.
    *   @param  spellPressed - Spell of the spell pressed.
    */
    public void SpellButtonPressed(KeyCode buttonPressed, Spell spellPressed){
        // Only attempt to cast if learned.
        if(_si.SpellLevels[spellPressed.SpellNum] > 0 && !spellPressed.onCd){
            // Hide cast if a spell was prepped and new button pressed is different than last.
            if(_si.LastSpellPressed != null && _si.LastButtonPressed != buttonPressed)
                _si.LastSpellPressed.HideCast();
            // If pressed spell is a castable spell.
            if(spellPressed is ICastable){
                // If spell is not cast on press.
                if(!spellPressed.isQuickCast){
                    // If Last button press is different new than new button press, prep the spell.
                    if(_si.LastButtonPressed != buttonPressed){
                        spellPressed.DisplayCast();
                        _si.LastButtonPressed = buttonPressed;
                        _si.LastSpellPressed = spellPressed;
                    }
                }
                // Cast the spell since it is cast on press.
                else{
                    ((ICastable) spellPressed).Cast();
                    _si.LastSpellPressed = null;
                    _si.LastButtonPressed = KeyCode.None;
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
        if(_si.LastSpellPressed != null && !_si.ButtonClick){
            // If prepped spell is not instant cast then hide its cast.
            if(!_si.LastSpellPressed.isQuickCast){
                _si.LastSpellPressed.HideCast();
            }
            // Get GameObject the player wants to cast on. 
            if(_si.LastSpellPressed is ITargetCastable){
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                // If the player click hit a GameObject.
                if(Physics.Raycast(ray, out hitInfo))
                    // Handle GamObject checking in the spell.
                    ((ITargetCastable) _si.LastSpellPressed).Cast(hitInfo.collider.gameObject);
            }
            // Cast spell.
            else{
                ((ICastable) _si.LastSpellPressed).Cast();
            }
            // Unprepare spell.
            _si.LastSpellPressed = null;
            _si.LastButtonPressed = KeyCode.None;
        }
    }

    public void AnyInput(){
        // If the input detected is different than the last button press and not left click.
        if(!Input.GetKeyDown(_si.LastButtonPressed) && !Input.GetMouseButtonDown(0)){
            // If a spell has been prepped and the last spell pressed is not a quick cast, then unprepare the spell.
            if(_si.LastSpellPressed != null && !_si.LastSpellPressed.isQuickCast){
                _si.LastSpellPressed.HideCast();
                _si.LastSpellPressed = null;
                _si.LastButtonPressed = KeyCode.None;
            }
        }
    }
}
