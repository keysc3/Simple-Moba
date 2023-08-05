using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a stat.
*
* @author: Colin Keys
*/
public class Stat {
    
    private float baseValue = 0f;
    #region "BaseValue property"
    public float BaseValue { 
        get {
            return baseValue;
        } 
        set{
            if(value > 0f)
                baseValue = value;
        }
    }
    #endregion
    private List<float> modifiers = new List<float>();

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
