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
    private IUnit effectedUnit;
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
    public Drowsy(ScriptableDrowsy drowsyEffect, float duration, int spellLevel, GameObject unitCasted, GameObject unitEffected) : base(drowsyEffect, duration, unitCasted, unitEffected){
        this.spellLevel = spellLevel;
        effectedUnit = effected.GetComponent<IUnit>();
        if(((ScriptableDrowsy) effectType).slow != null)
            drowsySlow = (Slow) ((ScriptableDrowsy) effectType).slow.InitializeEffect(spellLevel, casted, effected);
        if(((ScriptableDrowsy) effectType).sleep != null)
            drowsySleep = (Sleep) ((ScriptableDrowsy) effectType).sleep.InitializeEffect(spellLevel, casted, effected);
    }

    /*
    *   StartEffect - Start the drowsy effect.
    */
    public override void StartEffect(){
        if(effectedUnit != null){
            if(drowsySlow != null)
                effectedUnit.statusEffects.AddEffect(drowsySlow);
        }
    }

    /*
    *   EndEffect - End the drowsy effect.
    */
    public override void EndEffect(){
        if(effectedUnit != null){
            if(drowsySleep != null)
                effectedUnit.statusEffects.AddEffect(drowsySleep);
        }
    }
}
