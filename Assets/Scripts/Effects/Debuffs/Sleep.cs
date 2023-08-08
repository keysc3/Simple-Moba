using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements a sleep effect where the slept has all spell and movement disabled.
*
* @author: Colin Keys
*/
public class Sleep : Effect
{
    private Unit effectedUnit;
    //private NavMeshAgent effectedNavMeshAgent;
    //private PlayerController playerController;
    //private PlayerSpellInput playerSpellInput;

    /*
    *   Sleep - Initialize a new sleep effect.
    *   @param sleepEffect - ScriptableSleep of the sleep effect to apply.
    *   @param duration - float of the duration for the sleep to last.
    *   @param unitCasted - GameObject of the unit that casted the sleep.
    *   @param unitEffected - GameObject of the unit that the sleeep is affecting.
    */
    public Sleep(ScriptableSleep sleepEffect, float duration, GameObject unitCasted, GameObject unitEffected) : base(sleepEffect, duration, unitCasted, unitEffected){
        effectedUnit = effected.GetComponent<Unit>();
        //effectedNavMeshAgent = effected.GetComponent<NavMeshAgent>();
        //playerController = effected.GetComponent<PlayerController>();
        //playerSpellInput = effected.GetComponent<PlayerSpellInput>();
    }

    /*
    *   StartEffect - Start the sleep effect.
    */
    public override void StartEffect(){
        if(effectedUnit != null){
            if(effectedUnit is Player){
                Player player = (Player) effectedUnit;
                player.GetComponent<PlayerController>().enabled = false;
                player.GetComponent<PlayerSpellInput>().enabled = false;
            }
            // Add bonus effect method to a take damage delegate?
            // Reset the units current path.
            effectedUnit.navMeshAgent.ResetPath();
        }
    }

    /*
    *   EndEffect - End the sleep effect.
    */
    public override void EndEffect(){
        if(effectedUnit != null){
            // Reset the path
            effectedUnit.navMeshAgent.ResetPath();
            if(effectedUnit is Player){
                // Give controls back if slept is active GameObject.
                if(ActiveChampion.instance.champions[ActiveChampion.instance.ActiveChamp] == effected){
                    Player player = (Player) effectedUnit;
                    player.GetComponent<PlayerController>().enabled = true;
                    player.GetComponent<PlayerSpellInput>().enabled = true;
                }
            }
        }
    }
}
