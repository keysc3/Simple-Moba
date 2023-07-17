using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements a unit.
*
* @author: Colin Keys
*/
public class Unit : MonoBehaviour, IDamagable, IKillable
{
    public ScriptableUnit unit;
    public UnitStats unitStats;
    //public Level level;
    public StatusEffects statusEffects;
    public NavMeshAgent navMeshAgent;
    protected Collider myCollider;
    [field: SerializeField] public bool isDead { get; protected set; }

    public delegate void BonusDamage(GameObject toDamage, bool isDot); 
    public BonusDamage bonusDamage;

    // Called when the script instance is being loaded. 
    protected virtual void Awake(){
        Init();
        statusEffects = new StatusEffects(this);
        navMeshAgent = GetComponent<NavMeshAgent>();
        myCollider = GetComponent<Collider>();
        isDead = false;
    }

    /*
    *   Init - Handles setup specific to this parent class.
    */
    protected virtual void Init(){
        unitStats = new UnitStats(unit);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        statusEffects.UpdateEffects();
    }

    // Called after all Update functions.
    void LateUpdate(){
        float finalMS = unitStats.CalculateMoveSpeed(statusEffects);
        navMeshAgent.speed = finalMS;
    }

    /*
    *   TakeDamage - Damages the unit.
    *   @param incomingDamage - float of the incoming damage amount.
    *   @param damageType - string of the type of damage that is being inflicted.
    *   @param from - GameObject of the damage source.
    *   @param isDot - bool if the damage was from a dot.
    */
    public virtual void TakeDamage(float incomingDamage, string damageType, GameObject from, bool isDot){
        Unit fromUnit = from.GetComponent<Unit>();
        float damageToTake = DamageCalculator.CalculateDamage(incomingDamage, damageType, fromUnit.unitStats, unitStats);
        unitStats.SetHealth(unitStats.currentHealth - damageToTake);
        Debug.Log(transform.name + " took " + damageToTake + " " + damageType + " damage from " + from.transform.name);
        // If dead then award a kill and start the death method.
        if(unitStats.currentHealth <= 0f){
            DeathActions(fromUnit);
            Death();
        }
        // Apply any damage that procs after recieving damage.
        else{
            bonusDamage?.Invoke(gameObject, isDot);
        }
    }

    /*
    *   SetDeathStatus - Sets the isDead property of the Unit.
    *   @param dead - bool to set isDead to.
    */
    public void SetDeathStatus(bool dead){
        isDead = dead;
    }

    /*
    *   Death - Handles the death of a unit by destroying it.
    */
    public virtual void Death(){
        Destroy(gameObject);
        return;
    }
    
    /*
    *   DeathActions - Handles any game state/other Unit actions upon this Units death.
    *   @param fromUnit - Unit that killed this Unit.
    */
    protected virtual void DeathActions(Unit fromUnit){
        isDead = true;
    }
}
