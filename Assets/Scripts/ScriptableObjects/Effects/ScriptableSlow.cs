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


    public void SetDuration(float duration){
        this.duration = duration;
    }

    public bool isEffectStronger(ScriptableSlow strongest, ScriptableSlow newSlow){
        if(newSlow.slowPercent > strongest.slowPercent)
            return true;
        else
            return false;
    }

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
