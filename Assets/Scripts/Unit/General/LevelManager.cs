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
    public Dictionary<string, int> spellLevels { get; } = new Dictionary<string, int>();

    private Slider xpSlider = null;
    private TMP_Text playerUILevelText = null;
    private TMP_Text playerBarLevelText = null;
    private Transform spellsContainer = null;
    private RectTransform statusEffectsRect = null;
    private Gradient gradient;
    private int level = 1;
    public int Level { 
        get => level; 
        private set {
            level = value;
            if(playerUILevelText != null)
                playerUILevelText.SetText(level.ToString());
            if(playerBarLevelText != null)
                playerBarLevelText.SetText(level.ToString());
        }
    }
    private int spellLevelPoints = 0;
    public int SpellLevelPoints {
        get => spellLevelPoints;
        private set {
            if(spellsContainer != null){
                if(value > 0){
                    SetSkillLevelUpActive(true);
                    if(spellLevelPoints == 0)
                        statusEffectsRect.anchoredPosition += new Vector2(levelInfo.xShift, levelInfo.yShift);
                }
                else{
                    SetSkillLevelUpActive(false);
                    statusEffectsRect.anchoredPosition -= new Vector2(levelInfo.xShift, levelInfo.yShift);
                }
            }
            spellLevelPoints = value;
        }
    }
    private float gainAmount = 100f;
    private float currentXP = 0f;
    public float CurrentXP {
        get => currentXP;
        private set {
            currentXP = value;
            if(xpSlider != null){
                if(Level != levelInfo.maxLevel)
                    xpSlider.value = Mathf.Round((currentXP/levelInfo.requiredXP[Level]) * 100f);
                else
                    xpSlider.value = 100f;
            }
        }
    }
    private LevelInfo levelInfo;
    private IUnit _unit;
    
    /*
    *   LevelManager - Initializes a new level manager.
    *   @param plater - Player this level manager is for.
    *   @param levelInfo - LevelInfo to use for leveling information/constants.
    */
    public LevelManager(IUnit unit){
        if(unit is IPlayer){
            IPlayer player = (IPlayer) unit;
            if(player.playerUI != null && player.playerBar != null){
                xpSlider = player.playerUI.transform.Find("Player/Info/PlayerContainer/InnerContainer/Experience").GetComponent<Slider>();
                playerUILevelText = player.playerUI.transform.Find("Player/Info/PlayerContainer/InnerContainer/IconContainer/Level/Value").GetComponent<TMP_Text>();
                spellsContainer = player.playerUI.transform.Find("Player/Combat/SpellsContainer/");
                statusEffectsRect = player.playerUI.transform.Find("Player/StatusEffects").GetComponent<RectTransform>();
                playerBarLevelText = player.playerBar.transform.Find("PlayerBar/Container/Level/Value").GetComponent<TMP_Text>();
            }
        }
        SetUpGradient();
        levelInfo = ScriptableObject.CreateInstance<LevelInfo>();
        _unit = unit;
        // Set up spell levels dictionary.
        for(int i = 0; i < 4; i++)
            spellLevels.Add("Spell_" + (i+1), 0);
        if(unit is IPlayer)
            if(((IPlayer) unit).score != null)
                ((IPlayer) unit).score.takedownCallback += GainXP; 
        SpellLevelPoints += 1;
    }

    public LevelManager(IUnit unit, int startingLevel){
        levelInfo = ScriptableObject.CreateInstance<LevelInfo>();
        _unit = unit;
        // Set up spell levels dictionary.
        for(int i = 0; i < 4; i++)
            spellLevels.Add("Spell_" + (i+1), 0);
        if(unit is IPlayer)
            if(((IPlayer) unit).score != null)
                ((IPlayer) unit).score.takedownCallback += GainXP;
        Level = startingLevel;
        SpellLevelPoints += startingLevel;
    }

    /*
    *   Gain XPTester - Add xp to the champions total. Used for testing with a key input.
    *   @param gained - float of the amount of xp to add.
    */
    public void GainXPTester(){
        CurrentXP += gainAmount;
        if(Level != levelInfo.maxLevel){
            if(CurrentXP >= levelInfo.requiredXP[Level])
                LevelUp();
        }
    }

    /*
    *   GainXP - Add xp to the champions total.
    *   @param killed - IUnit of the killed unit.
    */
    public void GainXP(IUnit killed){
        float gained;
        if(killed is IPlayer)
            gained = levelInfo.championKillXP[((IPlayer) killed).levelManager.Level - 1];
        else
            gained = levelInfo.defaultXP;
        CurrentXP += gained;
        if(CurrentXP >= levelInfo.requiredXP[Level] && Level != levelInfo.maxLevel){
            LevelUp();
        }
    }

    /*
    *   LevelUp - Level up the champion. This includes increasing level, spell levels points, next level xp, and champion stats.
    *
    */
    public void LevelUp(){
        // Keep any overflow xp for the next.
        CurrentXP = CurrentXP - levelInfo.requiredXP[Level];
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
            SkillLevelUpGradient(currentTime);
        }
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
                if((spell_4_level < 1 && Level > 5) || (spell_4_level < 2 && Level > 10) || (spell_4_level < 3 && Level > 15))
                    SpellLevelUp(spell);
            }
        }
    }

    /*
    *   SpellLevelUp - Levels up the given spell and updates the UI.
    *   @paream spell - string of the spell to be leveled up.
    */
    public void SpellLevelUp(string spell){
        if(spellLevelPoints > 0){
            spellLevels[spell] = spellLevels[spell] + 1;
            SpellLevelPoints -= 1;
            // If the spell wasn't level 1 yet then take of the spell unlearned cover.
            if(spellLevels[spell] == 1)
                SpellLearned(spell);
            else
                SpellLeveled(spell, spellLevels[spell]);
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

    /*
    *   SetSkillLevelUpActive - Activates or deactivates spell level up buttons.
    *   @param isActive - bool of wether or not to active the buttons.
    */
    public void SetSkillLevelUpActive(bool isActive){
        // For each spell.
        foreach(KeyValuePair<string, int> spell in spellLevels){
            string find = spell.Key + "_Container";
            GameObject spellLevelUpObj = spellsContainer.Find(find + "/LevelUp").gameObject;
            // If the UI should be active.
            if(isActive){
                // If the kvp is spell 4.
                if(spell.Key == "Spell_4"){
                    int spell_4_level = spellLevels["Spell_4"];
                    // If spell 4 can be leveled.
                    if((spell_4_level < 1 && Level > 5) || (spell_4_level < 2 && Level > 10) || (spell_4_level < 3 && Level > 15)){
                        spellLevelUpObj.SetActive(true);
                    }
                    else
                        spellLevelUpObj.SetActive(false);
                }
                // If a basic spell then activate its level up available UI if it isn't max spell level.
                else{
                    if(spell.Value < 5){
                        spellLevelUpObj.SetActive(true);  
                    }
                    else{
                        spellLevelUpObj.SetActive(false);
                    }
                }
            }
            else{
                spellLevelUpObj.SetActive(false);  
            }
        }
    }

    /*
    *   SpellLearned - Removes the spell cover the first time a spell is leveled.
    *   @param spell - string of the spell number.
    */
    private void SpellLearned(string spell){
        if(spellsContainer != null){
            Transform spellCover = spellsContainer.Find(spell + "_Container/SpellContainer/Spell/CD/Cover");
            spellCover.gameObject.SetActive(false);
        }
        SpellLeveled(spell, 1);
    }

    /*
    *   SpellLearned - Updates the UI to show a spell was leveled.
    *   @param spell - string of the spell number.
    *   @param spellLevel - int of the spells new level.
    */
    private void SpellLeveled(string spell, int spellLevel){
        if(spellsContainer != null){
            Transform spellLevels = spellsContainer.Find(spell + "_Container/Levels");
            spellLevels.Find("Level" + spellLevel + "/Fill").gameObject.SetActive(false);
        }
    }

    /*
    *   SetUpGradient - Creates a new Gradient object to use for animating the spell level up buttons.
    */
    private void SetUpGradient(){
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;
        Color defaultBorderColor = new Color(167f/255f, 126f/255f, 69f/255f);
        gradient = new Gradient();
        // Two color gradient.
        colorKey = new GradientColorKey[2];
        // Set colors and set their time to opposite ends.
        colorKey[0].color = new Color(167f/255f, 126f/255f, 69f/255f);
        colorKey[0].time = 0.0f;
        colorKey[1].color = new Color(230f/255f, 219f/255f, 204f/255f);
        colorKey[1].time = 1.0f;
        // One alpha gradient.
        alphaKey = new GradientAlphaKey[1];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        // Set the gradient.
        gradient.SetKeys(colorKey, alphaKey);
    }

    /*
    *   SkillLevelUpGradient - Animates the spell level up buttons.
    *   @param currentTime - float of the current game time.
    */
    public void SkillLevelUpGradient(float currentTime){
        // For each spell.
        for(int i = 0; i < 4; i++){
            if(spellsContainer != null){
                spellsContainer.Find("Spell_" + (i+1) + "_Container/LevelUp/Background")
                .gameObject.GetComponent<Image>().color = gradient.Evaluate(Mathf.PingPong(currentTime, 1));
            }
        }
    }
}
