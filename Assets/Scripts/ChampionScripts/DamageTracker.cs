using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements damage tracking for assist granting and damage recap.
*
* @author: Colin Keys
*/
[System.Serializable]
public class DamageTracker
{
    [field: SerializeField] public List<Damage> damageReceived { get; private set; }
    private float resetTime = 15f; 
    private float assistTime = 10.0f;
    //public float lastDamageReceived;

    public DamageTracker(){
        damageReceived = new List<Damage>();
    }

    /*
    *   CheckForReset - Checks if the last instance of damage is greater than the reset damage received timer.
    */
    public void CheckForReset(float currentTime){
        // No damage received for 15s, remove all damage tracking.
        if(currentTime - damageReceived[damageReceived.Count-1].time >= resetTime)
            ResetDamageTracker();
    }

    /*
    *   AddDamage - Adds a new Damage object to the damage tracker list.
    *   @param damageDealer - GameObject of the damage source.
    *   @param damageAmount - float of the amount of damage recieved.
    *   @param damageType - string of the type of damage.
    */
    public void AddDamage(GameObject damageDealer, float damageAmount, string damageType){
        damageReceived.Add(new Damage(damageDealer, damageAmount, damageType));
        //lastDamageReceived = Time.time;
    }

    /*
    *   CheckForAssists - Checks if there are any eligible GameObjects to receive an assist.
    *   @return List<GameObject> - List of GameObjects containing any damage dealers to this GameObject in the last 10s.
    */
    public List<GameObject> CheckForAssists(){
        List<GameObject> assist = new List<GameObject>();
        foreach(Damage damage in damageReceived){
            if(Time.time - damage.time <= assistTime && !assist.Contains(damage.from)){
                assist.Add(damage.from);
            } 
        }
        return assist;
    }

    /*
    *   ResetDamageTracker - Clears the damage tracker list.
    */
    public void ResetDamageTracker(){
        damageReceived.Clear();
    }
}
