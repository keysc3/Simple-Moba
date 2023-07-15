using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour, IDamagable
{
    public ScriptableUnit unit;
    public UnitStats unitStats;
    //public Level level;
    public StatusEffects statusEffects;
    public NavMeshAgent navMeshAgent;
    public UIManager uiManager;
    [field: SerializeField] public bool isDead { get; protected set; }

    public delegate void BonusDamage(GameObject toDamage, bool isDot); 
    public BonusDamage bonusDamage;

    protected virtual void Awake(){
        uiManager = GetComponent<UIManager>();
        //this.champion = champion;
        //unitStats = new UnitStats(unit);
        Init();
        uiManager = GetComponent<UIManager>();
        statusEffects = new StatusEffects(this, uiManager);
        navMeshAgent = GetComponent<NavMeshAgent>();
        isDead = false;
    }

    protected virtual void Init(){
        unitStats = new UnitStats(unit);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        statusEffects.UpdateEffects();
    }

    void LateUpdate(){
        float finalMS = unitStats.CalculateMoveSpeed(statusEffects);
        navMeshAgent.speed = finalMS;
    }

    public virtual void TakeDamage(float incomingDamage, string damageType, GameObject from, bool isDot){
        Unit fromUnit = from.GetComponent<Unit>();
        float damageToTake = DamageCalculator.CalculateDamage(incomingDamage, damageType, fromUnit.unitStats, unitStats);
        unitStats.SetHealth(unitStats.currentHealth - damageToTake);
        Debug.Log(transform.name + " took " + damageToTake + " " + damageType + " damage from " + from.transform.name);
        // If dead then award a kill and start the death method.
        if(unitStats.currentHealth <= 0f){
            isDead = true;
            GetComponent<RespawnDeath>().Death();
        }
        // Apply any damage that procs after recieving damage.
        else{
            bonusDamage?.Invoke(gameObject, isDot);
        }
    }

    public void SetDeathStatus(bool dead){
        isDead = dead;
    }
}
