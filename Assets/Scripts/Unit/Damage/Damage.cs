using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a damage class for creating objects to hold damage info.
*
* @author: Colin Keys
*/
public class Damage
{
    public float amount { get; }
    public float time { get; }
    public string type { get; }
    public IUnit from { get; }

    /*
    *   Damage - Damage info object.
    *   @param damageDealer - IUnit of the unit that dealt damage.
    *   @param damageAmount - float of the damage amount.
    *   @param damageType - string of the damage type.
    */
    public Damage(IUnit damageDealer, float damageAmount, string damageType){
        from = damageDealer;
        amount = damageAmount;
        type = damageType;
        time = Time.time;
    }
}
