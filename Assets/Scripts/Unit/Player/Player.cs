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
    public DamageTracker damageTracker { get; private set; }
    public Inventory inventory { get; private set; }
    public Score score { get; private set; }
    public LevelManager levelManager { get; private set; }
    public GameObject playerUI { get; private set; }
    public GameObject playerBar { get; private set; }
    public Vector3 mouseOnCast;
    public bool isCasting = false;
    private Spell currentCastedSpell;
    public Spell CurrentCastedSpell { 
        get  => currentCastedSpell; 
        set => currentCastedSpell = !isCasting ? null : value;
    }
    [SerializeField] private Material dead;
    [SerializeField] private LevelInfo levelInfo;
    private PlayerController playerController;
    public PlayerSpellInput playerSpellInput { get; private set; }
    private ChampionSpells championSpells;
    private Material alive;
    private Renderer rend;

    // TODO: handle respawn position somewhere else.
    private Vector3 respawnPosition = new Vector3(0f, 1.6f, -3.0f);


    /*
    *   Init - Handles setup specific to this child class.
    */
    protected override void Init(){
        unitStats = new ChampionStats((ScriptableChampion) SUnit);
        playerController = GetComponent<PlayerController>();
        playerSpellInput = GetComponent<PlayerSpellInput>();
        championSpells = GetComponent<ChampionSpells>();
        rend = GetComponent<Renderer>();
        alive = rend.material;
        damageTracker = new DamageTracker();
        inventory = new Inventory();
        score = new Score();
        levelManager = new LevelManager(this, levelInfo);
        (playerUI, playerBar) = UIManager.instance.SetupPlayerUI(this);
        UIManager.instance.InitialValueSetup(this);
    }

    // Update is called once per frame
    protected override void Update(){
        base.Update();
        // Check if damage tracker needs resetting.
        damageTracker.CheckForReset(Time.time);
        // Check for level up skill input if skill level up available.
        levelManager.LevelUpSkill();
        // Test GainXP
        if(ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion] == gameObject){
            if(Input.GetKeyDown(KeyCode.K))
                levelManager.GainXPTester();
        }
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
        UIManager.instance.UpdateHealthBar(this);
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
    }

    /*
    *   Death - Handles the death of a player.
    */
    public override void Death(){
        score.Death();
        UIManager.instance.UpdateDeaths(score.deaths.ToString(), playerUI);
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
        if(ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion] == gameObject){
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
        UIManager.instance.UpdateManaBar(this);
        UIManager.instance.UpdateHealthBar(this);
        UIManager.instance.SetPlayerBarActive(true, playerBar);
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
            UIManager.instance.UpdateDeathTimer(respawn - timer, playerUI);
            yield return null;
        }
        UIManager.instance.UpdateDeathTimer(0f, playerUI);
        Respawn();
    }
}
