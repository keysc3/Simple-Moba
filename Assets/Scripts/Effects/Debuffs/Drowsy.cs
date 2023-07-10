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
    private UnitStats effectedUnitStats;
    private float reducedAmount;
    private NavMeshAgent effectedNavMeshAgent;
    public Effect associatedSlow { get; private set; }

    /*
    *   Drowsy - Initialize a new drowsy effect.
    *   @param dotEffect - ScriptableDrowsy of the drowsy effect to apply.
    *   @param duration - float of the duration for the drowsy to last.
    *   @param unitCasted - GameObject of the unit that casted the drowsy.
    *   @param - unitEffected - GameObject of the unit that the drowsy is affecting.
    */
    public Drowsy(ScriptableDrowsy drowsyEffect, float duration, GameObject unitCasted, GameObject unitEffected) : base(drowsyEffect, duration, unitCasted, unitEffected){
        effectedUnitStats = effected.GetComponent<UnitStats>();
        effectedNavMeshAgent = effected.GetComponent<NavMeshAgent>();
    }

    /*
    *   StartEffect - Start the drowsy effect.
    */
    public override void StartEffect(){
        associatedSlow = ((ScriptableDrowsy) effectType).slow.InitializeEffect(casted, effected);
        effected.GetComponent<StatusEffectManager>().AddEffect(associatedSlow);
    }

    /*
    *   EndEffect - End the drowsy effect.
    */
    public override void EndEffect(){
        effectedNavMeshAgent.speed =  effectedNavMeshAgent.speed + reducedAmount;
        // Initialize new sleep effect.
        effected.GetComponent<StatusEffectManager>().AddEffect(((ScriptableDrowsy) effectType).sleep
        .InitializeEffect(((ScriptableDrowsy) effectType).spellLevel, casted, effected));
    }
}
