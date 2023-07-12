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
public class UnitStats : MonoBehaviour
{
    [field: SerializeField] public bool isDead { get; protected set; }
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
    [field: SerializeField] public Unit unit { get; private set; }

    private StatusEffectManager statusEffectManager;
    private NavMeshAgent navMeshAgent;

    public delegate void BonusDamage(GameObject toDamage, bool isDot); 
    public BonusDamage bonusDamage;

    protected virtual void Awake(){
        // Set player base player values
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
        isDead = false;
        currentHealth = maxHealth.GetValue();
        statusEffectManager = GetComponent<StatusEffectManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
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

    public void SetDeathStatus(bool dead){
        isDead = dead;
    }

    /*
    *   TakeDamage - Damages the unit.
    *   @param incomingDamage - float of the incoming damage amount.
    *   @param damageType - string of the type of damage that is being inflicted.
    *   @param from - GameObject of the damage source.
    */
    public virtual void TakeDamage(float incomingDamage, string damageType, GameObject from, bool isDot){
        float damageToTake = DamageCalculator.CalculateDamage(incomingDamage, damageType, from, gameObject);
        currentHealth -= damageToTake;
        Debug.Log(transform.name + " took " + damageToTake + " " + damageType + " damage from " + from.transform.name);
        // If dead then award a kill and start the death method.
        if(currentHealth <= 0f){
            isDead = true;
            Debug.Log(transform.name + " killed by " + from.transform.name);
            from.GetComponent<ScoreManager>().Kill(gameObject);
            GetComponent<RespawnDeath>().Death();
            if(unit is Champion){
                foreach(GameObject assist in GetComponent<DamageTracker>().CheckForAssists()){
                    if(assist != from)
                        assist.GetComponent<ScoreManager>().Assist();
                }
                GetComponent<ScoreManager>()?.Death();
            }
        }
        else{
            bonusDamage?.Invoke(gameObject, isDot);
        }
    }
    
    /*
    *   UpdateAttackSpeed - Updates a units attack speed.
    */
    public void UpdateAttackSpeed(){
        float finalAS = ((Champion) unit).attackSpeed * (1 + (bonusAttackSpeed.GetValue()/100));
        if(finalAS > 2.5f)
            finalAS = 2.5f;
        attackSpeed.SetBaseValue(finalAS);
    }

    /*
    *   CalculateMoveSpeed - Calculates a units move speed. All speed boosts are used but only one slow is used.
    */
    public float CalculateMoveSpeed(){
        List<Effect> speedBonuses = statusEffectManager.GetEffectsByType(typeof(ScriptableSpeedBonus));
        float additive = 1f;
        float multiplicative = 1f;
        // Calculate the additive and multiplicative speed boosts.
        foreach(Effect effect in speedBonuses){
            ScriptableSpeedBonus myBonus = (ScriptableSpeedBonus) effect.effectType;
            if(myBonus.isAdditive){
                additive += myBonus.bonusPercent;
            }
            else{
                multiplicative *= (1f + myBonus.bonusPercent);
            }
        }
        List<Effect> slows = statusEffectManager.GetEffectsByType(typeof(ScriptableSlow));
        float slowPercent = 1f;
        // Calculate the slow percentage to apply to the units speed.
        foreach(Effect effect in slows){
            if(effect.isActivated){
                ScriptableSlow mySlow = (ScriptableSlow) effect.effectType;
                slowPercent *= (1f - mySlow.slowPercent);
            }
        }
        // Calculate final value.
        float finalMS = speed.GetValue() * additive * multiplicative * slowPercent;
        return finalMS;
    }

    // Called after all update functions are called.
    private void LateUpdate(){
        float finalMS = CalculateMoveSpeed();
        navMeshAgent.speed = finalMS;
    }
}
