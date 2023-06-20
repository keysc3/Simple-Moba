using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements damage tracking for assist granting and damage recap.
*
* @author: Colin Keys
*/
[System.Serializable]
public class DamageTracker : MonoBehaviour
{
    [field: SerializeField] public List<Damage> damageReceived = new List<Damage>(); 
    //public float lastDamageReceived;

    // Update is called once per frame
    void Update()
    {
        // No damage received for 15s, remove all damage tracking.
        if(damageReceived.Count > 0){
            if(Time.time - damageReceived[damageReceived.Count-1].time >= 15f)
                ResetDamageTracker();
        }
    }

    /*
    *   DamageTaken - Adds a new Damage object to the damage tracker list.
    *   @param damageDealer - GameObject of the damage source.
    *   @param damageAmount - float of the amount of damage recieved.
    *   @param damageType - string of the type of damage.
    */
    public void DamageTaken(GameObject damageDealer, float damageAmount, string damageType){
        damageReceived.Add(new Damage(damageDealer, damageAmount, damageType));
        //lastDamageReceived = Time.time;
    }

    /*
    *   CheckForAssists - Checks if there are any eligible GameObjects to receive an assist.
    *   @return List<GameObject> - List of GameObjects containing any damage dealers to this GameObejct in the last 10s.
    */
    public List<GameObject> CheckForAssists(){
        List<GameObject> assist = new List<GameObject>();
        foreach(Damage damage in damageReceived){
            if(Time.time - damage.time <= 10f && !assist.Contains(damage.from)){
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
