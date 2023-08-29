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

    private float damagePerTick;
    //private float totalDealt;
    private float nextTick;

    /*
    *   Dot - Initialize a new dot effect.
    *   @param dotEffect - ScriptableDot of the dot effect to apply.
    *   @param totalDamage - float of the total damage to deal over the dots duration.
    *   @param duration - float of the duration for the dot to last.
    *   @param unitCasted - GameObject of the unit that casted the dot.
    *   @param - unitEffected - GameObject of the unit that the dot is affecting.
    */
    public Dot(ScriptableDot dotEffect, float totalDamage, float duration, IUnit casted, IUnit effected) : base(dotEffect, duration, casted, effected){
        // Get damage to deal on each tick.
        damagePerTick = totalDamage/(effectDuration/((ScriptableDot) effectType).tickRate);
        nextTick = Time.time;
        
    }

    /*
    *   EffectTick - Tick for the dots effect.
    */
    public override void EffectTick(){
        if(nextTick <= Time.time){
            // Apply the dot and calculate next tick time.
            if(effected is IDamageable)
                ((IDamageable) effected).TakeDamage(damagePerTick, ((ScriptableDot) effectType).damageType, casted, true);
            nextTick = Time.time + ((ScriptableDot) effectType).tickRate;
        }
    }
}
