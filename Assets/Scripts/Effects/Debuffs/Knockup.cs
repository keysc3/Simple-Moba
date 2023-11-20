using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Knockup : Effect
{
    private NavMeshAgent effectedNavMeshAgent;
    private float initialHeight;
    private float maxHeight;
    /*
    *   Knockup - Initialize a new knockup effect.
    *   @param knockupEffect - ScriptableKnockup of the knockup effect to apply.
    *   @param duration - float of the duration for the knockup to last.
    *   @param casted - IUnit of the unit that caused the effect.
    *   @param effected - IUnit of the unit that is being affected by the effect.
    */
    public Knockup(ScriptableKnockup knockupEffect, float duration, IUnit casted, IUnit effected) : base(knockupEffect, duration, casted, effected){
        effectedNavMeshAgent = effected.GameObject.GetComponent<NavMeshAgent>();
        initialHeight = effected.Position.y;
        maxHeight = ((ScriptableKnockup) effectType).knockupHeight;
    }

    /*
    *   StartEffect - Start the knockup effect.
    */
    public override void StartEffect(){
        if(effected != null){
            // If the knockuped unit ia a champion disable their controls.
            if(effected is IPlayer){
                effected.GameObject.GetComponent<PlayerControllerBehaviour>().enabled = false;
                effected.GameObject.GetComponent<SpellInputBehaviour>().enabled = false;
            }
            if(effectedNavMeshAgent != null){
                effectedNavMeshAgent.ResetPath();
                effectedNavMeshAgent.enabled = false;
            }
        }
    }

    /*
    *   EndEffect - End the knockup effect.
    */
    public override void EndEffect(){
        if(effected != null){
            if(effected is IPlayer){
                // Give controls back if knocked up is active GameObject.
                if(ActiveChampion.instance.players[ActiveChampion.instance.ActiveChamp] == effected){
                    effected.GameObject.GetComponent<PlayerControllerBehaviour>().enabled = true;
                    effected.GameObject.GetComponent<SpellInputBehaviour>().enabled = true;
                }
            }
            if(effectedNavMeshAgent != null){
                effectedNavMeshAgent.enabled = true;
            }
        }
    }

    /*
    *   EffectTick - Tick for the charms effect.
    */
    public override void EffectTick(){
        Vector3 position = effected.Position;
        float endPosition = maxHeight;
        float startPosition = initialHeight;
        float timer = effectTimer;
        if(effectTimer > (effectDuration/2f)){
            endPosition = initialHeight;
            startPosition = maxHeight;
            timer -= (effectDuration/2f);
        }
        float height = Mathf.Lerp(startPosition, endPosition, timer/(effectDuration/2f));
        effected.GameObject.transform.position = new Vector3(position.x, height, position.z);
    }
}
