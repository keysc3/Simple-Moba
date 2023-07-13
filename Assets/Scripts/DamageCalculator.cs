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
    public static float CalculateDamage(float incomingDamage, string damageType, Unit from, Unit to){
        float finalDamage = incomingDamage;
        if(damageType == "magic")
            finalDamage = MitigateMagicDamage(incomingDamage, to);
        else if(damageType == "physical")
            finalDamage = MitigatePhysicalDamage(incomingDamage, to);
        return finalDamage;
    }

    /*
    *   MitigateMagicDamage - Reduces the incoming magic damage based on the champions stats.
    *   @param incomingDamage - float of the magic damage to mitigate.
    *   @param from - Unit whose magic resist stat to use for mitigating.
    */
    private static float MitigateMagicDamage(float incomingDamage, Unit to){
        float finalDamage = incomingDamage * (100/(100 + to.magicResist));
        return finalDamage;
    }

    /*
    *   MitigatePhysicalDamage - Reduces the incoming physical damage based on the champions stats.
    *   @param incomingDamage - float of the physical damage to mitigate.
    *   @param from - Unit whose armor stat to use for mitigating.
    */
    private static float MitigatePhysicalDamage(float incomingDamage, Unit to){
        float finalDamage = incomingDamage * (100/(100 + to.armor));
        return finalDamage;
    }
}
