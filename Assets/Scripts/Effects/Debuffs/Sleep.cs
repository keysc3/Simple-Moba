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
    private NavMeshAgent effectedNavMeshAgent;
    /*
    *   Sleep - Initialize a new sleep effect.
    *   @param sleepEffect - ScriptableSleep of the sleep effect to apply.
    *   @param duration - float of the duration for the sleep to last.
    *   @param unitCasted - GameObject of the unit that casted the sleep.
    *   @param unitEffected - GameObject of the unit that the sleeep is affecting.
    */
    public Sleep(ScriptableSleep sleepEffect, float duration, GameObject unitCasted, GameObject unitEffected) : base(sleepEffect, duration, unitCasted, unitEffected){
        effectedNavMeshAgent = effected.GetComponent<NavMeshAgent>();
    }

    /*
    *   StartEffect - Start the sleep effect.
    */
    public override void StartEffect(){
        effected.GetComponent<PlayerController>().enabled = false;
        effected.GetComponent<PlayerSpellInput>().enabled = false;
        // Add bonus effect method to a take damage delegate?
        // Reset the units current path.
        effectedNavMeshAgent.ResetPath();
    }

    /*
    *   EndEffect - End the sleep effect.
    */
    public override void EndEffect(){
        // Reset the path
        effectedNavMeshAgent.ResetPath();
        // Give controls back if slept is active GameObject.
        if(ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion] == effected){
            effected.GetComponent<PlayerController>().enabled = true;
            effected.GetComponent<PlayerSpellInput>().enabled = true;
        }
    }
}
