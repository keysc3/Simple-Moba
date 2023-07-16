using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a status effects object..
*
* @author: Colin Keys
*/
public class StatusEffects
{

    public List<string> effectNames = new List<string>();
    public List<Effect> statusEffects { get; private set; } = new List<Effect>();
    private int highestActiveCCValue = 0;
    private Effect mostImpairing;
    public Unit unit;

    public StatusEffects(Unit unit){
        this.unit = unit;
    }

    /*
    *   UpdateEffects - Ticks the effects.
    */
    public void UpdateEffects(){
        // Increment every effects timer.
        for(int i = statusEffects.Count - 1; i >= 0; i--){
            // If there is an effect, used here incase the user dies from an effect tick that isn't the last effect.
            if(statusEffects.Count > 0){
                statusEffects[i].TimerTick(Time.deltaTime);
                // Avoid any indexing errors if unit dies from the effect tick.
                if(!unit.isDead){
                    if(statusEffects[i].isFinished){
                        Effect effect = statusEffects[i];
                        statusEffects[i].EndEffect();
                        statusEffects.RemoveAt(i);
                        effectNames.RemoveAt(i);
                        // If effect was a slow find the new strongest slow if another exists.
                        if(effect is Slow){
                            if(CheckForEffectByType(effect)){
                                SetStrongestSlow((Slow) effect);
                            }
                        }
                        // If there are still running effects, activate the most impairing.
                        if(statusEffects.Count > 0){
                            SetMostImpairing(GetMostImpairing());
                        }
                        else
                            highestActiveCCValue = 0;
                    }
                }
                else
                    highestActiveCCValue = 0;
            }
        }
    }

    /*
    *   AddEffect - Adds an effect to the status managers effect list.
    *   @param effect - Effect to add to the status effects list.
    */
    public void AddEffect(Effect effect){
        // If there is an existing effect, only the new one if it is more impairing than the current most.
        if(statusEffects.Count > 0){
            if(effect.effectType.ccValue > highestActiveCCValue){
                // Only deactivate non-zero cc values.
                if(highestActiveCCValue > 0)
                    mostImpairing.SetIsActivated(false);
                SetMostImpairing(effect);
            }
            else{
                for(int i = 0; i < statusEffects.Count; i++){
                    // Reset effect variables if the same effect hit them and it is not stackable.
                    if(statusEffects[i].effectType.name == effect.effectType.name && !effect.effectType.isStackable){
                        statusEffects[i].OverrideEffect(effect.casted);
                        return;
                    }
                }
            }
        }
        else{
            SetMostImpairing(effect);
        }
        statusEffects.Add(effect);
        effectNames.Add(effect.effectType.name);
        // CC Values of zero are always active.
        if(effect.effectType.ccValue == 0){
            // If a new slow effect was added then only activate the strongest one.
            if(effect is Slow){
                SetStrongestSlow((Slow) effect);
            }
            else
                effect.SetIsActivated(true);
        }
        // If the effect is a slow and a child of another effect then do not add it to the UI.
        if(effect is Slow){
            if(((ScriptableSlow) effect.effectType).isChild){
                return;
            }
        }
        if(unit.unit is ScriptableChampion)
            UIManager.instance.AddStatusEffectUI(this, effect, ((Player) unit).playerUI);
    }

    /*
    *   SetStrongestSlow - Sets the strongest slow to be activated. Slow is the only zero cc value effect that applies the strongest.
    *   @param effect - Slow Effect that was added.
    */
    public void SetStrongestSlow(Slow effect){
        // Get the strongest slows index in the status effect list.
        int index = effect.GetStrongest(statusEffects);
        // Deactivate all slows in the list that aren't the strongest.
        for(int i = 0; i < statusEffects.Count; i++){
            if(statusEffects[i] is Slow){
                if(index == i)
                    statusEffects[i].SetIsActivated(true);
                else
                    statusEffects[i].SetIsActivated(false);
            } 
        }
    }

    /*
    *   ResetEffects - Resets all effects currently on the unit.
    */
    public void ResetEffects(){
        statusEffects.Clear();
    }

    /*
    *   CheckForEffectWithSource - Checks for the given effect from the source in the status managers effect list.
    *   @param checkFor - ScriptableObject of the effect to check for.
    *   @param source - GameObject of the source of the effect.
    *   @return bool - bool of whether or not the effect exists on this GameObject.
    */
    public bool CheckForEffectWithSource(ScriptableEffect checkFor, GameObject source){
        foreach(Effect effect in statusEffects){
            if(effect.casted == source && effect.effectType.GetType() == checkFor.GetType())
                return true;
        }
        return false;
    }

    /*
    *   CheckForEffectByName - Checks for the given effect in the status managers effect list with given name.
    *   @param checkFor - ScriptableObject of the effect to check for.
    *   @return bool - bool of whether or not the effect exists on this GameObject.
    */
    public bool CheckForEffectByName(ScriptableEffect checkFor, string effectName){
        foreach(Effect effect in statusEffects){
            if(effect.effectType.GetType() == checkFor.GetType() && effect.effectType.name == effectName)
                return true;
        }
        return false;
    }

    /*
    *   CheckForEffectByType - Checks for the given effect in the status managers effect list with given type.
    *   @param checkFor - Effect of the effect type to check for.
    *   @return bool - bool of whether or not the effect exists on this GameObject.
    */
    public bool CheckForEffectByType(Effect checkFor){
        foreach(Effect effect in statusEffects){
            if(effect.effectType.GetType() == checkFor.effectType.GetType())
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
        float timeTillExpired = myEffects[0].effectDuration - myEffects[0].effectTimer;
        if(myEffects.Count > 1){
            for(int i = 1; i < myEffects.Count; i++){
                // If duration left is less than the current timeTillExpired then set the new next expiring.
                float check = myEffects[i].effectDuration - myEffects[i].effectTimer;
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
    public Effect GetMostImpairing(){
        int index = 0;
        int highestCC = 0;
        // Check for highest CC value effect.
        for(int i = 0; i < statusEffects.Count; i++){
            // Don't check values of 0.
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
        }
        return statusEffects[index];
    }

    /*
    *   SetMostImpairing - Activates and sets the tracking variables for the most impairing status effect in the list.
    *   @param effect - Effect of the status effect to activate and set.
    */
    public void SetMostImpairing(Effect effect){
        effect.SetIsActivated(true);
        mostImpairing = effect;
        highestActiveCCValue = effect.effectType.ccValue;
    }

    /*
    *   RemoveEffect - Removes an effect from the status effect list.
    *   @param effectType - ScriptableEffect of the status effect remove.
    *   @param casted - GameObject of the effects caster.
    */
    public void RemoveEffect(ScriptableEffect effectType, GameObject casted){
        for(int i = 0; i < statusEffects.Count; i++){
            if(effectType.GetType() == statusEffects[i].effectType.GetType() && casted == statusEffects[i].casted){
                statusEffects[i].SetIsActivated(false);
                statusEffects[i].EndEffect();
                statusEffects.RemoveAt(i);
                effectNames.RemoveAt(i);
                SetMostImpairing(GetMostImpairing());
                //effect.SetIsFinished(true);
                return;
            }
        }
    }
}
