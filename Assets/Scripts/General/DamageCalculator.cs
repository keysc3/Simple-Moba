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
    public static float CalculateDamage(float incomingDamage, DamageType damageType, UnitStats from, UnitStats to){
        if(incomingDamage <= 0f)
            return 0f;
        float finalDamage = incomingDamage;
        if(damageType == DamageType.Magic)
            finalDamage = MitigateMagicDamage(incomingDamage, to);
        else if(damageType == DamageType.Physical)
            finalDamage = MitigatePhysicalDamage(incomingDamage, to);
        Debug.Log($"Dealing {finalDamage} of {incomingDamage} incoming damage.");
        return finalDamage;
    }

    /*
    *   MitigateMagicDamage - Reduces the incoming magic damage based on the champions stats.
    *   @param incomingDamage - float of the magic damage to mitigate.
    *   @param from - UnitStats whose magic resist stat to use for mitigating.
    */
    private static float MitigateMagicDamage(float incomingDamage, UnitStats to){
        float finalDamage = incomingDamage * (100/(100 + to.magicResist.GetValue()));
        return finalDamage;
    }

    /*
    *   MitigatePhysicalDamage - Reduces the incoming physical damage based on the champions stats.
    *   @param incomingDamage - float of the physical damage to mitigate.
    *   @param from - UnitStats whose armor stat to use for mitigating.
    */
    private static float MitigatePhysicalDamage(float incomingDamage, UnitStats to){
        float finalDamage = incomingDamage * (100/(100 + to.armor.GetValue()));
        return finalDamage;
    }
}
