using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a slow effect.
*
* @author: Colin Keys
*/
public class Slow : Effect
{
    private float slowPercent;
    public float SlowPercent {
        get => slowPercent;
        set {
            if(value >= 0f && value <= 1f)
                slowPercent = value;
        }
    }

    /*
    *   Slow - Initialize a new slow effect.
    *   @param slowEffect - ScriptableSlow of the slow effect to apply.
    *   @param duration - float of the duration for the slow to last.
    *   @param unitCasted - GameObject of the unit that casted the slow.
    *   @param - unitEffected - GameObject of the unit that the slow is affecting.
    */
    public Slow(ScriptableSlow slowEffect, float duration, float slowPercent, IUnit casted, IUnit effected) : base(slowEffect, duration, casted, effected){
        this.slowPercent = slowPercent;
    }

    /*
    *   isEffectStronger - Checks if the the second effect is stronger than the first.
    *   @param strongest - Slow of a slow.
    *   @param newSlow - Slow of the slow to check if stronger.
    */
    public bool isEffectStronger(Slow strongest, Slow newSlow){
        if(newSlow.slowPercent > strongest.slowPercent){
            return true;
        }
        else
            return false;
    }

    /*
    *   GetStrongest - Finds the index of the strongest slow. GetStrongest is only called if a slow exists in the list given.
    *   @param myEffects - List containing all effects to check.
    *   @return int - int of the index the strongest slow is at.
    */
    public int GetStrongest(List<Effect> myEffects){
        int strongestIndex = -1;
        int i = 0;
        foreach(Effect effect in myEffects){
            if(effect is Slow){
                if(strongestIndex != -1){
                    if(isEffectStronger((Slow) myEffects[strongestIndex], (Slow) effect))
                        strongestIndex = i;
                }
                else
                    strongestIndex = i;
            }
            i++;
        }
        return strongestIndex;
    }
}
