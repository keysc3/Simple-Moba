using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : Unit, IRespawnable
{
    public Inventory inventory;
    public Score score;
    private PlayerController playerController;
    private PlayerSpellInput playerSpellInput;
    private ChampionAbilities championAbilities;
    private LevelManager levelManager;
    private DamageTracker damageTracker;
    private Material alive;
    [SerializeField] private Material dead;

    // TODO: handle respawn position somewhere else.
    private Vector3 respawnPosition = new Vector3(0f, 1.6f, -3.0f);


    protected override void Init(){
        unitStats = new ChampionStats((ScriptableChampion)unit);
        playerController = GetComponent<PlayerController>();
        playerSpellInput = GetComponent<PlayerSpellInput>();
        championAbilities = GetComponent<ChampionAbilities>();
        levelManager = GetComponent<LevelManager>();
        damageTracker = GetComponent<DamageTracker>();
        inventory = new Inventory(this);
        score = new Score();
    }

    // Start is called before the first frame update
    void Start()
    {
        alive = rend.material;
    }

    /*
    *   TakeDamage - Damages the unit.
    *   @param incomingDamage - float of the incoming damage amount.
    *   @param damageType - string of the type of damage that is being inflicted.
    *   @param from - GameObject of the damage source.
    */
    public override void TakeDamage(float incomingDamage, string damageType, GameObject from, bool isDot){
        Unit fromUnit = from.GetComponent<Unit>();
        float damageToTake = DamageCalculator.CalculateDamage(incomingDamage, damageType, fromUnit.unitStats, unitStats);
        unitStats.SetHealth(unitStats.currentHealth - damageToTake);
        Debug.Log(transform.name + " took " + damageToTake + " " + damageType + " damage from " + from.transform.name);
        // If dead then award a kill and start the death method.
        if(unitStats.currentHealth <= 0f){
            isDead = true;
            Debug.Log(transform.name + " killed by " + from.transform.name);
            Death();
            if(fromUnit is Player){
                Player killer = (Player) fromUnit;
                killer.score.ChampionKill(gameObject);
                killer.uiManager.UpdateKills(killer.score.kills.ToString());
                // Grant any assists if the unit is a champion.
                foreach(GameObject assist in GetComponent<DamageTracker>().CheckForAssists()){
                    if(assist != from)
                        assist.GetComponent<Player>().score.Assist();
                }
            }
            score.Death();
            uiManager.UpdateDeaths(score.deaths.ToString());
        }
        // Apply any damage that procs after recieving damage.
        else{
            bonusDamage?.Invoke(gameObject, isDot);
        }
        GetComponent<DamageTracker>().DamageTaken(from, incomingDamage, damageType);
        GetComponent<UIManager>().UpdateHealthBar();
    }

    /*
    *   Death - Handles the death of a player by disabling functionality.
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
        uiManager.UpdateManaBar();
        uiManager.UpdateHealthBar();
        uiManager.SetPlayerBarActive(true);
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
            uiManager.UpdateDeathTimer(respawn - timer);
            yield return null;
        }
        uiManager.UpdateDeathTimer(0f);
        Respawn();
    }
}
