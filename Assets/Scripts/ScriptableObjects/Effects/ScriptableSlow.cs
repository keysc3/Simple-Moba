using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Slow")]
public class ScriptableSlow : ScriptableEffect
{
    [field: SerializeField] public float duration { get; private set; }
    [field: SerializeField] public float slowPercent { get; private set; }

    /*
    *   InitializeEffect - Initializes a new slow effect with the duration and slow percent.
    *   @param unitCasted - GameObject of the unit that casted the slow.
    *   @param unitEffected - GameObject of the unit effected by the slow.
    */
    public Effect InitializeEffect(GameObject unitCasted, GameObject unitEffected){
        ccValue = 0;
        return new Slow(this, duration, unitCasted, unitEffected);
    }

    /*
    *   SetDuration - Sets the slows duration.
    *   @param duration - float of duration to set slow to.
    */
    public void SetDuration(float duration){
        this.duration = duration;
    }

    /*
    *   isEffectStronger - Checks if the the second effect is stronger than the first.
    *   @param strongest - ScriptableSlow of a slow.
    *   @param newSlow - ScriptableSlow of the slow to check if stronger.
    */
    public bool isEffectStronger(ScriptableSlow strongest, ScriptableSlow newSlow){
        if(newSlow.slowPercent > strongest.slowPercent)
            return true;
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
            if(effect.effectType is ScriptableSlow){
                if(strongestIndex != -1){
                    if(isEffectStronger((ScriptableSlow) myEffects[strongestIndex].effectType, (ScriptableSlow) effect.effectType))
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
