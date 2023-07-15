using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a leveling system for champions.
*
* @author: Colin Keys
*/
public class LevelManager : MonoBehaviour
{
    public int level { get; private set; } = 1;
    public int maxLevel { get; private set; } = 18;
    public float respawnTime { get; private set; }
    public Dictionary<string, int> spellLevels { get; private set; } = new Dictionary<string, int>();

    [SerializeField] private float currentXP;
    [SerializeField] private float requiredXP;
    [SerializeField] private float gainAmount;
    [SerializeField] private int maxSpellLevel;
    [SerializeField] private int maxUltLevel;

    private int spellLevelPoints = 1;
    private bool newLevel = true;
    private List<float> championKillXP = new List<float>(){42f, 114f, 144f, 174f, 204f, 234f, 308f, 392f, 486f, 
    590f, 640f, 690f, 740f, 790f, 840f, 890f, 940f, 990f};
    private float xShift = 0f;
    private float yShift = 15f;
    private ChampionStats championStats;
    private ScriptableChampion champion;
    private UIManager uiManager;

    // Called when the script instance is being loaded.
    private void Awake()
    {
        uiManager = GetComponent<UIManager>();
    }

    // Start is called before the first frame update
    private void Start(){
        championStats = (ChampionStats) GetComponent<Player>().unitStats;
        champion = (ScriptableChampion) GetComponent<Player>().unit;
        CurrentRespawnTime();
        for(int i = 0; i < 4; i++)
            spellLevels.Add("Spell_" + (i+1), 0);
        StartCoroutine(LevelUpSkill());
        GetComponent<Player>().score.takedownCallback += GainXP; 
    }

    /*
    *   GainXPTester - Add xp to the champions total. Used for testing with a key input.
    *   @param gained - float of the amount of xp to add.
    */
    private void GainXPTester(float gained){
        currentXP += gained;
        if(currentXP >= requiredXP && level != maxLevel){
            LevelUp();
        }
        uiManager.UpdateExperienceBar(currentXP, requiredXP);
    }

    /*
    *   GainXP - Add xp to the champions total.
    *   @param killed - GameObject of the killed unit.
    */
    private void GainXP(GameObject killed){
        float gained;
        if(killed.GetComponent<Unit>().unit is ScriptableChampion)
            gained = championKillXP[killed.GetComponent<LevelManager>().level - 1];
        else
            gained = 30f;
        currentXP += gained;
        if(currentXP >= requiredXP && level != maxLevel){
            LevelUp();
        }
        uiManager.UpdateExperienceBar(currentXP, requiredXP);
    }

    /*
    *   LevelUp - Level up the champion. This includes increasing level, spell levels points, next level xp, and champion stats.
    *
    */
    private void LevelUp(){
        // Increase level and required amount for the next level.
        level++;
        uiManager.UpdateLevelText(level);
        // Keep any overflow xp for the next.
        currentXP = currentXP - requiredXP;
        requiredXP += 100.0f;
        gainAmount += 100.0f;
        // Give the champion another skill level up point and start the coroutine to allow them to level spells if not already running.
        spellLevelPoints += 1;
        newLevel = true;
        if(spellLevelPoints == 1){
            StartCoroutine(LevelUpSkill());
        }
        // Increase the champions base stats.
        IncreaseChampionStats();
        CurrentRespawnTime();
        championStats.UpdateAttackSpeed();
        uiManager.UpdateHealthBar();
        uiManager.UpdateManaBar();
        uiManager.UpdateExperienceBar(currentXP, requiredXP);
        uiManager.UpdateAllStats();
    }

    /*
    *   IncreaseChampionStats - Increases the base stats of the champion based on their level growth stat for each stat.
    */
    private void IncreaseChampionStats(){
        // Increase base health and current health by the same amount.
        float healthIncrease = GrowthAmountCalculation(champion.healthGrowth);
        IncreaseBaseStat(championStats.maxHealth, healthIncrease);
        championStats.SetHealth(championStats.currentHealth + healthIncrease);
        // Increase base mana and current mana by the same amount.
        float manaIncrease = GrowthAmountCalculation(champion.manaGrowth);
        IncreaseBaseStat(championStats.maxMana, manaIncrease);
        championStats.SetMana(championStats.currentMana + manaIncrease);
        // Increase any base stats that have a growth stat.
        IncreaseBaseStat(championStats.physicalDamage, GrowthAmountCalculation(champion.physicalDamageGrowth));
        IncreaseBaseStat(championStats.HP5, GrowthAmountCalculation(champion.HP5Growth));
        IncreaseBaseStat(championStats.MP5, GrowthAmountCalculation(champion.MP5Growth));
        IncreaseBaseStat(championStats.armor, GrowthAmountCalculation(champion.armorGrowth));
        IncreaseBaseStat(championStats.magicResist, GrowthAmountCalculation(champion.magicResistGrowth));
        championStats.bonusAttackSpeed.SetBaseValue(GrowthAmountCalculationAtkSpd(champion.attackSpeedGrowth));
    }

