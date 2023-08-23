using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a generic units stats.
*
* @author: Colin Keys
*/
[System.Serializable]
public class UnitStats
{
    private float currentHealth;
    public float CurrentHealth { 
        get => currentHealth;
        set => currentHealth = value < maxHealth.GetValue() ? value : maxHealth.GetValue(); 
    }
    public Stat maxHealth { get; }
    public Stat magicDamage { get; }
    public Stat physicalDamage { get; }
    public Stat HP5 { get; }
    public Stat armor { get; }
    public Stat magicResist { get; }
    public Stat speed { get; }
    public Stat autoRange { get; }
    public Stat autoWindUp { get; }
    public Stat attackSpeed { get; }
    public Stat attackProjectileSpeed { get; }
    public Stat bonusAttackSpeed { get; }
    public Stat haste { get; }

    public UnitStats(ScriptableUnit unit){
        magicDamage = new Stat(unit.magicDamage);
        physicalDamage = new Stat(unit.physicalDamage);
        maxHealth = new Stat(unit.baseHealth);
        HP5 = new Stat(unit.HP5);
        armor = new Stat(unit.armor);
        magicResist = new Stat(unit.magicResist);
        speed = new Stat(unit.speed);
        autoRange = new Stat(unit.autoRange);
        autoWindUp = new Stat(unit.autoWindUp);
        attackSpeed = new Stat(unit.attackSpeed);
        attackProjectileSpeed = new Stat(unit.attackProjectileSpeed);
        bonusAttackSpeed = new Stat(0f);
        haste = new Stat(0f);
        currentHealth = maxHealth.GetValue();
    }

    /*
    *   ResetHealth - Set the champions current health value to the max health value.
    */
    public void ResetHealth(){
        currentHealth = maxHealth.GetValue();
    }
    
    /*
    *   UpdateAttackSpeed - Updates a units attack speed.
    */
    public void UpdateAttackSpeed(){
        float finalAS = attackSpeed.BaseValue * (1 + (bonusAttackSpeed.GetValue()/100));
        if(finalAS > 2.5f)
            finalAS = 2.5f;
        float modifier = finalAS - attackSpeed.BaseValue;
        attackSpeed.ClearModifiers();
        attackSpeed.AddModifier(modifier);
    }

    /*
    *   CalculateMoveSpeed - Calculates a units move speed. All speed boosts are used but only one slow is used.
    */
    public float CalculateMoveSpeed(NewStatusEffects statusEffects){
        List<Effect> speedBonuses = statusEffects.GetEffectsByType(typeof(ScriptableSpeedBonus));
        float additive = 1f;
        float multiplicative = 1f;
        // Calculate the additive and multiplicative speed boosts.
        foreach(Effect effect in speedBonuses){
            ScriptableSpeedBonus myBonus = (ScriptableSpeedBonus) effect.effectType;
            if(myBonus.isAdditive){
                additive += ((SpeedBonus) effect).BonusPercent;
            }
            else{
                multiplicative *= (1f + ((SpeedBonus) effect).BonusPercent);
            }
        }
        List<Effect> slows = statusEffects.GetEffectsByType(typeof(ScriptableSlow));
        float slowPercent = 1f;
        // Calculate the slow percentage to apply to the units speed.
        foreach(Effect effect in slows){
            if(effect.IsActivated){
                Slow mySlow = (Slow) effect;
                slowPercent *= (1f - mySlow.SlowPercent);
                break;
            }
        }
        // Calculate final value.
        float finalMS = speed.GetValue() * additive * multiplicative * slowPercent;
        return finalMS;
    }
}
