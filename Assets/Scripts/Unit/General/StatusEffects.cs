using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a status effects object.
*
* @author: Colin Keys
*/
public class StatusEffects
{
    public List<Effect> statusEffects { get; } = new List<Effect>();
    
    private int highestActiveCCValue = 0;
    private Effect mostImpairing;

    public delegate void OnTimerTick(Effect effect);
    public event OnTimerTick OnDurationUpdate;

    public delegate void OnEffectAdded(Effect effect);
    public event OnEffectAdded EffectAdded;
    
    /*
    *   UpdateEffects - Ticks the effects.
    *   @param delta - float of the interval since the last frame.
    */
    public void UpdateEffects(float delta){
        // Increment every effects timer.
        for(int i = statusEffects.Count - 1; i >= 0; i--){
            // If there is an effect or invalid index, used here incase the user dies from an effect tick that isn't the last effect.
            if(statusEffects.Count > 0 && statusEffects.Count >= i - 1){
                Effect effectUpdating = statusEffects[i];
                statusEffects[i].TimerTick(delta);
                // Check if the effect still exists in the case that the unit died from the effect.
                if(statusEffects.Contains(effectUpdating)){
                    if(statusEffects[i].isFinished){
                        Effect effect = statusEffects[i];
                        statusEffects.RemoveAt(i);
                        // If effect was a slow find the new strongest slow if another exists.
                        if(effect is Slow){
                            if(CheckForEffectByType<Slow>()){
                                SetStrongestSlow((Slow) effect);
                            }
                        }
                        // If there are still running effects, activate the most impairing.
                        NewMostImpairing();
                    }
                }
                else
                    NewMostImpairing();
            }
        }
        OnDurationUpdate?.Invoke(mostImpairing);
    }

    /*
    *   AddEffect - Adds an effect to the status managers effect list.
    *   @param effect - Effect to add to the status effects list.
    */
    public void AddEffect(Effect effect){
        if(effect.effected.IsDead)
            return;
        // If there is an existing effect, only the new one if it is more impairing than the current most.
        if(statusEffects.Count > 0){
            if(effect.effectType.ccValue > highestActiveCCValue){
                // Only deactivate non-zero cc values.
                if(highestActiveCCValue > 0)
                    mostImpairing.IsActivated = false;
                SetMostImpairing(effect);
            }
            else{
                // Set most impairing if new effect is a keyword 0 cc value effect.
                if(highestActiveCCValue == 0 && mostImpairing.effectType.keyword == "Default" && effect.effectType.keyword != "Default")
                    SetMostImpairing(effect);
                for(int i = statusEffects.Count - 1; i >= 0; i--){
                    // If the same effect hit them and it is not stackable remove it for the new one.
                    if(statusEffects[i].effectType.name == effect.effectType.name && !effect.effectType.isStackable){
                        statusEffects[i].EndEffect();
                        statusEffects.RemoveAt(i);
                    }
                }
            }
        }
        else{
            SetMostImpairing(effect);
        }
        statusEffects.Add(effect);
        // CC Values of zero are always active.
        if(effect.effectType.ccValue == 0){
            // If a new slow effect was added then only activate the strongest one.
            if(effect is Slow){
                SetStrongestSlow((Slow) effect);
            }
            else
                effect.IsActivated = true;
        }
        // If the effect is a slow and a child of another effect then do not add it to the UI.
        if(effect is Slow){
            if(((ScriptableSlow) effect.effectType).isChild){
                return;
            }
        }
        // Add UI element.
        if(effect.effected != null && effect.effected is IPlayer)
            EffectAdded?.Invoke(effect);
    }

    /*
    *   SetStrongestSlow - Sets the strongest slow to be activated. Slow is the only zero cc value effect that applies the strongest.
    *   @param effect - Slow Effect that was added.
    */
    private void SetStrongestSlow(Slow effect){
        // Get the strongest slows index in the status effect list.
        int index = effect.GetStrongest(statusEffects);
        // Deactivate all slows in the list that aren't the strongest.
        for(int i = 0; i < statusEffects.Count; i++){
            if(statusEffects[i] is Slow){
                if(index == i)
                    statusEffects[i].IsActivated = true;
                else
                    statusEffects[i].IsActivated = false;
            } 
        }
    }

    /*
    *   ResetEffects - Resets all effects currently on the unit.
    */
    public void ResetEffects(){
        for(int i = statusEffects.Count - 1; i > -1; i--){
            if(!(statusEffects[i] is PersonalSpell))
                statusEffects.RemoveAt(i);
        }
        NewMostImpairing();
    }

    private void NewMostImpairing(){
        if(statusEffects.Count > 0)
            SetMostImpairing(GetMostImpairing());
        else{
            highestActiveCCValue = 0;
            mostImpairing = null;
        }
    }

