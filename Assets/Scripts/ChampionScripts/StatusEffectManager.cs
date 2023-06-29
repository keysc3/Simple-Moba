using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Manages the buffs and debuffs on a unit.
*
* @author: Colin Keys
*/

public class StatusEffectManager : MonoBehaviour
{

    public List<Effect> statusEffects { get; private set; } = new List<Effect>();
    private UnitStats unitStats;
    //public List<string> effectNames { get; private set; } = new List<string>();

    private void Awake(){
        unitStats = gameObject.GetComponent<UnitStats>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(statusEffects.Count > 0){
            for(int i = statusEffects.Count - 1; i >= 0; i--){
                statusEffects[i].EffectTick(Time.deltaTime);
                // Avoid any indexing errors if unit dies from the effect tick.
                if(!unitStats.isDead){
                    if(statusEffects[i].isFinished){
                        statusEffects[i].EndEffect();
                        statusEffects.RemoveAt(i);
                        //effectNames.RemoveAt(i);
                    }
                }
            }
        }
    }

    //TODO: Apply most impairing cc effect, the others have their duration tick but not their effects.
    public void AddEffect(Effect effect){
        if(statusEffects.Count > 0){
            for(int i = 0; i < statusEffects.Count; i++){
                if(statusEffects[i].effectType.name == effect.effectType.name){
                    statusEffects[i].OverrideEffect(effect.casted);
                    return;
                }
            }
        }
        statusEffects.Add(effect);
        effect.StartEffect();
    }

    /*
    *   ResetEffects - Resets all effects currently on the unit.
    */
    public void ResetEffects(){
        statusEffects.Clear();
        //effectNames.Clear();
    }

    public bool CheckForEffect(ScriptableObject checkFor, GameObject source){
        foreach(Effect effect in statusEffects){
            if(effect.casted == source && effect.effectType == checkFor)
                return true;
        }
        return false;
    }
}
