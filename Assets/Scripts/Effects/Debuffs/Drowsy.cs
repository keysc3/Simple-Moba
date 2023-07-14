using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements a drowsy effect where the target isw slowed then put to sleep after a duration.
*
* @author: Colin Keys
*/
public class Drowsy : Effect
{
    private int spellLevel;
    private UnitStats effectedUnitStats;
    private float reducedAmount;
    private NavMeshAgent effectedNavMeshAgent;

    /*
    *   Drowsy - Initialize a new drowsy effect.
    *   @param dotEffect - ScriptableDrowsy of the drowsy effect to apply.
    *   @param duration - float of the duration for the drowsy to last.
    *   @param unitCasted - GameObject of the unit that casted the drowsy.
    *   @param - unitEffected - GameObject of the unit that the drowsy is affecting.
    */
    public Drowsy(ScriptableDrowsy drowsyEffect, float duration, int spellLevel, GameObject unitCasted, GameObject unitEffected) : base(drowsyEffect, duration, unitCasted, unitEffected){
        this.spellLevel = spellLevel;
        effectedUnitStats = effected.GetComponent<UnitStats>();
        effectedNavMeshAgent = effected.GetComponent<NavMeshAgent>();
    }

    /*
    *   StartEffect - Start the drowsy effect.
    */
    public override void StartEffect(){
        effected.GetComponent<StatusEffectManager>().AddEffect(((ScriptableDrowsy) effectType).slow
        .InitializeEffect(spellLevel, casted, effected));
    }

    /*
    *   EndEffect - End the drowsy effect.
    */
    public override void EndEffect(){
        effectedNavMeshAgent.speed =  effectedNavMeshAgent.speed + reducedAmount;
        // Initialize new sleep effect.
        effected.GetComponent<StatusEffectManager>().AddEffect(((ScriptableDrowsy) effectType).sleep
        .InitializeEffect(spellLevel, casted, effected));
    }
}