    /*
    *   CheckForEffectWithSource - Checks for the given effect from the source in the status managers effect list.
    *   @param checkFor - ScriptableObject of the effect to check for.
    *   @param source - IUnit of the source of the effect.
    *   @return bool - bool of whether or not the effect exists on this GameObject.
    */
    public bool CheckForEffectWithSource(ScriptableEffect checkFor, IUnit source){
        foreach(Effect effect in statusEffects){
            if(effect.casted == source && effect.effectType == checkFor)
                return true;
        }
        return false;
    }

    /*
    *   CheckForEffectByName - Checks for the effect in the status managers effect list with given name.
    *   @param effectName - string of the effects name.
    *   @return bool - bool of whether or not the effect exists on this GameObject.
    */
    public bool CheckForEffectByName(string effectName){
        foreach(Effect effect in statusEffects){
            if(effect.effectType.name == effectName)
                return true;
        }
        return false;
    }

    /*
    *   CheckForEffectByType - Checks for the given effect in the status managers effect list with given type.
    *   @return bool - bool of whether or not the effect exists on this GameObject.
    */
    public bool CheckForEffectByType<T>() where T: Effect{
        foreach(Effect effect in statusEffects){
            if(effect is T)
                return true;
        }
        return false;
    }

    /*
    *   GetEffectsByType - Gets all effects matching the given type.
    *   @param type - Type of the effect type to search for.
    */
    public List<Effect> GetEffectsByType(System.Type type){
        List<Effect> effects = new List<Effect>();
        foreach(Effect effect in statusEffects){
            if(effect.effectType.GetType() == type)
                effects.Add(effect);
        }
        return effects;
    }

    /*
    *   GetEffectsByName - Gets all effects matching the given name.
    *   @param name - string of the name to search for.
    */
    public List<Effect> GetEffectsByName(string name){
        List<Effect> effects = new List<Effect>();
        foreach(Effect effect in statusEffects){
            if(effect.effectType.name == name)
                effects.Add(effect);
        }
        return effects;
    }

    /*
    *   GetNextExpiringStack - Gets the next expiring stack of a stackable effect.
    *   @param effect - Effect to get the next expiring stack for.
    */
    public Effect GetNextExpiringStack(Effect effect){
        List<Effect> myEffects = GetEffectsByName(effect.effectType.name);
        // Set default values.
        Effect nextExipiring = myEffects[0];
        float timeTillExpired = myEffects[0].EffectDuration - myEffects[0].effectTimer;
        if(myEffects.Count > 1){
            for(int i = 1; i < myEffects.Count; i++){
                // If duration left is less than the current timeTillExpired then set the new next expiring.
                float check = myEffects[i].EffectDuration - myEffects[i].effectTimer;
                if(check < timeTillExpired){
                    timeTillExpired = check;
                    nextExipiring = myEffects[i];
                }
            }
        }
        return nextExipiring;
    }

    /*
    *   GetMostImpairing - Gets the most impairing effect in the status manager effects list. Highest cc value = most impairing.
    *   @return Effect - Effect of the most impairing effect in the list.
    */
    private Effect GetMostImpairing(){
        int index = 0;
        int highestCC = 0;
        // Check for highest CC value effect.
        for(int i = 0; i < statusEffects.Count; i++){
            // Only compare cc values greater than 0.
            if(statusEffects[i].effectType.ccValue != 0){
                if(statusEffects[i].effectType.ccValue > highestCC){
                    index = i;
                    highestCC = statusEffects[i].effectType.ccValue;
                }
                // Prioritize oldest effect.
                else if(statusEffects[i].effectType.ccValue == highestCC){
                    if(statusEffects[i].effectTimer > statusEffects[index].effectTimer){
                        index = i;
                    }
                }
            }
            else{
                // Set most impairing to be a 0 cc value keyword effect if 0 is highest value.
                if(highestCC == 0 && statusEffects[i].effectType.keyword != "Default")
                    index = i;
            }
        }
        return statusEffects[index];
    }

    /*
    *   SetMostImpairing - Activates and sets the tracking variables for the most impairing status effect in the list.
    *   @param effect - Effect of the status effect to activate and set.
    */
    private void SetMostImpairing(Effect effect){
        effect.IsActivated = true;
        mostImpairing = effect;
        highestActiveCCValue = effect.effectType.ccValue;
        //OnActivated?.Invoke(effect.effectType.keyword);
    }

    /*
    *   RemoveEffect - Removes an effect from the status effect list.
    *   @param effectType - ScriptableEffect of the status effect remove.
    *   @param casted - IUnit of the effects caster.
    */
    public void RemoveEffect(ScriptableEffect effectType, IUnit casted){
        for(int i = 0; i < statusEffects.Count; i++){
            if(effectType.GetType() == statusEffects[i].effectType.GetType() && casted == statusEffects[i].casted){
                Effect removed = statusEffects[i];
                statusEffects[i].IsActivated = false;
                statusEffects[i].EndEffect();
                statusEffects.RemoveAt(i);
                if(removed == mostImpairing)
                    NewMostImpairing();
                return;
            }
        }
    }
}
