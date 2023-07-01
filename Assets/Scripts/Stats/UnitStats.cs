using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [field: SerializeField] public Unit unit { get; private set; }

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
        isDead = false;
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

    public void SetDeathStatus(bool dead){
        isDead = dead;
    }

    /*
    *   TakeDamage - Damages the champion.
    *   @param incomingDamage - float of the incoming damage amount.
    *   @param damageType - string of the type of damage that is being inflicted.
    *   @param from - GameObject of the damage source.
    */
    public virtual void TakeDamage(float incomingDamage, string damageType, GameObject from, bool isDot){
        float finalDamage = incomingDamage;
        // If still alive then apply damage mitigation.
        if(currentHealth > 0f){
            if(damageType == "magic")
                finalDamage = MitigateMagicDamage(incomingDamage);
            else if(damageType == "physical")
                finalDamage = MitigatePhysicalDamage(incomingDamage);
            currentHealth -= finalDamage;
            Debug.Log(transform.name + " took " + finalDamage + " " + damageType + " damage from " + from.transform.name);
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
    }

    /*
    *   MitigateMagicDamage - Reduces the incoming magic damage based on the champions stats.
    */
    private float MitigateMagicDamage(float incomingDamage){
        //TODO: mitigate
        return incomingDamage;
    }

    /*
    *   MitigatePhysicalDamage - Reduces the incoming physical damage based on the champions stats.
    */
    private float MitigatePhysicalDamage(float incomingDamage){
        //TODO: mitigate
        return incomingDamage;
    }
    
}
