using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a drowsy effect where the target isw slowed then put to sleep after a duration.
*
* @author: Colin Keys
*/
public class Drowsy : Effect
{
    private int spellLevel;
    private float reducedAmount;
    public Slow drowsySlow { get; private set; }
    public Sleep drowsySleep { get; private set; }

    /*
    *   Drowsy - Initialize a new drowsy effect.
    *   @param dotEffect - ScriptableDrowsy of the drowsy effect to apply.
    *   @param duration - float of the duration for the drowsy to last.
    *   @param unitCasted - GameObject of the unit that casted the drowsy.
    *   @param - unitEffected - GameObject of the unit that the drowsy is affecting.
    */
    public Drowsy(ScriptableDrowsy drowsyEffect, float duration, int spellLevel, IUnit casted, IUnit effected) : base(drowsyEffect, duration, casted, effected){
        this.spellLevel = spellLevel;
        if(((ScriptableDrowsy) effectType).slow != null)
            drowsySlow = (Slow) ((ScriptableDrowsy) effectType).slow.InitializeEffect(spellLevel, casted, effected);
        if(((ScriptableDrowsy) effectType).sleep != null)
            drowsySleep = (Sleep) ((ScriptableDrowsy) effectType).sleep.InitializeEffect(spellLevel, casted, effected);
    }

    /*
    *   StartEffect - Start the drowsy effect.
    */
    public override void StartEffect(){
        if(effected != null){
            if(drowsySlow != null)
                effected.statusEffects.AddEffect(drowsySlow);
        }
    }

    /*
    *   EndEffect - End the drowsy effect.
    */
    public override void EndEffect(){
        if(effected != null){
            if(drowsySleep != null)
                effected.statusEffects.AddEffect(drowsySleep);
        }
    }
}
