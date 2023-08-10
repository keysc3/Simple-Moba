using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a dot effect.
*
* @author: Colin Keys
*/
public class Dot : Effect
{

    private Unit effectedUnit;
    private float damagePerTick;
    private float totalDamage;
    private float nextTick;

    /*
    *   Dot - Initialize a new dot effect.
    *   @param dotEffect - ScriptableDot of the dot effect to apply.
    *   @param totalDamage - float of the total damage to deal over the dots duration.
    *   @param duration - float of the duration for the dot to last.
    *   @param unitCasted - GameObject of the unit that casted the dot.
    *   @param - unitEffected - GameObject of the unit that the dot is affecting.
    */
    public Dot(ScriptableDot dotEffect, float totalDamage, float duration, GameObject unitCasted, GameObject unitEffected) : base(dotEffect, duration, unitCasted, unitEffected){
        effectedUnit = effected.GetComponent<Unit>();
        this.totalDamage = totalDamage;
        nextTick = Time.time;
        damagePerTick = totalDamage/(effectDuration/((ScriptableDot) effectType).tickRate);
    }

    /*
    *   EffectTick - Tick for the dots effect.
    */
    public override void EffectTick(){
        if(nextTick <= Time.time){
            // Apply the dot and calculate next tick time.
            effectedUnit.TakeDamage(damagePerTick, ((ScriptableDot) effectType).damageType, casted, true);
            nextTick = Time.time + ((ScriptableDot) effectType).tickRate;
        }
    }
}
