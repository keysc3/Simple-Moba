using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Abstract class for effects.
*
* @author: Colin Keys
*/
public abstract class Effect
{
    public bool isFinished { get; private set; }
    public ScriptableEffect effectType { get; }
    public GameObject casted { get; private set; } 
    public GameObject effected { get; }
    public float effectTimer { get; private set; } = 0f;
    public float effectDuration { get; set; }
    private bool isActivated;
    #region "IsActivated property"
    public bool IsActivated { 
        get {
            return isActivated;
        }
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
    #endregion

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
        effectDuration = duration;
        isFinished = false;
        effectType = effect;
    }

    /*
    *   StartEffect - Start the effect.
    */
    public abstract void StartEffect();

    /*
    *   EndEffect - End the effect.
    */
    public abstract void EndEffect();

    /*
    *   TimerTick - Handles the effects duration.
    *   @param delta - float of the time passed since the last tick.
    */
    public virtual void TimerTick(float delta){
        if(isActivated)
            EffectTick();
        // Persistent effects have duration -1.
        if(effectDuration != -1f){
            if(effectTimer <= effectDuration){
                Debug.Log("EffectTimer: " + effectTimer + " " + "EffectDuration: " + effectDuration);
                effectTimer += delta;
            }
            else{
                Debug.Log("EffectTimer: " + effectTimer + " " + "EffectDuration: " + effectDuration);
                isFinished = true;
            }
        }
    }

    /*
    *   EffectTick - Applies a tick of the effect. Used for effects that need to override.
    */
    public virtual void EffectTick(){
        // Place holder.
    }

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
