using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a damage class for creating objects to hold damage info.
*
* @author: Colin Keys
*/
[System.Serializable]
public class Damage
{

    [field: SerializeField] public float amount { get; private set; }
    [field: SerializeField] public float time { get; private set; }
    [field: SerializeField] public string type { get; private set; }
    [field: SerializeField] public GameObject from { get; private set; }

    /*
    *   Damage - Damage info object.
    *   @param damageDealer - GameObject of the unit that dealt damage.
    *   @param damageAmount - float of the damage amount.
    *   @param damageType - string of the damage type.
    */
    public Damage(GameObject damageDealer, float damageAmount, string damageType){
        from = damageDealer;
        amount = damageAmount;
        type = damageType;
        time = Time.time;
    }
}
