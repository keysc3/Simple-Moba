using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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

    private NavMeshAgent navMeshAgent;
    private PlayerController playerController;
    private PlayerSpellInput playerSpellInput;
    private ChampionSpells championSpells;

    public GameObject playerUI { get; private set; }
    public GameObject playerBar { get; private set; }

    public Vector3 MouseOnCast { get; set; }
    public bool IsCasting { get; set; }
    private ISpell currentCastedSpell;
    public ISpell CurrentCastedSpell { 
        get  => currentCastedSpell; 
        set => currentCastedSpell = !IsCasting ? null : value;
    }

    private Material alive;
    private Renderer rend;

    [SerializeField] private Material dead;
    [SerializeField] private GameObject playerBarPrefab;
    [SerializeField] private GameObject playerUIPrefab;

    // TODO: handle respawn position somewhere else.
    private Vector3 respawnPosition = new Vector3(0f, 1.6f, -3.0f);

    void Awake(){
        unitStats = new ChampionStats((ScriptableChampion) sUnit);
        //Debug.Log("NE: " + NewUIManager.instance);
        playerUI = CreateNewPlayerUI(gameObject.name, SUnit.icon);
        playerBar = CreateNewPlayerBar(gameObject);
        isDead = false;
        statusEffects = new NewStatusEffects();
        damageTracker = new NewDamageTracker();
        inventory = new Inventory();
        myCollider = GetComponent<Collider>();
        if(playerUI != null)
            score = new NewScore(playerUI.transform.Find("Score/Container"));
        else
            score = new NewScore(null);
        levelManager = new NewLevelManager(this);
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
        UIManager.instance.NewInitialValueSetup(playerUI, playerBar, (ChampionStats) unitStats);
    }

    // Update is called once per frame
    void Update()
    {
        damageTracker.CheckForReset(Time.time);
        statusEffects.UpdateEffects(Time.deltaTime);
        levelManager.LevelUpSkill(Time.time);
        if(UIManager.instance != null){
            if(playerUI != null){
                UIManager.instance.UpdatePlayerUIHealthBar(playerUI, (ChampionStats) unitStats, IsDead);
                UIManager.instance.UpdatePlayerBarHealthBar(playerBar, (ChampionStats) unitStats, IsDead);
                UIManager.instance.UpdateManaUIs(playerUI, playerBar, (ChampionStats) unitStats);
                UIManager.instance.UpdateAllStatsUI(playerUI, (ChampionStats) unitStats);
            }
        }
        if(NewActiveChampion.instance.champions[NewActiveChampion.instance.NewActiveChamp] == gameObject){
            if(Input.GetKeyDown(KeyCode.K))
                levelManager.GainXPTester();
        }
    }

    void LateUpdate(){
        unitStats.UpdateAttackSpeed();
        navMeshAgent.speed = unitStats.CalculateMoveSpeed(statusEffects);
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

    public GameObject CreateNewPlayerUI(string name, Sprite icon){
        // Set up the players UI.
        GameObject newPlayerUI = (GameObject) Instantiate(playerUIPrefab, playerUIPrefab.transform.position, playerUIPrefab.transform.rotation);
        newPlayerUI.name = name + "UI";
        newPlayerUI.transform.SetParent(GameObject.Find("/Canvas").transform);
        RectTransform newPlayerUIRectTransform = newPlayerUI.GetComponent<RectTransform>();
        newPlayerUIRectTransform.offsetMin = new Vector2(0, 0);
        newPlayerUIRectTransform.offsetMax = new Vector2(0, 0);
        newPlayerUI.transform.Find("Player/Info/PlayerContainer/InnerContainer/IconContainer/Icon").GetComponent<Image>().sprite = icon;
        return newPlayerUI;
    }
    public GameObject CreateNewPlayerBar(GameObject champion){
        GameObject newPlayerBar = (GameObject) Instantiate(playerBarPrefab, playerBarPrefab.transform.position, playerBarPrefab.transform.rotation);
        newPlayerBar.name = champion.name + "PlayerBar";
        RectTransform newPlayerBarRectTransform = newPlayerBar.GetComponent<RectTransform>();
        Vector3 newPlayerBarPos = newPlayerBarRectTransform.anchoredPosition;
        newPlayerBar.transform.SetParent(champion.transform);
        newPlayerBarRectTransform.anchoredPosition3D = newPlayerBarPos;
        return newPlayerBar;
    }
}
