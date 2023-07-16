using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements a slow effect.
*
* @author: Colin Keys
*/
public class Slow : Effect
{
    public float slowPercent { get; private set; }
    
    /*
    *   Slow - Initialize a new slow effect.
    *   @param slowEffect - ScriptableSlow of the slow effect to apply.
    *   @param duration - float of the duration for the slow to last.
    *   @param unitCasted - GameObject of the unit that casted the slow.
    *   @param - unitEffected - GameObject of the unit that the slow is affecting.
    */
    public Slow(ScriptableSlow slowEffect, float duration, float slowPercent, GameObject unitCasted, GameObject unitEffected) : base(slowEffect, duration, unitCasted, unitEffected){
        this.slowPercent = slowPercent;
    }

    /*
    *   StartEffect - Start the slow effect.
    */
    public override void StartEffect(){

    }

    /*
    *   EndEffect - End the slow effect.
    */
    public override void EndEffect(){

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