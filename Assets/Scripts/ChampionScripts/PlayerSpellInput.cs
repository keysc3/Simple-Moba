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
    private ChampionAbilities myAbilities;
    private LevelManager levelManager;

    // Start is called before the first frame update
    private void Start()
    {
        myAbilities = GetComponent<ChampionAbilities>();
        levelManager = GetComponent<LevelManager>();
    }

    // Update is called once per frame
    private void Update(){   
        // Spell 1
        if(Input.GetKeyDown(KeyCode.Q)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                // Only attempt to cast if learned.
                if(levelManager.spellLevels["Spell_1"] > 0)
                    myAbilities.Spell_1();
            }
        }
        // Spell 2
        if(Input.GetKeyDown(KeyCode.W)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                // Only attempt to cast if learned.
                if(levelManager.spellLevels["Spell_2"] > 0)
                    myAbilities.Spell_2();
            }
        }
        // Spell 3
        if(Input.GetKeyDown(KeyCode.E)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                // Only attempt to cast if learned.
                if(levelManager.spellLevels["Spell_3"] > 0)
                    myAbilities.Spell_3();
            }
        }
        // Spell 4
        if(Input.GetKeyDown(KeyCode.R)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                // Only attempt to cast if learned.
                if(levelManager.spellLevels["Spell_4"] > 0)
                    myAbilities.Spell_4();
            }
        }
    }
}
