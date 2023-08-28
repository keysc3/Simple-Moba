using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Class for effects.
*
* @author: Colin Keys
*/
public class Effect
{
    public bool isFinished { get; private set; }
    public ScriptableEffect effectType { get; }
    public GameObject casted { get; private set; } 
    public GameObject effected { get; }
    public float effectTimer { get; private set; } = 0f;
    protected float effectDuration;
    public float EffectDuration { 
        get => effectDuration;
        set {
            if(value == -1f || value >= 0f)
                effectDuration = value;
        }
    }
    private bool isActivated;
    public bool IsActivated { 
        get => isActivated;
        set {
            if(value != isActivated){
                isActivated = value;
                if(value == false)
                    EndEffect();
                else
                    StartEffect();
            }
        }
    }

    /*
    *   Effect - Initialize a new Effect object.
    *   @param effect - ScriptableObject of the effect to initialize.
    *   @param duration - float of the duration for the effect to last.
    *   @param unitCasted - GameObject of the unit that caused the effect.
    *   @param unitEffected - GameObject of the unit that is being affected by the effect.
    */
    public Effect(ScriptableEffect effect, float duration, GameObject unitCasted, GameObject unitEffected){
        casted =  unitCasted;
        effected = unitEffected;
        EffectDuration = duration;
        isFinished = false;
        effectType = effect;
    }

    /*
    *   StartEffect - Start the effect.
    */
    public virtual void StartEffect(){}

    /*
    *   EndEffect - End the effect.
    */
    public virtual void EndEffect(){}

    /*
    *   TimerTick - Handles the effects duration.
    *   @param delta - float of the time passed since the last tick.
    */
    public virtual void TimerTick(float delta){
        if(isActivated)
            EffectTick();
        // Persistent effects have duration -1.
        if(effectDuration != -1f){
            effectTimer += delta;
            Debug.Log("EffectTimer: " + effectTimer + " " + "EffectDuration: " + effectDuration);
            if(effectTimer >= effectDuration){
                if(isActivated)
                    EndEffect();
                isFinished = true;
            }
        }
    }

    /*
    *   EffectTick - Applies a tick of the effect.
    */
    public virtual void EffectTick(){}

    /*
    *   OverrideEffect - Resets the effects timer to 0 and sets the new source.
    *   @param source - GameObject of the caster.
    */
    public virtual void OverrideEffect(GameObject source){
        effectTimer = 0f;
        isFinished = false;
        casted = source;
    }

    /*
    *   ResetTimer - Resets the effects timer.
    */
    public void ResetTimer(){
        effectTimer = 0.0f;
    }
}
