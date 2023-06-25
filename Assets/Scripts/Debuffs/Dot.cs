using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : Effect
{

    private UnitStats effectedUnitStats;
    private float damagePerTick;
    private float totalDamage;
    private float totalDealt;
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
        effectedUnitStats = effected.GetComponent<UnitStats>();
        this.totalDamage = totalDamage;
    }

    /*
    *   StartEffect - Start the dot effect.
    */
    public override void StartEffect(){
        damagePerTick = totalDamage/(effectDuration/((ScriptableDot) effectType).tickRate);
        nextTick = Time.time;
        totalDealt = 0f;
    }

    /*
    *   EndEffect - End the dot effect.
    */
    public override void EndEffect(){
        Debug.Log("Damage dealt from dot: " + totalDealt);
        // Placeholder.
    }

    /*
    *   EffectTick - Dot effect tick.
    */
    public override void EffectTick(float delta){
        if(nextTick <= Time.time){
            totalDealt += damagePerTick;
            effectedUnitStats.TakeDamage(damagePerTick, ((ScriptableDot) effectType).damageType, effected);
            nextTick = Time.time + ((ScriptableDot) effectType).tickRate;
        }
        base.EffectTick(delta);
    }

}
