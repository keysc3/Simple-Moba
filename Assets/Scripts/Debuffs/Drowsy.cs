using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Drowsy : Effect
{
    private UnitStats effectedUnitStats;
    private float slowPercent;
    private float reducedAmount;
    private NavMeshAgent effectedNavMeshAgent;

    /*
    *   Drowsy - Initialize a new drowsy effect.
    *   @param dotEffect - ScriptableDrowsy of the drowsy effect to apply.
    *   @param duration - float of the duration for the drowsy to last.
    *   @param unitCasted - GameObject of the unit that casted the drowsy.
    *   @param - unitEffected - GameObject of the unit that the drowsy is affecting.
    */
    public Drowsy(ScriptableDrowsy drowsyEffect, float slowPercent, float duration, GameObject unitCasted, GameObject unitEffected) : base(drowsyEffect, duration, unitCasted, unitEffected){
        effectedUnitStats = effected.GetComponent<UnitStats>();
        this.slowPercent = slowPercent;
        effectedNavMeshAgent = effected.GetComponent<NavMeshAgent>();
    }

    /*
    *   StartEffect - Start the drowsy effect.
    */
    public override void StartEffect(){
        float speed = effectedUnitStats.speed.GetValue();
        reducedAmount = speed * slowPercent;
        effectedNavMeshAgent.speed = speed - reducedAmount;
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
