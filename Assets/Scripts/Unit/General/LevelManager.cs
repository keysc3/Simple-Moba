using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
* Purpose: Implements a leveling system for champions.
*
* @author: Colin Keys
*/
public class LevelManager
{
    public Dictionary<SpellType, int> spellLevels { get; } = new Dictionary<SpellType, int>{{SpellType.Spell1, 0}, {SpellType.Spell2, 0}, {SpellType.Spell3, 0}, {SpellType.Spell4, 0}};

    private int level = 1;
    public int Level { 
        get => level; 
        private set => level = value;
    }
    private int spellLevelPoints = 0;
    public int SpellLevelPoints {
        get => spellLevelPoints;
        private set {
            spellLevelPoints = value;
            SkillPointsCallback?.Invoke(this);
        }
    }
    private float gainAmount = 100f;
    private float currentXP = 0f;
    public float CurrentXP {
        get => currentXP;
        private set {
            currentXP = value;
            if(value >= levelInfo.requiredXP[Level])
                LevelUp();
            if(level != levelInfo.maxLevel)
                UpdateExperienceCallback?.Invoke(currentXP, levelInfo.requiredXP[Level]);
            else
                UpdateExperienceCallback?.Invoke(1f, 1f);
        }
    }
    private LevelInfo levelInfo;
    private IUnit _unit;

    public delegate void UpdateLevelUI(int level);
    public event UpdateLevelUI UpdateLevelCallback;

    public delegate void UpdateExperienceUI(float current, float required);
    public event UpdateExperienceUI UpdateExperienceCallback;

    public delegate void UpdateSkillPointsUI(LevelManager levelManager);
    public event UpdateSkillPointsUI SkillPointsCallback;

    public delegate void UpdateLevelUpUI(SpellType spell, int level);
    public event UpdateLevelUpUI SpellLevelUpCallback;

    public delegate void SkillPointAvailable(float currentTime);
    public event SkillPointAvailable SkillPointAvailableCallback;
    
    /*
    *   LevelManager - Initializes a new level manager.
    *   @param plater - Player this level manager is for.
    *   @param levelInfo - LevelInfo to use for leveling information/constants.
    */
    public LevelManager(IUnit unit){
        levelInfo = ScriptableObject.CreateInstance<LevelInfo>();
        _unit = unit;
        if(unit is IPlayer)
            if(((IPlayer) unit).score != null)
                ((IPlayer) unit).score.takedownCallback += GainXP; 
        SpellLevelPoints += 1;
    }

    public LevelManager(IUnit unit, int startingLevel){
        levelInfo = ScriptableObject.CreateInstance<LevelInfo>();
        _unit = unit;
        if(unit is IPlayer)
            if(((IPlayer) unit).score != null)
                ((IPlayer) unit).score.takedownCallback += GainXP;
        SpellLevelPoints += 1;
        if(startingLevel < 1)
            startingLevel = 1;
        else if(startingLevel > 18)
            startingLevel = 18;
        for(int i = 1; i < startingLevel; i++){
            LevelUp();
        }
    }

    /*
    *   Gain XPTester - Add xp to the champions total. Used for testing with a key input.
    *   @param gained - float of the amount of xp to add.
    */
    public void GainXPTester(){
        if(Level != levelInfo.maxLevel)
            CurrentXP += gainAmount;
    }

    /*
    *   GainXP - Add xp to the champions total.
    *   @param killed - IUnit of the killed unit.
    */
    public void GainXP(IUnit killed){
        if(Level != levelInfo.maxLevel){
            float gained;
            if(killed is IPlayer)
                gained = levelInfo.championKillXP[((IPlayer) killed).levelManager.Level - 1];
            else
                gained = levelInfo.defaultXP;
            CurrentXP += gained;
        }
    }

    /*
    *   LevelUp - Level up the champion. This includes increasing level, spell levels points, next level xp, and champion stats.
    *
    */
    public void LevelUp(){
        // Keep any overflow xp for the next.
        if(CurrentXP > levelInfo.requiredXP[Level])
            CurrentXP = CurrentXP - levelInfo.requiredXP[Level];
        else
            CurrentXP = 0;
        // Increase level and required amount for the next level.
        Level++;
        //requiredXP += 100.0f;
        gainAmount += 100.0f;
        // Give the champion another skill level up point and start the coroutine to allow them to level spells if not already running.
        SpellLevelPoints += 1;
        // Increase the champions base stats.
        if(_unit is IPlayer)
            if(_unit.unitStats != null)
                IncreaseChampionStats((ChampionStats) _unit.unitStats, (ScriptableChampion) _unit.SUnit);
        UpdateLevelCallback?.Invoke(level);
    }

