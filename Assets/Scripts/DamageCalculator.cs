using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements final damage calculations.
*
* @author: Colin Keys
*/
public static class DamageCalculator
{

    /*
    *   CalculateDamage - Calculates the final damage based on the taking and dealing units mitigation stats.
    *   @param incomingDamage - float of the incoming damage amount.
    *   @param damageType - string of the type of damage that is being inflicted.
    *   @param from - GameObject of the damage source.
    *   @param to - GameObject the damage is going to.
    */
    public static float CalculateDamage(float incomingDamage, string damageType, GameObject from, GameObject to){
        float finalDamage = incomingDamage;
        if(damageType == "magic")
            finalDamage = MitigateMagicDamage(incomingDamage);
        else if(damageType == "physical")
            finalDamage = MitigatePhysicalDamage(incomingDamage);
        return finalDamage;
    }

    /*
    *   MitigateMagicDamage - Reduces the incoming magic damage based on the champions stats.
    */
    private static float MitigateMagicDamage(float incomingDamage){
        //TODO: mitigate
        return incomingDamage;
    }

    /*
    *   MitigatePhysicalDamage - Reduces the incoming physical damage based on the champions stats.
    */
    private static float MitigatePhysicalDamage(float incomingDamage){
        //TODO: mitigate
        return incomingDamage;
    }
}