    /*
    *   IncreaseBaseStat - Increase the base value of stat to the value for the champions current level.
    *   @param stat - Stat object to increase the base value of.
    *   @param growthAmount - float of the amount to increase the base value by.
    */
    private void IncreaseBaseStat(Stat stat, float growthAmount){
        float newBaseValue = stat.GetBaseValue() + growthAmount;
        stat.SetBaseValue(newBaseValue);
    }

    /*
    *   GrowthAmountCalculation - Calculates the amount to increase a stat for a new level by base on the growth multiplier for that stat.
    */
    private float GrowthAmountCalculation(float growthStat){
        return growthStat * (0.65f + 0.035f * level);
    }

    /*
    *   GrowthAmountCalculationAtkSpd - Calculates the amount to increase a stat for a new level by base on the growth multiplier for that stat.
    */
    private float GrowthAmountCalculationAtkSpd(float growthStat){
        return growthStat * (level - 1) * (0.7025f + (0.0175f * (level - 1)));
    }

    // Update is called once per frame
    private void Update()
    {
        if(ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion] == gameObject){
            if(Input.GetKeyDown(KeyCode.K))
                GainXPTester(gainAmount);
        }
    }

    /*
    *   LevelUpSkill - Coroutine for leveling up the champions spell when given a skill point.Up to 5 levels for basic abilities and 3 for ultimate.
    */
    private IEnumerator LevelUpSkill(){
        uiManager.ShiftStatusEffects(new Vector2(xShift, yShift));
        // Use all skill points before stopping.
        while(spellLevelPoints != 0){
            // If a level up or skill point was used since the last UI update then update the UI.
            if(newLevel){
                uiManager.SetSkillLevelUpActive(spellLevels, level, true);
                newLevel = false;
            }
            if(ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion] == gameObject){
                // If first spell level up key bind pressed and it is not at max level then level it.
                if(Input.GetKey(KeyCode.LeftControl)){
                    if(Input.GetKeyDown(KeyCode.Q))
                        SpellLevelUpRequest("Spell_1", "basic");
                // If second spell level up key bind pressed and it is not at max level then level it.
                    if(Input.GetKeyDown(KeyCode.W))
                        SpellLevelUpRequest("Spell_2", "basic");
                // If third spell level up key bind pressed and it is not at max level then level it.
                    if(Input.GetKeyDown(KeyCode.E))
                        SpellLevelUpRequest("Spell_3", "basic");
                // If fourth spell level up key bind pressed and it is not at max level then level it.
                    if(Input.GetKeyDown(KeyCode.R))
                        SpellLevelUpRequest("Spell_4", "ultimate");
                }
            }
            // Skill level up available animation.
            uiManager.SkillLevelUpGradient();
            yield return null;
        }
        // Disable skill level up UI.
        uiManager.SetSkillLevelUpActive(spellLevels, level, false);
        // Shift effects UI back.
        uiManager.ShiftStatusEffects(new Vector2(-xShift, -yShift));
    }

    /*
    *   SpellLevelUpRequest - Checks if the requested spell can be leveled up and calls the level up method.
    *   @param spell - string of the spell requested to be leveled up.
    *   @param type - string of the type of spell to distinguish between 5 point vs 3 point spells (basic vs ultimate).
    */
    private void SpellLevelUpRequest(string spell, string type){
        // Basic ability.
        if(type == "basic"){
            if(spellLevels[spell] != maxSpellLevel)
                SpellLevelUp(spell);
        }
        // Ultimate ability.
        else{
            if(spellLevels[spell] != maxUltLevel){
                int spell_4_level = spellLevels["Spell_4"];
                // Allow one point in fourth spell (ultimate) at level 6-10, two points at 11-15, and three points at 16-18.
                if((spell_4_level < 1 && level > 5) || (spell_4_level < 2 && level > 10) || (spell_4_level < 3 && level > 15))
                    SpellLevelUp(spell);
            }
        }
    }

    /*
    *   SpellLevelUp - Levels up the given spell and updates the UI.
    *   @paream spell - string of the spell to be leveled up.
    */
    private void SpellLevelUp(string spell){
        spellLevels[spell] = spellLevels[spell] + 1;
        spellLevelPoints -= 1;
        newLevel = true;
        // If the spell wasn't level 1 yet then take of the spell unlearned cover.
        if(spellLevels[spell] == 1)
            uiManager.SpellLearned(spell);
        else
            uiManager.SpellLeveled(spell, spellLevels[spell]);
    }

    /*
    *   CurrentRespawnTime - Calculates the current respawn time based on the players level.
    */
    private void CurrentRespawnTime(){
        if(level < 7){
            respawnTime = (level * 2f) + 4f;
        }
        else if(level == 7){
            respawnTime = 21f;
        }
        else{
            respawnTime = (level * 2.5f) + 7.5f;
        }
    }
}
