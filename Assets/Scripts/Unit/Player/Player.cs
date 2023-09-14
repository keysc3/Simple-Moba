using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

/*
* Purpose: Implements a player unit.
*
* @author: Colin Keys
*/
public class Player : MonoBehaviour, IPlayer
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
    public StatusEffects statusEffects { get; set; }
    public DamageTracker damageTracker { get; set; }
    public Inventory inventory { get; set; }
    [SerializeField] private ScriptableUnit sUnit;
    public ScriptableUnit SUnit { get => sUnit; }
    public LevelManager levelManager { get; set; }
    public Score score { get; set; }
    public BonusDamage bonusDamage { get; set; }
    public GameObject playerUI { get; private set; }
    public GameObject playerBar { get; private set; }
    public Vector3 MouseOnCast { get; set; }
    public bool IsCasting { get; set; }
    private ISpell currentCastedSpell;
    public ISpell CurrentCastedSpell { 
        get  => currentCastedSpell; 
        set => currentCastedSpell = !IsCasting ? null : value;
    }
    public GameObject GameObject { get => gameObject; }
    public Vector3 Position { get => transform.position; set => transform.position = value; }

    private Collider myCollider;
    private NavMeshAgent navMeshAgent;
    private PlayerControllerBehaviour playerController;
    private SpellInputBehaviour playerSpellInput;
    private PlayerSpells playerSpells;
    private Material alive;
    private Renderer rend;
    private GameObject playerIconCover;
    private TMP_Text playerRespawnTimer;
    [SerializeField] private Material dead;
    [SerializeField] private GameObject playerBarPrefab;
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private GameObject statusEffectPrefab;

    // TODO: handle respawn position somewhere else.
    private Vector3 respawnPosition = new Vector3(0f, 1.6f, -3.0f);

    // Called when the script instance is being loaded.
    private void Awake(){
        unitStats = new ChampionStats((ScriptableChampion) sUnit);
        playerUI = CreateNewPlayerUI();
        playerBar = CreateNewPlayerBar();
        if(playerUI != null){
            playerIconCover = playerUI.transform.Find("Player/Info/PlayerContainer/InnerContainer/IconContainer/IconCover").gameObject;
            playerRespawnTimer = playerIconCover.transform.Find("DeathTimer").GetComponent<TMP_Text>();
        }
        isDead = false;
        statusEffects = new StatusEffects(statusEffectPrefab);
        damageTracker = new DamageTracker();
        inventory = new Inventory();
        myCollider = GetComponent<Collider>();
        if(playerUI != null)
            score = new Score(playerUI.transform.Find("Score/Container"));
        else
            score = new Score(null);
        levelManager = new LevelManager(this);
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerController = GetComponent<PlayerControllerBehaviour>();
        playerSpellInput = GetComponent<SpellInputBehaviour>();
        playerSpells = GetComponent<PlayerSpells>();
        rend = GetComponent<Renderer>();
        alive = rend.material;
    }

    // Start is called before the first frame update
    private void Start()
    {
        //UIManager.instance.InitialValueSetup(playerUI, playerBar, (ChampionStats) unitStats);
    }

    // Update is called once per frame
    private void Update()
    {
        damageTracker.CheckForReset(Time.time);
        statusEffects.UpdateEffects(Time.deltaTime);
        levelManager.LevelUpSkill(Time.time);
        if(ActiveChampion.instance.champions[ActiveChampion.instance.ActiveChamp] == gameObject){
            if(Input.GetKeyDown(KeyCode.K))
                levelManager.GainXPTester();
        }
        if(ActiveChampion.instance.champions[ActiveChampion.instance.ActiveChamp] == gameObject){
            if(Input.GetKeyDown(KeyCode.T))
                Death();
        }
    }

    // Called after all Update functions have been called.
    private void LateUpdate(){
        unitStats.UpdateAttackSpeed();
        navMeshAgent.speed = unitStats.CalculateMoveSpeed(statusEffects);
    }

    /*
    *   TakeDamage - Damages the unit.
    *   @param incomingDamage - float of the incoming damage amount.
    *   @param damageType - string of the type of damage that is being inflicted.
    *   @param damager - IUnit of the damage source.
    *   @param isDot - bool if the damage was from a dot.
    */
    public void TakeDamage(float incomingDamage, string damageType, IUnit damager, bool isDot){
        if(!isDead){
            float damageToTake = DamageCalculator.CalculateDamage(incomingDamage, damageType, damager.unitStats, unitStats);
            unitStats.CurrentHealth = unitStats.CurrentHealth - damageToTake;
            if(damageTracker != null){
                damageTracker.AddDamage(damager, damageToTake, damageType);
            }
            // If dead then award a kill and start the death method.
            if(unitStats.CurrentHealth <= 0f){
                unitStats.CurrentHealth = 0f;
                UpdateScores(damager);
                Death();
            }
            // Apply any damage that procs after recieving damage.
            else{
                bonusDamage?.Invoke(this, isDot);
            }
        }
    }

    /*
    *   UpdateScores - Update the scores of the every unit involved in a kill.
    *   @param damager - IUnit of the killing player.
    */
    private void UpdateScores(IUnit damager){
        if(score != null)
            score.Death();
        if(damager is IPlayer){
            IPlayer killer = (IPlayer) damager;
            if(killer.score != null)
                killer.score.ChampionKill(this);
            // Grant any assists if the unit is a champion.
            if(damageTracker != null){
                foreach(IPlayer assister in damageTracker.CheckForAssists(killer, Time.time)){
                    assister.score.Assist(this);
                }
            }
        }
    }

    /*
    *   Death - Handles the death of a player.
    */
    private void Death(){
        isDead = true;
        if(statusEffects != null)
            statusEffects.ResetEffects();
        if(damageTracker != null)
            damageTracker.ResetDamageTracker();
        // Disable all combat and movement controls.
        if(playerController != null)
            playerController.enabled = false; 
        if(playerSpellInput != null)
            playerSpellInput.enabled = false;
        if(navMeshAgent != null){
            navMeshAgent.ResetPath();
            navMeshAgent.enabled = false;
        }
        // Disable collider and remove status effects.
        if(myCollider != null)
            myCollider.enabled = false;
        // Handle champion death clean up.
        if(playerSpells != null)
            playerSpells.OnDeathSpellCleanUp();
        if(rend != null)
            rend.material = dead;
        if(levelManager != null)
            StartCoroutine(RespawnTimer(levelManager.RespawnTime()));
    }

    /*
    *   Respawn - Respawn the player by enabling functionality, resetting stats, and moving to respawn location.
    */
    private void Respawn(){
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
        playerBar.SetActive(true);
        // Move player to respawn location.
        transform.position = respawnPosition;
    }

    /*
    *   RespawnTimer - Coroutine for timing a respawn.
    *   @param respawn - flaot of the respawn time.
    */
    private IEnumerator RespawnTimer(float respawn){
        float timer = 0.0f;
        if(playerIconCover != null)
            playerIconCover.SetActive(true);
        while(timer < respawn){
            timer += Time.deltaTime;
            if(playerRespawnTimer != null)
                playerRespawnTimer.SetText(Mathf.Ceil(respawn - timer).ToString());
            yield return null;
        }
        if(playerIconCover != null)
            playerIconCover.SetActive(false); 
        Respawn();
    }

    /*
    *   CreateNewPlayerUI - Creates a new player UI.
    *   @return GameObject - New player UI.
    */
    private GameObject CreateNewPlayerUI(){
        GameObject newPlayerUI = null;
        if(playerUIPrefab != null){
            newPlayerUI = (GameObject) Instantiate(playerUIPrefab, playerUIPrefab.transform.position, playerUIPrefab.transform.rotation);
            newPlayerUI.name = transform.name + "UI";
            newPlayerUI.transform.SetParent(GameObject.Find("/Canvas").transform);
            RectTransform newPlayerUIRectTransform = newPlayerUI.GetComponent<RectTransform>();
            newPlayerUIRectTransform.offsetMin = new Vector2(0, 0);
            newPlayerUIRectTransform.offsetMax = new Vector2(0, 0);
            newPlayerUI.transform.Find("Player/Info/PlayerContainer/InnerContainer/IconContainer/Icon").GetComponent<Image>().sprite = SUnit.icon;
            // Setup health and mana UI updates.
            UpdateHealthUI updateHealthUI = newPlayerUI.transform.Find("Player/Combat/ResourceContainer/HealthContainer/HealthBar").GetComponent<UpdateHealthUI>();
            updateHealthUI.player = this;
            UpdateManaUI updateManaUI = newPlayerUI.transform.Find("Player/Combat/ResourceContainer/ManaContainer/ManaBar").GetComponent<UpdateManaUI>();
            updateManaUI.player = this;
            // Setup stats UI updates.
            UpdateAllStatsUI updateAllStatsUI = newPlayerUI.transform.Find("Player/Info/Stats/Container").GetComponent<UpdateAllStatsUI>();
            updateAllStatsUI.player = this;
        }
        return newPlayerUI;
    }

    /*
    *   CreateNewPlayerBar - Creates a new player bar.
    *   @return GameObject - New player bar.
    */
    private GameObject CreateNewPlayerBar(){
        GameObject newPlayerBar = null;
        if(playerBarPrefab != null){
            newPlayerBar = (GameObject) Instantiate(playerBarPrefab, playerBarPrefab.transform.position, playerBarPrefab.transform.rotation);
            newPlayerBar.name = transform.name + "PlayerBar";
            RectTransform newPlayerBarRectTransform = newPlayerBar.GetComponent<RectTransform>();
            Vector3 newPlayerBarPos = newPlayerBarRectTransform.anchoredPosition;
            newPlayerBar.transform.SetParent(transform);
            newPlayerBarRectTransform.anchoredPosition3D = newPlayerBarPos;
        }
        return newPlayerBar;
    }
}
