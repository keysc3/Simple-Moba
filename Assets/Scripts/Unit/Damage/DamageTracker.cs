using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements damage tracking for assist granting and damage recap.
*
* @author: Colin Keys
*/
public class DamageTracker
{
    public List<Damage> damageReceived { get; } = new List<Damage>();

    private float resetTime = 15f; 
    private float assistTime = 10.0f;

    /*
    *   CheckForReset - Checks if the last instance of damage is greater than the reset damage received timer.
    */
    public void CheckForReset(float currentTime){
        if(damageReceived.Count > 0){
            // No damage received for 15s, remove all damage tracking.
            if(currentTime - damageReceived[damageReceived.Count-1].time >= resetTime)
                ResetDamageTracker();
        }
    }

    /*
    *   AddDamage - Adds a new Damage object to the damage tracker list.
    *   @param damageDealer - IUnit of the damage source.
    *   @param damageAmount - float of the amount of damage recieved.
    *   @param damageType - string of the type of damage.
    */
    public void AddDamage(IUnit damageDealer, float damageAmount, string damageType){
        damageReceived.Add(new Damage(damageDealer, damageAmount, damageType));
    }

    /*
    *   CheckForAssists - Checks if there are any eligible GameObjects to receive an assist.
    *   @param killer - IPlayer of the killing unit.
    *   @param currentTime - float of the current game time.
    *   @return List<GameObject> - List of GameObjects containing any damage dealers to this GameObject in the last 10s.
    */
    public List<IPlayer> CheckForAssists(IUnit killer, float currentTime){
        List<IPlayer> assisters = new List<IPlayer>();
        foreach(Damage damage in damageReceived){
            if(damage.from != killer && damage.from is IPlayer && !assisters.Contains((IPlayer) damage.from)){
                if(currentTime - damage.time <= assistTime){
                    assisters.Add((IPlayer) damage.from);
                } 
            }
        }
        return assisters;
    }

    /*
    *   ResetDamageTracker - Clears the damage tracker list.
    */
    public void ResetDamageTracker(){
        damageReceived.Clear();
    }
}
