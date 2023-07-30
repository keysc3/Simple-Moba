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
    private Camera mainCamera;
    public bool buttonClick = false;

    // Called when the script instance is being loaded.
    private void Awake(){
        player = GetComponent<Player>();
        championSpells = GetComponent<ChampionSpells>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        levelManager = player.levelManager;
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    private void Update(){   
        if(Input.anyKeyDown){
            if(!Input.GetKeyDown(lastButtonPressed) && !Input.GetMouseButtonDown(0)){
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

        if(Input.GetMouseButtonDown(0)){
            LeftClick();
        }
    }

    void LateUpdate(){
        buttonClick = false;
    }

    public void SpellButtonPressed(KeyCode buttonPressed, Spell spellPressed){
        // Only attempt to cast if learned.
        if(levelManager.spellLevels[spellPressed.spellNum] > 0 && !spellPressed.onCd){
            if(lastSpellPressed != null)
                lastSpellPressed.HideCast();
            if(spellPressed is ICastable){
                if(!spellPressed.isQuickCast){
                    if(lastButtonPressed != buttonPressed){
                        spellPressed.DisplayCast();
                        lastButtonPressed = buttonPressed;
                        lastSpellPressed = spellPressed;
                    }
                }
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

    private void LeftClick(){
        if(lastSpellPressed != null && !buttonClick){
            if(!lastSpellPressed.isQuickCast){
                lastSpellPressed.HideCast();
            }
            // Get GameObject the player wants to cast on. 
            if(lastSpellPressed is ITargetCastable){
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if(Physics.Raycast(ray, out hitInfo))
                    // Handle GamObject checking in the spell.
                    ((ITargetCastable) lastSpellPressed).Cast(hitInfo.collider.gameObject);
            }
            else{
                ((ICastable) lastSpellPressed).Cast();
            }
            lastSpellPressed = null;
            lastButtonPressed = KeyCode.None;
        }
    }
}
