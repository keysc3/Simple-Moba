using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Slow : Effect
{
    private float effectedSpeed;
    private NavMeshAgent effectedNavMeshAgent;
    private UnitStats effectedUnitStats;
    
    /*
    *   Slow - Initialize a new slow effect.
    *   @param slowEffect - ScriptableSlow of the slow effect to apply.
    *   @param duration - float of the duration for the slow to last.
    *   @param unitCasted - GameObject of the unit that casted the slow.
    *   @param - unitEffected - GameObject of the unit that the slow is affecting.
    */
    public Slow(ScriptableSlow slowEffect, float duration, GameObject unitCasted, GameObject unitEffected) : base(slowEffect, duration, unitCasted, unitEffected){
        effectedUnitStats = effected.GetComponent<UnitStats>();
        effectedNavMeshAgent = effected.GetComponent<NavMeshAgent>();
    }

    /*
    *   StartEffect - Start the slow effect.
    */
    public override void StartEffect(){
        float speed = effectedUnitStats.speed.GetValue();
        effectedSpeed = speed * ((ScriptableSlow) effectType).slowPercent;
        effectedNavMeshAgent.speed = speed - effectedSpeed;
    }

    /*
    *   EndEffect - End the slow effect.
    */
    public override void EndEffect(){
        effectedNavMeshAgent.speed =  effectedNavMeshAgent.speed + effectedSpeed;
    }
}
