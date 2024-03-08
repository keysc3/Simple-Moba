using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Player
{
    private float timeToResetHealth = 10f;

    public override void TakeDamage(float incomingDamage, DamageType damageType, IUnit damager, bool isDot){
        float damageToTake = DamageCalculator.CalculateDamage(incomingDamage, damageType, damager.unitStats, unitStats);
        unitStats.CurrentHealth = unitStats.CurrentHealth - damageToTake;
        bonusDamage?.Invoke(this, isDot);
        if(unitStats.CurrentHealth <= 1f){
            unitStats.CurrentHealth = 1f;
        }
        if(damageTracker != null){
            damageTracker.AddDamage(damager, damageToTake, damageType);
        }  
    }

    protected override void Update(){
        statusEffects.UpdateEffects(Time.deltaTime);
        if(damageTracker != null){
            if(damageTracker.TimeSinceLastDamage(Time.time) >= timeToResetHealth)
                unitStats.ResetHealth();
        }
    }
}
