using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

/*
* Purpose: Handles the input for casting spells. Spells are casted on press if they are an instant cast (isQuickCast). 
* Otherwise, the spell is prepped with an initial input press then a left click to cast it. Any input that isn't the prepped spell
* unprepares it.
*
* @author: Colin Keys
*/
public class PlayerSpellInput : MonoBehaviour
{
    public bool buttonClick = false;
    public Spell lastSpellPressed { get; private set; } = null;
    private KeyCode lastButtonPressed = KeyCode.None;
    private Player player;
    private Camera mainCamera;

    // Called when the script instance is being loaded.
    private void Awake(){
        player = GetComponent<Player>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    private void Update(){
        // If any input is detected.
        if(Input.anyKeyDown){
            // If the input detected is different than the last button press and not left click.
            if(!Input.GetKeyDown(lastButtonPressed) && !Input.GetMouseButtonDown(0)){
                // If a spell has been prepped and the last spell pressed is not a quick cast, then unprepare the spell.
                if(lastSpellPressed != null && !lastSpellPressed.isQuickCast){
                    lastSpellPressed.HideCast();
                    lastSpellPressed = null;
                    lastButtonPressed = KeyCode.None;
                }
            }
        }
        // Spell 1
        if(Input.GetKeyDown(KeyCode.Q)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                SpellButtonPressed(KeyCode.Q, player.championSpells.Spell1);
            }
        }
        // Spell 2
        if(Input.GetKeyDown(KeyCode.W)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                SpellButtonPressed(KeyCode.W, player.championSpells.Spell2);
            }
        }
        // Spell 3
        if(Input.GetKeyDown(KeyCode.E)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                SpellButtonPressed(KeyCode.E, player.championSpells.Spell3);
            }
        }
        // Spell 4
        if(Input.GetKeyDown(KeyCode.R)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                SpellButtonPressed(KeyCode.R, player.championSpells.Spell4);
            }
        }
        // Left click
        if(Input.GetMouseButtonDown(0)){
            LeftClick();
        }
    }

    //LateUpdate is called after all Update functions have been called.
    private void LateUpdate(){
        buttonClick = false;
    }

    /*
    *   SpellButtonPressed - Casts or readies a spell when its button or input is pressed.
    *   @param buttonPressed - KeyCode of the input used for the spell pressed.
    *   @param  spellPressed - Spell of the spell pressed.
    */
    public void SpellButtonPressed(KeyCode buttonPressed, Spell spellPressed){
        // Only attempt to cast if learned.
        if(player.levelManager.spellLevels[spellPressed.spellNum] > 0 && !spellPressed.onCd){
            // Hide cast if a spell was prepped and new button pressed is different than last.
            if(lastSpellPressed != null && lastButtonPressed != buttonPressed)
                lastSpellPressed.HideCast();
            // If pressed spell is a castable spell.
            if(spellPressed is ICastable){
                // If spell is not cast on press.
                if(!spellPressed.isQuickCast){
                    // If Last button press is different new than new button press, prep the spell.
                    if(lastButtonPressed != buttonPressed){
                        spellPressed.DisplayCast();
                        lastButtonPressed = buttonPressed;
                        lastSpellPressed = spellPressed;
                    }
                }
                // Cast the spell since it is cast on press.
                else{
                    ((ICastable) spellPressed).Cast();
                    lastSpellPressed = null;
                    lastButtonPressed = KeyCode.None;
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
    private void LeftClick(){
        // If a spell is prepped and the input is not from a button click.
        if(lastSpellPressed != null && !buttonClick){
            // If prepped spell is not instant cast then hide its cast.
            if(!lastSpellPressed.isQuickCast){
                lastSpellPressed.HideCast();
            }
            // Get GameObject the player wants to cast on. 
            if(lastSpellPressed is ITargetCastable){
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                // If the player click hit a GameObject.
                if(Physics.Raycast(ray, out hitInfo))
                    // Handle GamObject checking in the spell.
                    ((ITargetCastable) lastSpellPressed).Cast(hitInfo.collider.gameObject);
            }
            // Cast spell.
            else{
                ((ICastable) lastSpellPressed).Cast();
            }
            // Unprepare spell.
            lastSpellPressed = null;
            lastButtonPressed = KeyCode.None;
        }
    }
}
