using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements a generic units stats.
*
* @author: Colin Keys
*/
[System.Serializable]
public class UnitStats
{
    [field: SerializeField] public float currentHealth { get; private set; }
    [field: SerializeField] public float displayCurrentHealth { get; protected set; }
    [field: SerializeField] public Stat maxHealth { get; private set; }
    [field: SerializeField] public Stat magicDamage { get; private set; }
    [field: SerializeField] public Stat physicalDamage { get; private set; }
    [field: SerializeField] public Stat HP5 { get; private set; }
    [field: SerializeField] public Stat armor { get; private set; }
    [field: SerializeField] public Stat magicResist { get; private set; }
    [field: SerializeField] public Stat speed { get; private set; }
    [field: SerializeField] public Stat autoRange { get; private set; }
    [field: SerializeField] public Stat autoWindUp { get; private set; }
    [field: SerializeField] public Stat attackSpeed { get; private set; }
    [field: SerializeField] public Stat attackProjectileSpeed { get; private set; }
    [field: SerializeField] public Stat bonusAttackSpeed { get; private set; }
    [field: SerializeField] public ScriptableUnit unit { get; private set; }

    //private StatusEffectManager statusEffectManager;
    //private NavMeshAgent navMeshAgent;

    public UnitStats(ScriptableUnit unit){
        // Set player base player values
        this.unit = unit;
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
        currentHealth = maxHealth.GetValue();
    }

    /*
    *   SetHealth - Set the champions current health value.
    *   @param value - float of the value to change current health to.
    */
    public void SetHealth(float value){
        if(value <= maxHealth.GetValue())
            currentHealth = value;
        else
            ResetHealth();
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
        float finalAS = ((ScriptableChampion) unit).attackSpeed * (1 + (bonusAttackSpeed.GetValue()/100));
        if(finalAS > 2.5f)
            finalAS = 2.5f;
        attackSpeed.SetBaseValue(finalAS);
    }

    /*
    *   CalculateMoveSpeed - Calculates a units move speed. All speed boosts are used but only one slow is used.
    */
    public float CalculateMoveSpeed(StatusEffects statusEffects){
        List<Effect> speedBonuses = statusEffects.GetEffectsByType(typeof(ScriptableSpeedBonus));
        float additive = 1f;
        float multiplicative = 1f;
        // Calculate the additive and multiplicative speed boosts.
        foreach(Effect effect in speedBonuses){
            ScriptableSpeedBonus myBonus = (ScriptableSpeedBonus) effect.effectType;
            if(myBonus.isAdditive){
                additive += ((SpeedBonus) effect).bonusPercent;
            }
            else{
                multiplicative *= (1f + ((SpeedBonus) effect).bonusPercent);
            }
        }
        List<Effect> slows = statusEffects.GetEffectsByType(typeof(ScriptableSlow));
        float slowPercent = 1f;
        // Calculate the slow percentage to apply to the units speed.
        foreach(Effect effect in slows){
            if(effect.isActivated){
                //Debug.Log("champ: " + gameObject.name + "act?: " + effect.isActivated);
                Slow mySlow = (Slow) effect;
                slowPercent *= (1f - mySlow.slowPercent);
                break;
            }
        }
        // Calculate final value.
        float finalMS = speed.GetValue() * additive * multiplicative * slowPercent;
        return finalMS;
    }
}
