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
    private int spellLevel;
    private Vector3 currentTarget;
    private NavMeshAgent effectedNavMeshAgent;
    private Collider effectedCollider;
    public Slow charmSlow { get; private set; }
    
    /*
    *   Charm - Initialize a new charm effect.
    *   @param charmEffect - ScriptableCharm of the charm effect to apply.
    *   @param duration - float of the duration for the charm to last.
    *   @param casted - IUnit of the unit that caused the effect.
    *   @param effected - IUnit of the unit that is being affected by the effect.
    */
    public Charm(ScriptableCharm charmEffect, float duration, int spellLevel, IUnit casted, IUnit effected) : base(charmEffect, duration, casted, effected){
        this.spellLevel = spellLevel;
        effectedNavMeshAgent = effected.GameObject.GetComponent<NavMeshAgent>();
        //effectedCollider = effected.GameObject.GetComponent<Collider>();
        if(((ScriptableCharm) effectType).slow != null)
            charmSlow = (Slow) ((ScriptableCharm) effectType).slow.InitializeEffect(spellLevel, casted, effected);
    }

    /*
    *   StartEffect - Start the charm effect.
    */
    public override void StartEffect(){
        if(effected != null){
            // If the charmed unit ia a champion disable their controls.
            if(effected is IPlayer){
                effected.GameObject.GetComponent<PlayerControllerBehaviour>().enabled = false;
                effected.GameObject.GetComponent<SpellInputBehaviour>().enabled = false;
            }
            // Reset the units current path.
            if(effectedNavMeshAgent != null)
                effectedNavMeshAgent.ResetPath();
            if(charmSlow != null)
                effected.statusEffects.AddEffect(charmSlow);
        }
    }

    /*
    *   EndEffect - End the charm effect.
    */
    public override void EndEffect(){
        if(effected != null){
            if(effected is IPlayer){
                // Give controls back if charmed is active GameObject.
                if(ActiveChampion.instance.players[ActiveChampion.instance.ActiveChamp] == effected){
                    effected.GameObject.GetComponent<PlayerControllerBehaviour>().enabled = true;
                    effected.GameObject.GetComponent<SpellInputBehaviour>().enabled = true;
                }
            }
            // Reset the path and speed from the charm effect.
            if(effectedNavMeshAgent != null)
                effectedNavMeshAgent.ResetPath();
        }
    }

    /*
    *   EffectTick - Tick for the charms effect.
    */
    public override void EffectTick(){
        effectedNavMeshAgent.destination = casted.Position;
        Vector3 nextTarget;
        // If a path is set.
        if(effectedNavMeshAgent.hasPath){
            nextTarget = effectedNavMeshAgent.steeringTarget;
            // If a new target location exists set the target and look at the target location.
            if(currentTarget != nextTarget){
                nextTarget.y = effected.GameObject.transform.position.y;
                effected.GameObject.transform.LookAt(nextTarget);
                currentTarget = nextTarget;
            }
        } 
    }
}
