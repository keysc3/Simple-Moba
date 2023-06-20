using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Handles the death and respawn actions of a unit if they have any.
*
* @author: Colin Keys
*/
public class RespawnDeath : MonoBehaviour
{
    [SerializeField] private Material dead;

    private float respawn = 1f;
    private Vector3 respawnPosition = new Vector3(0f, 1.6f, -3.0f);
    private Material alive;
    private NavMeshAgent navMeshAgent;
    private UIManager uiManager;
    private Collider myCollider;
    private Renderer rend;
    private LevelManager levelManager;
    private PlayerController playerController;
    private PlayerSpellInput playerSpellInput;
    private ChampionAbilities championAbilities;
    private StatusEffectManager statusEffectManager;
    private UnitStats unitStats;
    private DamageTracker damageTracker;

    // Called when the script instance is being loaded.
    void Awake(){
        unitStats = GetComponent<UnitStats>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        statusEffectManager = GetComponent<StatusEffectManager>();
        rend = GetComponent<Renderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(unitStats.unit is Champion){
            uiManager = GetComponent<UIManager>();
            myCollider = GetComponent<Collider>();
            levelManager = GetComponent<LevelManager>();
            playerController = GetComponent<PlayerController>(); 
            playerSpellInput = GetComponent<PlayerSpellInput>();
            championAbilities = GetComponent<ChampionAbilities>();
            damageTracker = GetComponent<DamageTracker>();
        }
        alive = rend.material;
    }

    // Update is called once per frame
    void Update()
    {
        if(ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion] == gameObject){
            if(Input.GetKeyDown(KeyCode.T)){
                unitStats.SetDeathStatus(true);
                Death();
            }
        }
        if(Input.GetKeyDown(KeyCode.O)){
            if(respawn > 5.0f)
                respawn -= 5.0f;
        }
    }

    /*
    *   Death - Handles the death of a unit by destroying it or disabling functionality.
    */
    public void Death(){
        // If the unit is a minion destroy it.
        if(unitStats.unit is Minion){
            Destroy(gameObject);
            return;
        }
        // Disable all combat and movement controls.
        playerController.enabled = false; 
        playerSpellInput.enabled = false;
        navMeshAgent.ResetPath();
        navMeshAgent.enabled = false;
        // Disable collider and remove status effects.
        myCollider.enabled = false;
        statusEffectManager.ResetEffects();
        // Handle champion death clean up.
        championAbilities.OnDeathCleanUp();
        rend.material = dead;
        // Get the respawn length and start the timer.
        respawn = levelManager.respawnTime;
        StartCoroutine(RespawnTimer());
    }

    /*
    *   Respawn - Respawn the player by enabling functionality, resetting stats, and moving to respawn location.
    */
    private void Respawn(){
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
        ChampionStats championStats = (ChampionStats)unitStats;
        championStats.ResetHealth();
        championStats.ResetMana();
        damageTracker.ResetDamageTracker();
        // Set alive values.
        rend.material = alive;
        unitStats.SetDeathStatus(false);
        uiManager.UpdateManaBar();
        uiManager.UpdateHealthBar();
        uiManager.SetPlayerBarActive(true);
        // Move player to respawn location.
        transform.position = respawnPosition;
    }

    /*
    *   RespawnTimer - Coroutine for timing a respawn.
    */
    private IEnumerator RespawnTimer(){
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
