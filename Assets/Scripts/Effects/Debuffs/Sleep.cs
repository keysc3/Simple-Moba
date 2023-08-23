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
    private IUnit effectedUnit;
    private NavMeshAgent effectedNavMeshAgent;
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
        effectedUnit = effected.GetComponent<IUnit>();
        effectedNavMeshAgent = effected.GetComponent<NavMeshAgent>();
        //playerController = effected.GetComponent<PlayerController>();
        //playerSpellInput = effected.GetComponent<PlayerSpellInput>();
    }

    /*
    *   StartEffect - Start the sleep effect.
    */
    public override void StartEffect(){
        if(effectedUnit != null){
            if(effectedUnit is IPlayer){
                effected.GetComponent<PlayerControllerBehaviour>().enabled = false;
                effected.GetComponent<SpellInputBehaviour>().enabled = false;
            }
            // Add bonus effect method to a take damage delegate?
            // Reset the units current path.
            if(effectedNavMeshAgent != null)
                effectedNavMeshAgent.ResetPath();
        }
    }

    /*
    *   EndEffect - End the sleep effect.
    */
    public override void EndEffect(){
        if(effectedUnit != null){
            if(effectedUnit is IPlayer){
                // Give controls back if slept is active GameObject.
                if(ActiveChampion.instance.players[ActiveChampion.instance.ActiveChamp] == effectedUnit){
                    effected.GetComponent<PlayerControllerBehaviour>().enabled = true;
                    effected.GetComponent<SpellInputBehaviour>().enabled = true;
                }
            }
            if(effectedNavMeshAgent != null)
                effectedNavMeshAgent.ResetPath();
        }
    }
}
