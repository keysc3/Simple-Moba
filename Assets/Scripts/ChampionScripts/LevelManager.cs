using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a leveling system for champions.
*
* @author: Colin Keys
*/
[System.Serializable]
public class LevelManager
{
    [field: SerializeField] public int level { get; private set; } = 1;
    [field: SerializeField] public int spellLevelPoints { get; private set; } = 1;
    [field: SerializeField] public bool skillLevelUpUIActive { get; private set; } = true;
    [field: SerializeField] public LevelInfo levelInfo { get; private set; }
    [field: SerializeField] public float gainAmount { get; private set; } = 100f;
    public Dictionary<string, int> spellLevels { get; private set; } = new Dictionary<string, int>();
    
    [SerializeField] private float currentXP;
    private bool newLevel = true;
    private ChampionStats championStats;
    private ScriptableChampion champion;
    private Player player;
    
    /*
    *   LevelManager - Initializes a new level manager.
    *   @param plater - Player this level manager is for.
    *   @param levelInfo - LevelInfo to use for leveling information/constants.
    */
    public LevelManager(Player player, LevelInfo levelInfo){
        this.levelInfo = levelInfo;
        this.player = player;
        championStats = (ChampionStats) player.unitStats;
        champion = (ScriptableChampion) player.unit;
        for(int i = 0; i < 4; i++)
            spellLevels.Add("Spell_" + (i+1), 0);
        player.score.takedownCallback += GainXP; 
    }

    /*
    *   GainXPTester - Add xp to the champions total. Used for testing with a key input.
    *   @param gained - float of the amount of xp to add.
    */
    public void GainXPTester(float gained){
        currentXP += gained;
        if(level != levelInfo.maxLevel){
            if(currentXP >= levelInfo.requiredXP[level])
                LevelUp();
        }
        if(level != levelInfo.maxLevel)
            UIManager.instance.UpdateExperienceBar(currentXP, levelInfo.requiredXP[level], player.playerUI);
        else{
            UIManager.instance.UpdateExperienceBar(currentXP, currentXP, player.playerUI);
        }
    }

    /*
    *   GainXP - Add xp to the champions total.
    *   @param killed - GameObject of the killed unit.
    */
    private void GainXP(GameObject killed){
        float gained;
        if(killed.GetComponent<Unit>() is Player)
            gained = levelInfo.championKillXP[killed.GetComponent<Player>().levelManager.level - 1];
        else
            gained = 30f;
        currentXP += gained;
        if(currentXP >= levelInfo.requiredXP[level - 1] && level != levelInfo.maxLevel){
            LevelUp();
        }
        if(level != levelInfo.maxLevel)
            UIManager.instance.UpdateExperienceBar(currentXP, levelInfo.requiredXP[level], player.playerUI);
        else{
            UIManager.instance.UpdateExperienceBar(currentXP, currentXP, player.playerUI);
        }
    }

    /*
    *   LevelUp - Level up the champion. This includes increasing level, spell levels points, next level xp, and champion stats.
    *
    */
    private void LevelUp(){
        // Keep any overflow xp for the next.
        currentXP = currentXP - levelInfo.requiredXP[level];
        // Increase level and required amount for the next level.
        level++;
        UIManager.instance.UpdateLevelText(level, player.playerUI, player.playerBar);
        //requiredXP += 100.0f;
        gainAmount += 100.0f;
        // Give the champion another skill level up point and start the coroutine to allow them to level spells if not already running.
        spellLevelPoints += 1;
        newLevel = true;
        // Increase the champions base stats.
        IncreaseChampionStats();
        championStats.UpdateAttackSpeed();
        UIManager.instance.UpdateHealthBar(player);
        UIManager.instance.UpdateManaBar(player);
        UIManager.instance.UpdateAllStats(player);
    }

    /*
    *   IncreaseChampionStats - Increases the base stats of the champion based on their level growth stat for each stat.
    */
    private void IncreaseChampionStats(){
        // Increase base health and current health by the same amount.
        float healthIncrease = GrowthAmountCalculation(champion.healthGrowth);
        IncreaseBaseStat(championStats.maxHealth, healthIncrease);
        championStats.CurrentHealth = championStats.CurrentHealth + healthIncrease;
        // Increase base mana and current mana by the same amount.
        float manaIncrease = GrowthAmountCalculation(champion.manaGrowth);
        IncreaseBaseStat(championStats.maxMana, manaIncrease);
        championStats.CurrentMana = championStats.CurrentMana + manaIncrease;
        // Increase any base stats that have a growth stat.
        IncreaseBaseStat(championStats.physicalDamage, GrowthAmountCalculation(champion.physicalDamageGrowth));
        IncreaseBaseStat(championStats.HP5, GrowthAmountCalculation(champion.HP5Growth));
        IncreaseBaseStat(championStats.MP5, GrowthAmountCalculation(champion.MP5Growth));
        IncreaseBaseStat(championStats.armor, GrowthAmountCalculation(champion.armorGrowth));
        IncreaseBaseStat(championStats.magicResist, GrowthAmountCalculation(champion.magicResistGrowth));
        championStats.bonusAttackSpeed.BaseValue = GrowthAmountCalculationAtkSpd(champion.attackSpeedGrowth);
        //championStats.bonusAttackSpeed.SetBaseValue(GrowthAmountCalculationAtkSpd(champion.attackSpeedGrowth));
    }

