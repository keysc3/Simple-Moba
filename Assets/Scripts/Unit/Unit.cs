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
    [SerializeField] private ScriptableUnit sUnit;
    public ScriptableUnit SUnit { get => sUnit; }
    public bool isDead { get; protected set; }
    [field:SerializeField] public UnitStats unitStats { get; protected set; }
    public StatusEffects statusEffects { get; private set; }
    public NavMeshAgent navMeshAgent { get; private set; }
    public Collider myCollider { get; private set; }

    public delegate void BonusDamage(GameObject toDamage, bool isDot); 
    public BonusDamage bonusDamage;

    // Called when the script instance is being loaded. 
    protected virtual void Awake(){
        Init();
        statusEffects = new StatusEffects();
        navMeshAgent = GetComponent<NavMeshAgent>();
        myCollider = GetComponent<Collider>();
        isDead = false;
    }

    /*
    *   Init - Handles setup specific to this parent class.
    */
    protected virtual void Init(){
        unitStats = new UnitStats(SUnit);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        statusEffects.UpdateEffects();
    }

    // Called after all Update functions.
    private void LateUpdate(){
        //navMeshAgent.speed = unitStats.CalculateMoveSpeed(statusEffects);
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
        unitStats.CurrentHealth = unitStats.CurrentHealth - damageToTake;
        Debug.Log(transform.name + " took " + damageToTake + " " + damageType + " damage from " + from.transform.name);
        // If dead then award a kill and start the death method.
        if(unitStats.CurrentHealth <= 0f){
            DeathActions(fromUnit);
            Death();
        }
        // Apply any damage that procs after recieving damage.
        else{
            bonusDamage?.Invoke(gameObject, isDot);
        }
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
