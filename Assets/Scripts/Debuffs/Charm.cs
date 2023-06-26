using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements a charm effect where the charmed moves towards the unit that charmed them.
*
* @author: Colin Keys
*/
public class Charm : Effect
{
    private float effectedSpeed;
    private Vector3 currentTarget;
    private NavMeshAgent effectedNavMeshAgent;
    private UnitStats effectedUnitStats;
    
    /*
    *   Charm - Initialize a new charm effect.
    *   @param charmEffect - ScriptableCharm of the charm effect to apply.
    *   @param duration - float of the duration for the charm to last.
    *   @param unitCasted - GameObject of the unit that casted the charm.
    *   @param - unitEffected - GameObject of the unit that the charm is affecting.
    */
    public Charm(ScriptableCharm charmEffect, float duration, GameObject unitCasted, GameObject unitEffected) : base(charmEffect, duration, unitCasted, unitEffected){
        effectedUnitStats = effected.GetComponent<UnitStats>();
        effectedSpeed = effectedUnitStats.speed.GetValue();
        effectedNavMeshAgent = effected.GetComponent<NavMeshAgent>();
    }

    /*
    *   StartEffect - Start the charm effect.
    */
    public override void StartEffect(){
        // If the charmed unit ia a champion disable their controls.
        if(effectedUnitStats.unit is Champion){
            effected.GetComponent<PlayerController>().enabled = false;
            effected.GetComponent<PlayerSpellInput>().enabled = false;
        }
        // Reset the units current path.
        effectedNavMeshAgent.ResetPath();
    }

    /*
    *   EndEffect - End the charm effect.
    */
    public override void EndEffect(){
        // Reset the path and speed from the charm effect.
        effectedNavMeshAgent.ResetPath();
        effectedNavMeshAgent.speed = effectedSpeed;

        // Give controls back if charmed is active GameObject.
        if(effectedUnitStats.unit is Champion && ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion] == effected){
            effected.GetComponent<PlayerController>().enabled = true;
            effected.GetComponent<PlayerSpellInput>().enabled = true;
        }
    }

    /*
    *   EffectTick - Start the charm effect.
    */
    public override void EffectTick(float delta){
        base.EffectTick(delta);
        Debug.Log(effected);
        //effected.GetComponent<NavMeshAgent>().ResetPath();
        effectedNavMeshAgent.speed = effectedSpeed * ((ScriptableCharm) effectType).slowPercent;
        effectedNavMeshAgent.destination = casted.transform.position;
        Vector3 nextTarget;
        if(effectedNavMeshAgent.hasPath){
            nextTarget = effectedNavMeshAgent.steeringTarget;
            if(currentTarget != nextTarget){
                nextTarget.y = effectedCollider.bounds.center.y;
                effected.transform.LookAt(nextTarget);
                currentTarget = nextTarget;
            }
        }  
    }
}
