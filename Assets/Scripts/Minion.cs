using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Minion : Unit
{

    /*
    *   TakeDamage - Damages the unit.
    *   @param incomingDamage - float of the incoming damage amount.
    *   @param damageType - string of the type of damage that is being inflicted.
    *   @param from - GameObject of the damage source.
    *   @param isDot - bool if the damage was from a dot.
    */
    public override void TakeDamage(float incomingDamage, string damageType, GameObject from, bool isDot){
        Unit fromUnit = from.GetComponent<Unit>();
        float damageToTake = DamageCalculator.CalculateDamage(incomingDamage, damageType, fromUnit.unitStats, unitStats);
        unitStats.SetHealth(unitStats.currentHealth - damageToTake);
        Debug.Log(transform.name + " took " + damageToTake + " " + damageType + " damage from " + from.transform.name);
        // If dead then award a kill and start the death method.
        if(unitStats.currentHealth <= 0f){
            isDead = true;
            Death();
            if(fromUnit is Player){
                Player killer = (Player) fromUnit;
                killer.score.CreepKill(gameObject);
                UIManager.instance.UpdateCS(killer.score.cs.ToString(), killer.playerUI);
            }
        }
        // Apply any damage that procs after recieving damage.
        else{
            bonusDamage?.Invoke(gameObject, isDot);
        }
    }
}
