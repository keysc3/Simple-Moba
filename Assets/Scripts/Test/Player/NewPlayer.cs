using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewPlayer : MonoBehaviour, IPlayer, INewDamagable
{
    private bool isDead = false;
    public bool IsDead { 
        get => isDead; 
        private set{
            isDead = value;
            playerBar.SetActive(!value);
        }
    }
    public UnitStats unitStats { get; set; }
    public NewStatusEffects statusEffects { get; set; }
    public NewDamageTracker damageTracker { get; set; }
    public Inventory inventory { get; set; }
    //public Score score { get; private set; }
    
    public Collider myCollider { get;set; }
    //public ChampionStats unitStats { get; set; }
    [SerializeField] private ScriptableUnit sUnit;
    public ScriptableUnit SUnit { get => sUnit; }

    public NewLevelManager levelManager { get; set; }
    public NewScore score { get; set; }
    public BonusDamage bonusDamage { get; set; }

    public NavMeshAgent navMeshAgent { get; set; }
    public PlayerController playerController { get; private set; }
    public PlayerSpellInput playerSpellInput { get; private set; }
    public ChampionSpells championSpells { get; private set; }

    public GameObject playerUI { get; private set; }
    public GameObject playerBar { get; private set; }

    public Vector3 mouseOnCast;
    public bool isCasting = false;


    private Material alive;
    private Renderer rend;
    [SerializeField] private Material dead;

    // TODO: handle respawn position somewhere else.
    private Vector3 respawnPosition = new Vector3(0f, 1.6f, -3.0f);

    void Awake(){
        isDead = false;
        unitStats = new ChampionStats((ScriptableChampion) sUnit);
        statusEffects = new NewStatusEffects();
        damageTracker = new NewDamageTracker();
        inventory = new Inventory();
        myCollider = GetComponent<Collider>();

        levelManager = new NewLevelManager(this);
        score = new NewScore(playerUI.transform.Find("Score"));
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerController = GetComponent<PlayerController>();
        playerSpellInput = GetComponent<PlayerSpellInput>();
        championSpells = GetComponent<ChampionSpells>();

        rend = GetComponent<Renderer>();
        alive = rend.material;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        damageTracker.CheckForReset(Time.time);
        statusEffects.UpdateEffects(Time.deltaTime);
        if(UIManager.instance != null){
            UIManager.instance.UpdatePlayerUIHealthBar(playerUI, (ChampionStats) unitStats, IsDead);
            UIManager.instance.UpdatePlayerBarHealthBar(playerBar, (ChampionStats) unitStats, IsDead);
            UIManager.instance.UpdateManaUIs(playerUI, playerBar, (ChampionStats) unitStats);
            UIManager.instance.UpdateAllStatsUI(playerUI, (ChampionStats) unitStats);
        }
    }

    void LateUpdate(){
        unitStats.UpdateAttackSpeed();
        //navMeshAgent.speed = unitStats.CalculateMoveSpeed(statusEffects);
    }

    /*
    *   TakeDamage - Damages the unit.
    *   @param incomingDamage - float of the incoming damage amount.
    *   @param damageType - string of the type of damage that is being inflicted.
    *   @param from - GameObject of the damage source.
    *   @param isDot - bool if the damage was from a dot.
    */
    public void TakeDamage(float incomingDamage, string damageType, IUnit damager, bool isDot){
        float damageToTake = DamageCalculator.CalculateDamage(incomingDamage, damageType, damager.unitStats, unitStats);
        unitStats.CurrentHealth = unitStats.CurrentHealth - damageToTake;
        //Debug.Log(transform.name + " took " + damageToTake + " " + damageType + " damage from " + from.transform.name);
        // If dead then award a kill and start the death method.
        if(unitStats.CurrentHealth <= 0f){
            isDead = true;
            UpdateScores(damager);
            Death();
        }
        // Apply any damage that procs after recieving damage.
        else{
            bonusDamage?.Invoke(this, isDot);
        }
    }

    private void UpdateScores(IUnit damager){
        if(damager is IPlayer){
            IPlayer killer = (IPlayer) damager;
            killer.score.ChampionKill(this);
            //UIManager.instance.UpdateKills(killer.score.kills.ToString(), killer.playerUI);
            // Grant any assists if the unit is a champion.
            foreach(IPlayer assister in damageTracker.CheckForAssists(killer, Time.time)){
                assister.score.Assist(this);
            }
        }
    }

    /*
    *   Death - Handles the death of a player.
    */
    private void Death(){
        score.Death();
        //UIManager.instance.UpdateDeaths(score.deaths.ToString(), playerUI);
        // Disable all combat and movement controls.
        playerController.enabled = false; 
        playerSpellInput.enabled = false;
        navMeshAgent.ResetPath();
        navMeshAgent.enabled = false;
        // Disable collider and remove status effects.
        myCollider.enabled = false;
        statusEffects.ResetEffects();
        // Handle champion death clean up.
        championSpells.OnDeathSpellCleanUp();
        rend.material = dead;
        damageTracker.ResetDamageTracker();
        StartCoroutine(RespawnTimer(levelManager.RespawnTime()));
    }

    /*
    *   Respawn - Respawn the player by enabling functionality, resetting stats, and moving to respawn location.
    */
    public void Respawn(){
        navMeshAgent.enabled = true;
        // If active champion then enable controls.
        if(ActiveChampion.instance.champions[ActiveChampion.instance.ActiveChamp] == gameObject){
            playerController.enabled = true;
            playerSpellInput.enabled = true;
        }
        // TODO: Implement respawn cleanup
        //championAbilities.OnRespawnCleanUp();
        // Enable collider.
        myCollider.enabled = true;
        // Set currenthp and currentmana to max values.
        ((ChampionStats) unitStats).ResetHealth();
        ((ChampionStats) unitStats).ResetMana();
        // Set alive values.
        rend.material = alive;
        isDead = false;
        //UIManager.instance.UpdateManaBar(this);
        //UIManager.instance.UpdateHealthBar(this);
        // Move player to respawn location.
        transform.position = respawnPosition;
    }

    /*
    *   RespawnTimer - Coroutine for timing a respawn.
    *   @param respawn - flaot of the respawn time.
    */
    private IEnumerator RespawnTimer(float respawn){
        float timer = 0.0f;
        while(timer < respawn){
            timer += Time.deltaTime;
            //UIManager.instance.UpdateDeathTimer(respawn - timer, playerUI);
            yield return null;
        }
        //UIManager.instance.UpdateDeathTimer(0f, playerUI);
        Respawn();
    }
}
