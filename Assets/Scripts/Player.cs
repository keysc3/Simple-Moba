using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements a player unit.
*
* @author: Colin Keys
*/
public class Player : Unit, IRespawnable
{
    public DamageTracker damageTracker;
    public Inventory inventory;
    public Score score;
    private PlayerController playerController;
    private PlayerSpellInput playerSpellInput;
    private ChampionAbilities championAbilities;
    private LevelManager levelManager;
    protected Renderer rend;
    //private DamageTracker damageTracker;
    private Material alive;
    [SerializeField] private Material dead;
    public GameObject playerUIPrefab;
    public GameObject playerUI;
    public GameObject playerBar { get; private set; }

    // TODO: handle respawn position somewhere else.
    private Vector3 respawnPosition = new Vector3(0f, 1.6f, -3.0f);


    /*
    *   Init - Handles setup specific to this child class.
    */
    protected override void Init(){
        unitStats = new ChampionStats((ScriptableChampion)unit);
        playerController = GetComponent<PlayerController>();
        playerSpellInput = GetComponent<PlayerSpellInput>();
        championAbilities = GetComponent<ChampionAbilities>();
        levelManager = GetComponent<LevelManager>();
        rend = GetComponent<Renderer>();
        //damageTracker = GetComponent<DamageTracker>();
        damageTracker = new DamageTracker();
        inventory = new Inventory(this);
        score = new Score();
        playerUI = UIManager.instance.CreatePlayerHUD(gameObject, playerUIPrefab, this);
        playerBar = UIManager.instance.CreatePlayerBar(gameObject);
        UIManager.instance.SetUpPlayerUI(this, playerUI, playerBar);
    }

    // Start is called before the first frame update
    void Start()
    {
        alive = rend.material;
    }

    protected override void Update(){
        base.Update();
        if(damageTracker.damageReceived.Count > 0)
            damageTracker.CheckForReset(Time.time);
    }

    /*
    *   TakeDamage - Damages the unit.
    *   @param incomingDamage - float of the incoming damage amount.
    *   @param damageType - string of the type of damage that is being inflicted.
    *   @param from - GameObject of the damage source.
    *   @param isDot - bool if the damage was from a dot.
    */
    public override void TakeDamage(float incomingDamage, string damageType, GameObject from, bool isDot){
        base.TakeDamage(incomingDamage, damageType, from, isDot);
        damageTracker.AddDamage(from, incomingDamage, damageType);
        UIManager.instance.UpdateHealthBar(this, playerUI, playerBar);
    }

    /*
    *   DeathActions - Handles any game state/other Unit actions upon this Players death.
    *   @param fromUnit - Unit that killed this Player.
    */
    protected override void DeathActions(Unit fromUnit){
        base.DeathActions(fromUnit);
        if(fromUnit is Player){
            Player killer = (Player) fromUnit;
            killer.score.ChampionKill(gameObject);
            UIManager.instance.UpdateKills(killer.score.kills.ToString(), killer.playerUI);
            // Grant any assists if the unit is a champion.
            foreach(GameObject assist in damageTracker.CheckForAssists()){
                if(assist != fromUnit.gameObject)
                    assist.GetComponent<Player>().score.Assist();
            }
        }
        score.Death();
        UIManager.instance.UpdateDeaths(score.deaths.ToString(), playerUI);
    }

    /*
    *   Death - Handles the death of a player.
    */
    public override void Death(){
        // Disable all combat and movement controls.
        playerController.enabled = false; 
        playerSpellInput.enabled = false;
        navMeshAgent.ResetPath();
        navMeshAgent.enabled = false;
        // Disable collider and remove status effects.
        myCollider.enabled = false;
        statusEffects.ResetEffects();
        // Handle champion death clean up.
        championAbilities.OnDeathCleanUp();
        rend.material = dead;
        StartCoroutine(RespawnTimer(levelManager.respawnTime));
    }

    /*
    *   Respawn - Respawn the player by enabling functionality, resetting stats, and moving to respawn location.
    */
    public void Respawn(){
        navMeshAgent.enabled = true;
        // If active champion then enable controls.
        if(ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion] == gameObject){
            playerController.enabled = true;
            playerSpellInput.enabled = true;
        }
        // Handle any clean up needed before respawn.
        championAbilities.OnRespawnCleanUp();
        // Enable collider.
        myCollider.enabled = true;
        // Set currenthp and currentmana to max values.
        ((ChampionStats) unitStats).ResetHealth();
        ((ChampionStats) unitStats).ResetMana();
        damageTracker.ResetDamageTracker();
        // Set alive values.
        rend.material = alive;
        isDead = false;
        UIManager.instance.UpdateManaBar((ChampionStats) unitStats, playerUI, playerBar);
        UIManager.instance.UpdateHealthBar(this, playerUI, playerBar);
        UIManager.instance.SetPlayerBarActive(true, playerBar);
        // Move player to respawn location.
        transform.position = respawnPosition;
    }

    /*
    *   RespawnTimer - Coroutine for timing a respawn.
    */
    private IEnumerator RespawnTimer(float respawn){
        float timer = 0.0f;
        while(timer < respawn){
            timer += Time.deltaTime;
            UIManager.instance.UpdateDeathTimer(respawn - timer, playerUI);
            yield return null;
        }
        UIManager.instance.UpdateDeathTimer(0f, playerUI);
        Respawn();
    }
}