    /*
    *   IncreaseChampionStats - Increases the base stats of the champion based on their level growth stat for each stat.
    */
    private void IncreaseChampionStats(ChampionStats championStats, ScriptableChampion champion){
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
    *   @param growthStat - float of the growth value of the stat.
    */
    private float GrowthAmountCalculation(float growthStat){
        return growthStat * (0.65f + 0.035f * level);
    }

    /*
    *   GrowthAmountCalculationAtkSpd - Calculates the amount to increase attack speed by based on level from level up.
    *   @param growthStat - float of the growth value of the stat.
    */
    private float GrowthAmountCalculationAtkSpd(float growthStat){
        return growthStat * (Level - 1) * (0.7025f + (0.0175f * (Level - 1)));
    }

    /*
    *   LevelUpSkill - Calls a level up request for pressed spell and calls level up animation method.
    *   @param currentTime - float of current game time.
    */
    public void LevelUpSkill(float currentTime){
        // Check for level up skill input if skill level up available.
        if(spellLevelPoints > 0){
            if(ActiveChampion.instance.players[ActiveChampion.instance.ActiveChamp] == (IPlayer) _unit){
                // If first spell level up key bind pressed and it is not at max level then level it.
                if(Input.GetKey(KeyCode.LeftControl)){
                    if(Input.GetKeyDown(KeyCode.Q))
                        SpellLevelUpRequest(SpellType.Spell1);
                    // If second spell level up key bind pressed and it is not at max level then level it.
                    else if(Input.GetKeyDown(KeyCode.W))
                        SpellLevelUpRequest(SpellType.Spell2);
                    // If third spell level up key bind pressed and it is not at max level then level it.
                    else if(Input.GetKeyDown(KeyCode.E))
                        SpellLevelUpRequest(SpellType.Spell3);
                    // If fourth spell level up key bind pressed and it is not at max level then level it.
                    else if(Input.GetKeyDown(KeyCode.R))
                        SpellLevelUpRequest(SpellType.Spell4);
                }
            }
            // Skill level up available animation.
            SkillPointAvailableCallback?.Invoke(currentTime);
        }
    }

    /*
    *   SpellLevelUpRequest - Checks if the requested spell can be leveled up and calls the level up method.
    *   @param spell - string of the spell requested to be leveled up.
    */
    public void SpellLevelUpRequest(SpellType spell){
        // Basic ability.
        if(spell != SpellType.Spell4){
            if(spellLevels[spell] != levelInfo.maxSpellLevel)
                SpellLevelUp(spell);
        }
        // Ultimate ability.
        else{
            if(spellLevels[spell] != levelInfo.maxUltLevel){
                int spell_4_level = spellLevels[SpellType.Spell4];
                // Allow one point in fourth spell (ultimate) at level 6-10, two points at 11-15, and three points at 16-18.
                if((spell_4_level < 1 && Level > 5) || (spell_4_level < 2 && Level > 10) || (spell_4_level < 3 && Level > 15))
                    SpellLevelUp(spell);
            }
        }
    }

    /*
    *   SpellLevelUp - Levels up the given spell and updates the UI.
    *   @param spell - string of the spell to be leveled up.
    */
    private void SpellLevelUp(SpellType spell){
        if(spellLevelPoints > 0){
            spellLevels[spell] = spellLevels[spell] + 1;
            SpellLevelPoints -= 1;
            SpellLevelUpCallback?.Invoke(spell, spellLevels[spell]);
        }
    }

    /*
    *   RespawnTime - Calculates the respawn time based on the players level.
    *   @return float - Time to wait before respawning this player.
    */
    public float RespawnTime(){
        float respawnTime;
        if(Level < 7){
            respawnTime = (Level * 2f) + 4f;
        }
        else if(Level == 7){
            respawnTime = 21f;
        }
        else{
            respawnTime = (Level * 2.5f) + 7.5f;
        }
        return respawnTime;
    }
}
