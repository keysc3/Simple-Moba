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
        // Spell 1
        if(Input.GetKeyDown(KeyCode.Q)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                // Only attempt to cast if learned.
                if(levelManager.spellLevels["Spell_1"] > 0){
                    if(championSpells.spell1 is IHasCast)
                        ((IHasCast)championSpells.spell1).Cast();
                }
            }
        }
        // Spell 2
        if(Input.GetKeyDown(KeyCode.W)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                // Only attempt to cast if learned.
                if(levelManager.spellLevels["Spell_2"] > 0){
                    if(championSpells.spell2 is IHasCast)
                        ((IHasCast)championSpells.spell2).Cast();
                }
            }
        }
        // Spell 3
        if(Input.GetKeyDown(KeyCode.E)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                // Only attempt to cast if learned.
                if(levelManager.spellLevels["Spell_3"] > 0){
                    if(championSpells.spell3 is IHasCast)
                        ((IHasCast)championSpells.spell3).Cast();
                }
            }
        }
        // Spell 4
        if(Input.GetKeyDown(KeyCode.R)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                // Only attempt to cast if learned.
                if(levelManager.spellLevels["Spell_4"] > 0){
                    if(championSpells.spell4 is IHasCast)
                        ((IHasCast)championSpells.spell4).Cast();
                }
            }
        }
    }
}
