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
    //[field: SerializeField] public string effectName { get; private set; }
    public bool isFinished { get; private set; }
    public ScriptableEffect effectType { get; private set; }
    public GameObject casted { get; private set; } 
    
    protected GameObject effected;
    protected Collider effectedCollider;

    private float effectTimer = 0f;
    protected float effectDuration;
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
    *   EffetTick - Handles the effects duration.
    *   @param delta - float of the time passed since the last tick.
    */
    public virtual void EffectTick(float delta){
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
    *   OverrideEffect - Resets the effects timer to 0 and sets the new source.
    *   @param source - GameObject of the caster.
    */
    public virtual void OverrideEffect(GameObject source){
        effectTimer = 0f;
        isFinished = false;
        casted = source;
    }
}
