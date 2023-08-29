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
    *   @param casted - IUnit of the unit that caused the effect.
    *   @param effected - IUnit of the unit that is being affected by the effect.
    */
    public Sleep(ScriptableSleep sleepEffect, float duration, IUnit casted, IUnit effected) : base(sleepEffect, duration, casted, effected){
        effectedNavMeshAgent = effected.GameObject.GetComponent<NavMeshAgent>();
    }

    /*
    *   StartEffect - Start the sleep effect.
    */
    public override void StartEffect(){
        if(effected != null){
            if(effected is IPlayer){
                effected.GameObject.GetComponent<PlayerControllerBehaviour>().enabled = false;
                effected.GameObject.GetComponent<SpellInputBehaviour>().enabled = false;
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
        if(effected != null){
            if(effected is IPlayer){
                // Give controls back if slept is active GameObject.
                if(ActiveChampion.instance.players[ActiveChampion.instance.ActiveChamp] == effected){
                    effected.GameObject.GetComponent<PlayerControllerBehaviour>().enabled = true;
                    effected.GameObject.GetComponent<SpellInputBehaviour>().enabled = true;
                }
            }
            if(effectedNavMeshAgent != null)
                effectedNavMeshAgent.ResetPath();
        }
    }
}
