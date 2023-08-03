using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a stat.
*
* @author: Colin Keys
*/
[System.Serializable]
public class Stat {
    
    [field: SerializeField] public float BaseValue { get; set; }

    [SerializeField] private List<float> modifiers = new List<float>();

    /*
    *   Stat - Initializes a new stat object.
    *   @param value - float of the stats base value.
    */
    public Stat(float value){
        BaseValue = value;
    }
    
    /*
    *   GetValue - Gets the current value of the stat.
    */
    public float GetValue(){
        float totalValue = BaseValue;
        foreach(float modifier in modifiers)
            totalValue += modifier;
        return totalValue;
    }

    /*
    *   GetBaseValue - Gets the base value of the stat.
    */
    /*public float GetBaseValue(){
        return baseValue;
    }*/

    /*
    *   SetBaseValue - Sets the base value of the stat to a new value.
    *   @param newBaseValue - float of the value to change the base value to.
    */
    /*public void SetBaseValue(float newBaseValue){
        baseValue = newBaseValue;
    }*/

    /*
    *   AddModifier - Adds a modifier to the stats modifier list.
    *   @param modifier - float of the amount to modify the base value by.
    */
    public void AddModifier(float modifier){
        if(modifier != 0)
            modifiers.Add(modifier);
    }

    /*
    *   AddModifier - Removes a modifier frome the stats modifier list.
    *   @param modifier - float of the value to remove from the modifiers list.
    */
    public void RemoveModifier(float modifier){
        if(modifier != 0)
            modifiers.Remove(modifier);
    }

    /*
    *   ClearModifiers - Clears all modifiers from the stat.
    */
    public void ClearModifiers(){
        modifiers.Clear();
    }
}