    /*
    *   IncreaseBaseStat - Increase the base value of stat to the value for the champions current level.
    *   @param stat - Stat object to increase the base value of.
    *   @param growthAmount - float of the amount to increase the base value by.
    */
    private void IncreaseBaseStat(Stat stat, float growthAmount){
        float newBaseValue = stat.BaseValue + growthAmount;
        stat.BaseValue = newBaseValue;
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

    /*
    *   LevelUpSkill - Coroutine for leveling up the champions spell when given a skill point.Up to 5 levels for basic abilities and 3 for ultimate.
    */
    public void LevelUpSkill(){
        if(!skillLevelUpUIActive)
            UIManager.instance.ShiftStatusEffects(new Vector2(levelInfo.xShift, levelInfo.yShift), player.playerUI);
        // If a level up or skill point was used since the last UI update then update the UI.
        if(newLevel){
            UIManager.instance.SetSkillLevelUpActive(spellLevels, level, true, player.playerUI);
            skillLevelUpUIActive = true;
            newLevel = false;
        }
        if(ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion] == player.gameObject){
            // If first spell level up key bind pressed and it is not at max level then level it.
            if(Input.GetKey(KeyCode.LeftControl)){
                if(Input.GetKeyDown(KeyCode.Q))
                    SpellLevelUpRequest("Spell_1", "basic");
                // If second spell level up key bind pressed and it is not at max level then level it.
                else if(Input.GetKeyDown(KeyCode.W))
                    SpellLevelUpRequest("Spell_2", "basic");
                // If third spell level up key bind pressed and it is not at max level then level it.
                else if(Input.GetKeyDown(KeyCode.E))
                    SpellLevelUpRequest("Spell_3", "basic");
                // If fourth spell level up key bind pressed and it is not at max level then level it.
                else if(Input.GetKeyDown(KeyCode.R))
                    SpellLevelUpRequest("Spell_4", "ultimate");
            }
        }
        // Skill level up available animation.
        UIManager.instance.SkillLevelUpGradient(player.playerUI);
    }

    /*
    * DeactivateSkillLevelUpUI - Sets the skill level up UI active state to false and shifts the status effects back.
    */
    public void DeactivateSkillLevelUpUI(){
        skillLevelUpUIActive = false;
        // Disable skill level up UI.
        UIManager.instance.SetSkillLevelUpActive(spellLevels, level, false, player.playerUI);
        // Shift effects UI back.
        UIManager.instance.ShiftStatusEffects(new Vector2(-(levelInfo.xShift), -(levelInfo.yShift)), player.playerUI);
    }

    /*
    *   SpellLevelUpRequest - Checks if the requested spell can be leveled up and calls the level up method.
    *   @param spell - string of the spell requested to be leveled up.
    *   @param type - string of the type of spell to distinguish between 5 point vs 3 point spells (basic vs ultimate).
    */
    private void SpellLevelUpRequest(string spell, string type){
        // Basic ability.
        if(type == "basic"){
            if(spellLevels[spell] != levelInfo.maxSpellLevel)
                SpellLevelUp(spell);
        }
        // Ultimate ability.
        else{
            if(spellLevels[spell] != levelInfo.maxUltLevel){
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
    public void SpellLevelUp(string spell){
        spellLevels[spell] = spellLevels[spell] + 1;
        spellLevelPoints -= 1;
        newLevel = true;
        // If the spell wasn't level 1 yet then take of the spell unlearned cover.
        if(spellLevels[spell] == 1)
            UIManager.instance.SpellLearned(spell, player.playerUI);
        else
            UIManager.instance.SpellLeveled(spell, spellLevels[spell], player.playerUI);
    }

    /*
    *   RespawnTime - Calculates the respawn time based on the players level.
    *   @return float - Time to wait before respawning this player.
    */
    public float RespawnTime(){
        float respawnTime;
        if(level < 7){
            respawnTime = (level * 2f) + 4f;
        }
        else if(level == 7){
            respawnTime = 21f;
        }
        else{
            respawnTime = (level * 2.5f) + 7.5f;
        }
        return respawnTime;
    }
}
