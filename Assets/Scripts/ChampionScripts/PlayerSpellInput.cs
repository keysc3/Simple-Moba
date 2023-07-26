using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

/*
* Purpose: Handles the input for casting spells.
*
* @author: Colin Keys
*/
public class PlayerSpellInput : MonoBehaviour
{
    private ChampionSpells championSpells;
    private LevelManager levelManager;
    private Player player;
    private KeyCode lastButtonPressed;
    private Spell lastSpellPressed;

    // Called when the script instance is being loaded.
    private void Awake(){
        player = GetComponent<Player>();
        championSpells = GetComponent<ChampionSpells>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        levelManager = player.levelManager;
    }

    // Update is called once per frame
    private void Update(){   
        if(Input.anyKeyDown){
            if(!Input.GetKeyDown(lastButtonPressed) && !Input.GetMouseButtonDown(0)){
                if(lastSpellPressed != null && lastSpellPressed is IDisplayable){
                    if(((IDisplayable) lastSpellPressed).isDisplayed)
                        ((IDisplayable) lastSpellPressed).HideCast();
                    lastSpellPressed = null;
                }
            }
        }
        // Spell 1
        if(Input.GetKeyDown(KeyCode.Q)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                SpellButtonPressed(KeyCode.Q, championSpells.spell1);
            }
        }
        // Spell 2
        if(Input.GetKeyDown(KeyCode.W)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                SpellButtonPressed(KeyCode.W, championSpells.spell2);
            }
        }
        // Spell 3
        if(Input.GetKeyDown(KeyCode.E)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                SpellButtonPressed(KeyCode.E, championSpells.spell3);
            }
        }
        // Spell 4
        if(Input.GetKeyDown(KeyCode.R)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                SpellButtonPressed(KeyCode.R, championSpells.spell4);
            }
        }

        if(Input.GetMouseButtonDown(0))
            LeftClick();
    }

    private void SpellButtonPressed(KeyCode buttonPressed, Spell spellPressed){
        // Only attempt to cast if learned.
        if(levelManager.spellLevels[spellPressed.spellNum] > 0 && !spellPressed.onCd){
            if(spellPressed is ICastable){
                if(spellPressed is IDisplayable){
                    ((IDisplayable) spellPressed).DisplayCast();
                    lastButtonPressed = buttonPressed;
                    lastSpellPressed = spellPressed;
                }
                else{
                    ((ICastable) spellPressed).Cast();
                    lastSpellPressed = null;
                }
            }
        }
        else{
            Debug.Log("Can't cast " + spellPressed + " yet!");
        }
    }

    private void LeftClick(){
        if(lastSpellPressed != null){
            if(lastSpellPressed is ITargetCastable){
                // Check for target
                //((ITargetCastable) lastSpellPressed).Cast(target)
            }
            else{
                if(lastSpellPressed is IDisplayable){
                    ((IDisplayable) lastSpellPressed).HideCast();
                }
                ((ICastable) lastSpellPressed).Cast();
                lastSpellPressed = null;
            }
        }
    }
}
