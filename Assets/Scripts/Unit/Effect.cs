using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Abstract class for effects.
*
* @author: Colin Keys
*/
[System.Serializable]
public abstract class Effect
{
    [field: SerializeField] public bool isFinished { get; private set; }
    [field: SerializeField] public ScriptableEffect effectType { get; private set; }
    [field: SerializeField] public GameObject casted { get; private set; } 
    
    [field: SerializeField] protected GameObject effected;
    [field: SerializeField] protected Collider effectedCollider;

    [field: SerializeField] public float effectTimer { get; private set; } = 0f;
    [field: SerializeField] public float effectDuration { get; private set; }
    [field: SerializeField] public bool isActivated { get; private set; } = false;
    //private bool isActivated;

    /*
    *   Effect - Initialize a new Effect object.
    *   @param effect - ScriptableObject of the effect to initialize.
    *   @param duration - float of the duration for the effect to last.
    *   @param unitCasted - GamObject of the unit that caused the effect.
    *   @param unitEffected - GameObject of the unit that is being affected by the effect.
    */
    public Effect(ScriptableEffect effect, float duration, GameObject unitCasted, GameObject unitEffected){
        casted =  unitCasted;
        effected = unitEffected;
        effectDuration = duration;
        isFinished = false;
        //isActivated = false;
        effectType = effect;
        effectedCollider = unitEffected.GetComponent<Collider>();
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
        Debug.Log("EffectTimer: " + effectTimer + " " + "EffectDuration: " + effectDuration);
        if(effectTimer <= effectDuration){
            effectTimer += delta;
        }
        else{
            Debug.Log("EffectTimer: " + effectTimer + " " + "EffectDuration: " + effectDuration);
            isFinished = true;
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

    /*
    *   SetDuration - Sets the effects duration
    */
    public void SetDuration(float effectDuration){
        this.effectDuration = effectDuration;
    }
    /*
    *   SetIsActivated - Sets the effect to start or stop.
    *   @param isActivated - bool of whether to activate or deactivate the effect.
    */
    public void SetIsActivated(bool isActivated){
        if(this.isActivated != isActivated){
            this.isActivated = isActivated;
            if(isActivated == false)
                EndEffect();
            else
                StartEffect();
        }
    }

    /*
    *   SetIsFinished - Set the effects finished bool.
    *   @param isFinished - bool to set the effects finished bool to.
    */
    public void SetIsFinished(bool isFinished){
        this.isFinished = isFinished;
    }
}
