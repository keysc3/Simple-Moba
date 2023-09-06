using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements a minion unit.
*
* @author: Colin Keys
*/
public class Minion : MonoBehaviour, IMinion
{
    private bool isDead = false;
    public bool IsDead { 
        get => isDead; 
        private set{
            isDead = value;
        }
    }
    public UnitStats unitStats { get; set; }
    public StatusEffects statusEffects { get; set; }
    public DamageTracker damageTracker { get; set; }
    public Inventory inventory { get; set; }
    public LevelManager levelManager { get; set; }
    public BonusDamage bonusDamage { get; set; }
    [SerializeField] private ScriptableUnit sUnit;
    public ScriptableUnit SUnit { get => sUnit; }
    public GameObject GameObject { get => gameObject; }
    public Vector3 Position { get => transform.position; set => transform.position = value; }

    //[SerializeField] private GameObject statusEffectPrefab;
    private NavMeshAgent navMeshAgent;

    // Called when the script instance is being loaded.
    private void Awake(){
        isDead = false;
        unitStats = new MinionStats((ScriptableMinion) sUnit);
        statusEffects = new StatusEffects(null);
        damageTracker = new DamageTracker();
        inventory = new Inventory();
        levelManager = new LevelManager(this);
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    private void Update()
    {
        damageTracker.CheckForReset(Time.time);
        statusEffects.UpdateEffects(Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.P))
            Death();
    }

    // Called after all Update functions have been called.
    private void LateUpdate(){
        unitStats.UpdateAttackSpeed();
    }

    /*
    *   TakeDamage - Damages the unit.
    *   @param incomingDamage - float of the incoming damage amount.
    *   @param damageType - string of the type of damage that is being inflicted.
    *   @param damager - IUnit of the damage source.
    *   @param isDot - bool if the damage was from a dot.
    */
    public void TakeDamage(float incomingDamage, string damageType, IUnit damager, bool isDot){
        float damageToTake = DamageCalculator.CalculateDamage(incomingDamage, damageType, damager.unitStats, unitStats);
        unitStats.CurrentHealth = unitStats.CurrentHealth - damageToTake;
        //Debug.Log(transform.name + " took " + damageToTake + " " + damageType + " damage from " + from.transform.name);
        // If dead then award a creep kill and start the death method.
        if(unitStats.CurrentHealth <= 0f){
            if(damager is IPlayer)
                ((IPlayer) damager).score.CreepKill(this);
            Death();
        }
        // Apply any damage that procs after recieving damage.
        else{
            bonusDamage?.Invoke(this, isDot);
        }
    }

    public void Death(){
       isDead = true;
       Destroy(gameObject);
    }
}
