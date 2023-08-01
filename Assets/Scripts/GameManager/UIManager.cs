using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
* Purpose: Handles updating the UI.
*
* @author: Colin Keys
*/
public class UIManager : MonoBehaviour
{

    public static UIManager instance { get; private set; }

    [SerializeField] private GameObject playerBarPrefab;
    [SerializeField] private GameObject statusEffectPrefab;
    [SerializeField] private GameObject playerUIPrefab;
    private float buffDebuffUIWidth;
    private float xOffset = 2f;

    private Gradient gradient;
    private GradientColorKey[] colorKey;
    private GradientAlphaKey[] alphaKey;
    private Color defaultBorderColor;


    // Called when the script instance is being loaded.
    void Awake(){
        instance = this;
        defaultBorderColor = new Color(167f/255f, 126f/255f, 69f/255f);
        SetUpGradient();
        buffDebuffUIWidth = playerUIPrefab.transform.Find("Player/StatusEffects/BuffsContainer").GetComponent<RectTransform>().rect.width;
    }

    // Start is called before the first frame update.
    void Start(){

    }

    public void SetupPlayerUI(Player player, GameObject playerUI, GameObject playerBar){
        UpdateHealthBar(player, playerUI, playerBar);
        UpdateManaBar((ChampionStats) player.unitStats, playerUI, playerBar);
        UpdateAllStats((ChampionStats) player.unitStats, playerUI);
    }

    public GameObject CreatePlayerHUD(GameObject champion, GameObject championUI, Player player){
        // Set up the players HUD.
        GameObject newChampionUI = (GameObject) Instantiate(championUI, championUI.transform.position, championUI.transform.rotation);
        newChampionUI.name = champion.name + "UI";
        newChampionUI.transform.SetParent(GameObject.Find("/Canvas").transform);
        RectTransform newChampionUIRectTransform = newChampionUI.GetComponent<RectTransform>();
        newChampionUIRectTransform.offsetMin = new Vector2(0, 0);
        newChampionUIRectTransform.offsetMax = new Vector2(0, 0);
        // Set up the players health/mana/xp bar above their GameObject.
        return newChampionUI;
    }

    public GameObject CreatePlayerUI(GameObject champion, Player player){
        // Set up the players HUD.
        GameObject newPlayerUI = (GameObject) Instantiate(playerUIPrefab, playerUIPrefab.transform.position, playerUIPrefab.transform.rotation);
        newPlayerUI.name = champion.name + "UI";
        newPlayerUI.transform.SetParent(GameObject.Find("/Canvas").transform);
        RectTransform newPlayerUIRectTransform = newPlayerUI.GetComponent<RectTransform>();
        newPlayerUIRectTransform.offsetMin = new Vector2(0, 0);
        newPlayerUIRectTransform.offsetMax = new Vector2(0, 0);
        newPlayerUI.transform.Find("Player/Info/PlayerContainer/InnerContainer/IconContainer/Icon").GetComponent<Image>().sprite = player.unit.icon;
        SetupSpellUI(newPlayerUI, champion, player);
        // Set up the players health/mana/xp bar above their GameObject.
        return newPlayerUI;
    }

    public void SetupSpellUI(GameObject playerUI, GameObject champion, Player player){
        GameObject spellContainer = playerUI.transform.Find("Player/Combat/SpellsContainer").gameObject;
        ChampionSpells championSpells = champion.GetComponent<ChampionSpells>();
        List<KeyCode> inputs = new List<KeyCode>(){KeyCode.None, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R};
        // Setup spell1-4's spell button.
        for(int i = 0; i < 5; i++){
            SpellButton spellButton = spellContainer.transform.GetChild(i).Find("SpellContainer/Spell/Button").GetComponent<SpellButton>();
            spellButton.spell = championSpells.mySpells[i];
            spellButton.keyCode = inputs[i];
            spellButton.playerSpellInput = champion.GetComponent<PlayerSpellInput>();
            spellContainer.transform.GetChild(i).Find("SpellContainer/Spell/Icon").GetComponent<Image>().sprite = championSpells.mySpellData[i].sprite;
            spellButton.spell = championSpells.mySpells[i];
            // Setup spell1-4's level up buttons.
            if(i > 0){
                SpellLevelUpButton spellLevelUpButton = spellContainer.transform.GetChild(i).Find("LevelUp/Button").GetComponent<SpellLevelUpButton>();
                spellLevelUpButton.playerSpellInput = champion.GetComponent<PlayerSpellInput>();
                spellLevelUpButton.spell = championSpells.mySpells[i].spellNum;
                spellLevelUpButton.player = player;
            }
        }
    }

    public GameObject CreatePlayerBar(GameObject champion){
        GameObject newPlayerBar = (GameObject) Instantiate(playerBarPrefab, playerBarPrefab.transform.position, playerBarPrefab.transform.rotation);
        newPlayerBar.name = champion.name + "PlayerBar";
        RectTransform newPlayerBarRectTransform = newPlayerBar.GetComponent<RectTransform>();
        Vector3 newPlayerBarPos = newPlayerBarRectTransform.anchoredPosition;
        newPlayerBar.transform.SetParent(champion.transform);
        newPlayerBarRectTransform.anchoredPosition3D = newPlayerBarPos;
        return newPlayerBar;
    }

    /*
    *   UpdateHealthBar - Updates the health bar UI.
    */
    public void UpdateHealthBar(Player player, GameObject championUI, GameObject playerBar){
        Slider health = championUI.transform.Find("Player/Combat/ResourceContainer/HealthContainer/HealthBar").GetComponent<Slider>();
        TMP_Text healthText = health.transform.Find("Value").GetComponent<TMP_Text>();
        // If the champion is dead.
        if(!player.isDead){
            ChampionStats championStats = (ChampionStats) player.unitStats;
            // Get the health percent the player is at and set the health bar text to currenthp/maxhp.
            float healthPercent = Mathf.Round((championStats.currentHealth/championStats.maxHealth.GetValue()) * 100);
            healthText.SetText(Mathf.Ceil(championStats.currentHealth) + "/" + Mathf.Ceil(championStats.maxHealth.GetValue()));
            // Set the fill based on players health percent.
            playerBar.transform.Find("PlayerBar/Container/Health").GetComponent<Slider>().value = healthPercent;
            health.value = healthPercent;
        }
        else{
            // Set players health text and fill to 0.
            ChampionStats championStats = (ChampionStats) player.unitStats;
            playerBar.SetActive(false);
            healthText.SetText(0 + "/" + Mathf.Ceil(championStats.maxHealth.GetValue()));
            health.value = 0;
        }
    }

    /*
    *   UpdateManaBar - Updates the mana bar UI.
    */
    public void UpdateManaBar(ChampionStats championStats, GameObject championUI, GameObject playerBar){
        Slider mana = championUI.transform.Find("Player/Combat/ResourceContainer/ManaContainer/ManaBar").GetComponent<Slider>();
        // Get the percent of mana the player has left and set the mana bar text to currentmana/maxmana
        float manaPercent = Mathf.Round((championStats.currentMana/championStats.maxMana.GetValue()) * 100);
        mana.transform.Find("Value").GetComponent<TMP_Text>()
        .SetText(Mathf.Ceil(championStats.currentMana) + "/" + Mathf.Ceil(championStats.maxMana.GetValue()));
        // Set the fill based on the player mana percent.
        playerBar.transform.Find("PlayerBar/Container/Mana").GetComponent<Slider>().value = manaPercent;
        mana.value = manaPercent;
    }
    
    public void UpdateAllStats(ChampionStats championStats, GameObject playerUI){
        Transform statsContainer = playerUI.transform.Find("Player/Info/Stats/Container");
        UpdateStat("PhysicalDamage", championStats.physicalDamage.GetValue(), statsContainer);
        UpdateStat("Armor", championStats.armor.GetValue(), statsContainer);
        if(championStats.attackSpeed.GetValue() > 2.5f)
            UpdateStat("AttackSpeed", 2.5f, statsContainer);
        else
            UpdateStat("AttackSpeed", championStats.attackSpeed.GetValue(), statsContainer);
        UpdateStat("Crit", 0f, statsContainer);
        UpdateStat("MagicDamage", championStats.magicDamage.GetValue(), statsContainer);
        UpdateStat("MagicResist", championStats.magicResist.GetValue(), statsContainer);
        UpdateStat("Haste", championStats.haste.GetValue(), statsContainer);
        UpdateStat("Speed", championStats.speed.GetValue(), statsContainer);
    }

    public void UpdateStat(string statName, float value, Transform statsContainer){
        if(statName != "AttackSpeed")
            statsContainer.Find(statName).Find("Value").GetComponent<TMP_Text>().SetText(Mathf.Round(value).ToString());
        else
            statsContainer.Find(statName).Find("Value").GetComponent<TMP_Text>().SetText((Mathf.Round(value * 100f) * 0.01f).ToString());
    }

    /*
    *   SetUpGradient - Creates the gradient for the skill level up available animation.
    */
    public void SetUpGradient(){
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
    *   UpdateCooldown - Updates the UI text with the current cooldown left on a spell.
    *   @param spell - string of the spell to update.
    *   @param cooldownLeft - string of the amount of time left on the spells cooldown.
    *   @param totalCooldown - float of the total cooldown duration of the spell.
    */
    public void UpdateCooldown(string spell, float cooldownLeft, float totalCooldown, GameObject playerUI){
        Transform spellCD = playerUI.transform.Find("Player/Combat/SpellsContainer/" + spell + "_Container/SpellContainer/Spell/CD");
        // Set the cooldown panel children to be active.
        ChildrenSetActive(spellCD, true);

        TMP_Text text = spellCD.Find("Value").GetComponent<TMP_Text>();
        // If off cooldown.
        if(cooldownLeft == 0f)
            ChildrenSetActive(spellCD, false);
        else{
            // Update the UI cooldown text and slider.
            text.SetText(Mathf.Ceil(cooldownLeft).ToString());
            float fill = Mathf.Clamp(cooldownLeft/totalCooldown, 0f, 1f);
            spellCD.Find("Slider").GetComponent<Image>().fillAmount = fill;
        }
    }

    /*
    *   OffCooldown - Sets the spells cooldown UI to inactive.
    *   @param OffCooldown - transform of the spell cover to turn off.
    */
    public void ChildrenSetActive(Transform parent, bool isActive){
        for(int i = 0; i < parent.childCount; i++){
            parent.GetChild(i).gameObject.SetActive(isActive);
        }
    }

    /*
    *   SpellLearned - Sets the spell cover for the given spell to inactive.
    *   @param spell - string of the spell that was learned.
    */
    public void SpellLearned(string spell, GameObject playerUI){
        Transform spellCover = playerUI.transform.Find("Player/Combat/SpellsContainer/" + spell + "_Container/SpellContainer/Spell/CD/Cover");
        spellCover.gameObject.SetActive(false);
        SpellLeveled(spell, 1, playerUI);
    }
    
    /*
    *   SpellLeveled - Updates the UI to show the given spells new level.
    *   @param spell - string of the spell that was leveled.
    *   @param spellLevel - int of the new level of the spell.
    */
    public void SpellLeveled(string spell, int spellLevel, GameObject playerUI){
        Transform spellLevels = playerUI.transform.Find("Player/Combat/SpellsContainer/" + spell + "_Container/Levels");
        spellLevels.Find("Level" + spellLevel + "/Fill").gameObject.SetActive(false);
    }

    /*
    *   UpdateHealthRegen - Updates the health regen text UI.
    *   @param healthRegen - float of the value to update the text to.
    */
    public void UpdateHealthRegen(float healthRegen, GameObject playerUI){
        Slider health = playerUI.transform.Find("Player/Combat/ResourceContainer/HealthContainer/HealthBar").GetComponent<Slider>();
        health.transform.Find("Regen").GetComponent<TMP_Text>().SetText("+" + Mathf.Round(healthRegen * 100.0f) * 0.01f);
    }

    /*
    *   SetHealthRegenActive - Updates the health regen UI to be showing or not.
    *   @param isActive - bool of whether to active the UI or not.
    */
    public void SetHealthRegenActive(bool isActive, GameObject playerUI){
        Slider health = playerUI.transform.Find("Player/Combat/ResourceContainer/HealthContainer/HealthBar").GetComponent<Slider>();
        health.transform.Find("Regen").gameObject.SetActive(isActive);
    }

    /*
    *   SetManaRegenActive - Updates the mana regen UI to be showing or not.
    *   @param isActive - bool of whether to active the UI or not.
    */
    public void SetManaRegenActive(bool isActive, GameObject playerUI){
        Slider mana = playerUI.transform.Find("Player/Combat/ResourceContainer/ManaContainer/ManaBar").GetComponent<Slider>();
        mana.transform.Find("Regen").gameObject.SetActive(isActive);
    }

    /*
    *   UpdateManaRegen - Updates the mana regen text UI.
    *   @param manaRegen - float of the value to update the text to.
    */
    public void UpdateManaRegen(float manaRegen, GameObject playerUI){
        Slider mana = playerUI.transform.Find("Player/Combat/ResourceContainer/ManaContainer/ManaBar").GetComponent<Slider>();
        mana.transform.Find("Regen").GetComponent<TMP_Text>().SetText("+" + Mathf.Round(manaRegen * 100.0f) * 0.01f);
    }

    /*
    *   SetSkillLevelUpActive - Sets the skill level up available to active or not for each spell.
    *   @param spellLists - Dictionary of <string, int> pairs representing spell:level.
    *   @param level - int of the players level.
    *   @param isActive - bool of whether to set the UI elements active or not.
    */
    public void SetSkillLevelUpActive(Dictionary<string, int> spellLevels, int level, bool isActive, GameObject playerUI){
        // For each spell.
        foreach(KeyValuePair<string, int> spell in spellLevels){
            string find = spell.Key + "_Container";
            GameObject spellLevelUpObj = playerUI.transform.Find("Player/Combat/SpellsContainer/" + find + "/LevelUp").gameObject;
            // If the UI should be active.
            if(isActive){
                // If the kvp is spell 4.
                if(spell.Key == "Spell_4"){
                    int spell_4_level = spellLevels["Spell_4"];
                    // If spell 4 can be leveled.
                    if((spell_4_level < 1 && level > 5) || (spell_4_level < 2 && level > 10) || (spell_4_level < 3 && level > 15)){
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
    * SkillLevelUpGradient - Increments the skill level up available gradient time to animate it pulsing.
    */
    public void SkillLevelUpGradient(GameObject playerUI){
        float value = Time.time;
        // For each spell.
        for(int i = 0; i < 4; i++){
            playerUI.transform.Find("Player/Combat/SpellsContainer/Spell_" + (i+1) + "_Container/LevelUp/Background")
            .gameObject.GetComponent<Image>().color = gradient.Evaluate(Mathf.PingPong(value, 1));
        }
    }

    /*
    *   SetSpellActiveDuration - Animates the border of a spell using a slider to represent the spells active duration left.
    *   @param spell - int of the spell number that is active.
    *   @param duration - float of the total duration of the spell.
    *   @param active - float of the amount of time the spell has been active so far.
    */
    public void SetSpellActiveDuration(string spell, float duration, float active, GameObject playerUI){
        // Get value between 0 and 1 representing the percent of the spell duration left.
        float fill = 1.0f - (active/duration);
        fill = Mathf.Clamp(fill, 0f, 1f);
        // Set the fill on the active spells slider.
        GameObject spellDurationSlider = playerUI.transform.Find("Player/Combat/SpellsContainer/" + spell + "_Container/SpellContainer/Outline/Slider").gameObject;
        spellDurationSlider.SetActive(true);
        spellDurationSlider.transform.Find("Fill").GetComponent<Image>().fillAmount = fill;
    }
    
    /*
    *   SetSpellDurationOver - Set the spells active slider to inactive.
    *   @param spell - int of the spell to deactivate the UI for.
    */
    public void SetSpellDurationOver(string spell, GameObject playerUI){
        playerUI.transform.Find("Player/Combat/SpellsContainer/" + spell + "_Container/SpellContainer/Outline/Slider").gameObject.SetActive(false);
    }

    /*
    *   SetSpellCoverActive - Sets the spells UI cover to active or not.
    *   @param spell - int of the spell to update the UI for.
    *   @param isActive - bool of whether or not to activate the spell cover.
    */
    public void SetSpellCoverActive(string spell, bool isActive, GameObject playerUI){
        playerUI.transform.Find("Player/Combat/SpellsContainer/" + spell + "_Container/SpellContainer/Spell/CD/Cover").gameObject.SetActive(isActive);
    }

    /*
    *   UpdateExperienceBar - Updates the experience slider to the percent of experience earned towards the next level.
    *   @param currentXP - float of the players current experience.
    *   @param requiredXP float of the total required experience for the players next level.
    */
    public void UpdateExperienceBar(float currentXP, float requiredXP, GameObject playerUI){
        playerUI.transform.Find("Player/Info/PlayerContainer/InnerContainer/Experience").GetComponent<Slider>().value = Mathf.Round((currentXP/requiredXP) * 100);
    }

    /*
    *   UpdateLevelText - Updates the player level UI text.
    *   @param currentLevel - int of the players current level.
    */
    public void UpdateLevelText(int currentLevel, GameObject playerUI, GameObject playerBar){
        playerUI.transform.Find("Player/Info/PlayerContainer/InnerContainer/IconContainer/Level/Value").GetComponent<TMP_Text>().SetText(currentLevel.ToString());
        playerBar.transform.Find("PlayerBar/Container/Level/Value").GetComponent<TMP_Text>().SetText(currentLevel.ToString());
    }

    /*
    *   UpdateDeathTimer - Updates the death timer UI of the player.
    *   @param timeLeft - float of the time left before respawn.
    */
    public void UpdateDeathTimer(float timeLeft, GameObject playerUI){
        GameObject iconCover = playerUI.transform.Find("Player/Info/PlayerContainer/InnerContainer/IconContainer/IconCover").gameObject;
        // Active icon cover if not already activated.
        if(!iconCover.activeSelf)
            iconCover.SetActive(true);
        if(timeLeft != 0f){
            iconCover.transform.Find("DeathTimer").GetComponent<TMP_Text>().SetText(Mathf.Ceil(timeLeft).ToString());
        }
        else{
            iconCover.SetActive(false); 
        }
    }

    /*
    *   UpdateKills - Updates the UI text with the players current amount of kills.
    *   @param kills - string of the amount of kills to update to.
    */
    public void UpdateKills(string kills, GameObject playerUI){
        TMP_Text killsText = playerUI.transform.Find("Score/Container/Kills/Value").GetComponent<TMP_Text>();
        killsText.SetText(kills);
    }

    /*
    *   UpdateDeaths - Updates the UI text with the players current amount of deaths.
    *   @param deaths - string of the amount of deaths to update to.
    */
    public void UpdateDeaths(string deaths, GameObject playerUI){
        TMP_Text deathsText = playerUI.transform.Find("Score/Container/Deaths/Value").GetComponent<TMP_Text>();
        deathsText.SetText(deaths);
    }

    /*
    *   UpdateAssists - Updates the UI text with the players current amount of assists.
    *   @param assists - string of the amount of assists to update to.
    */
    public void UpdateAssists(string assists, GameObject playerUI){
        TMP_Text assistsText = playerUI.transform.Find("Score/Container/Assists/Value").GetComponent<TMP_Text>();
        assistsText.SetText(assists);
    }

    /*
    *   UpdateCS - Updates the UI text with the players current amount of cs.
    *   @param cs - string of the amount of cs to update to.
    */
    public void UpdateCS(string cs, GameObject playerUI){
        TMP_Text csText = playerUI.transform.Find("Score/Container/CS/Value").GetComponent<TMP_Text>();
        csText.SetText(cs);
    }

    /*
    *   AddItem - Updates the inventory UI with a new item.
    *   @param itemSlot - int of the UI item slot to add the item to.
    *   @param itemSprite - sprite of the item.
    */
    public void AddItem(int itemSlot, Sprite itemSprite, GameObject playerUI){
        GameObject itemImage = playerUI.transform.Find("Player/Items/ItemsContainer/Item_" + itemSlot + "_Container/Sprite").gameObject;
        itemImage.GetComponent<Image>().sprite = itemSprite;
        itemImage.SetActive(true);
    }

    /*
    *   AddItem - Removes an item from the inventory UI.
    *   @param itemSlot - int of the UI item slot to remove the item from.
    */
    public void RemoveItem(int itemSlot, GameObject playerUI){
        GameObject itemImage = playerUI.transform.Find("Player/Items/ItemsContainer/Item_" + itemSlot + "_Container/Sprite").gameObject;
        itemImage.GetComponent<Image>().sprite = null;
        itemImage.SetActive(false);
    }

    /*
    *   SetPlayerBarActive - Sets the player bar UI to active or not.
    *   @param isActive - bool of whether or not to activate the spell cover.
    */
    public void SetPlayerBarActive(bool isActive, GameObject playerBar){
        playerBar.SetActive(isActive);
    }

    /*
    *   SetChampionUIActive - Sets the champion UI to active or not.
    *   @param isActive - bool of whether or not to activate the spell cover.
    */
    public void SetChampionUIActive(bool isActive, GameObject playerUI){
        playerUI.SetActive(isActive);
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M)){
            if(ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion] == gameObject)
                gameObject.GetComponent<Player>().TakeDamage(10, "magic", gameObject, false);
        }
    }

    /*
    *   AddStatusEffectUI - Adds a status effect indicator to the UI. Left side is buffs, right side is debuffs.
    *   @param statusEffectManager - StatusEffectManager script for the gameObject the UI is being updated for.
    *   @param effect - Effect to add to the UI.
    */
    // TODO: Handle truncation of effects when there are too many to fit the initial container.
    // Could set a max size per row, if number of children % max size per row  > 0 -> 
    // increase size of container, increase y offset.
    public void AddStatusEffectUI(StatusEffects statusEffects, Effect effect, GameObject playerUI){
        // If a stackable effect already has 1 stack, don't create a new UI element.
        if(effect.effectType.isStackable)
            if(statusEffects.GetEffectsByType(effect.effectType.GetType()).Count > 1)
                return;
        //Instantiate status effect prefab.
        GameObject myEffect = (GameObject) Instantiate(statusEffectPrefab, Vector3.zero, Quaternion.identity);
        myEffect.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        myEffect.name = effect.effectType.name;
        myEffect.transform.Find("InnerContainer/Sprite").GetComponent<Image>().sprite = effect.effectType.sprite;
        if(effect.effectDuration == -1f)
            myEffect.transform.Find("InnerContainer/Slider").gameObject.SetActive(false);
        // Set color and position of the UI element.
        if(effect.effectType.isBuff){
            myEffect.transform.Find("Background").GetComponent<Image>().color = Color.blue;
            SetStatusEffectUIPosition(playerUI.transform.Find("Player/StatusEffects/BuffsContainer"), myEffect, true);
        }
        else{
            myEffect.transform.Find("Background").GetComponent<Image>().color = Color.red;
            SetStatusEffectUIPosition(playerUI.transform.Find("Player/StatusEffects/DebuffsContainer"), myEffect, false);
        }
        // Start effect timer animation coroutine.
        if(effect.effectType.isStackable)
            StartCoroutine(StackableStatusEffectUI(statusEffects, effect, myEffect, playerUI.transform.Find("Player/StatusEffects")));
        else
            StartCoroutine(StatusEffectUI(statusEffects, effect, myEffect, playerUI.transform.Find("Player/StatusEffects")));
    }

    /*
    *   SetStatusEffectUIPosition - Sets the position of the given status effect UI element.
    *   @param UI - Transform of the new elements parent to set.
    *   @param myEffect - GameObject of the new UI element.
    *   @param isBuff - bool for if the status effect is a buff or debuff.
    */
    public void SetStatusEffectUIPosition(Transform UI, GameObject myEffect, bool isBuff){
        // Set up variables
        float effectWidth = myEffect.GetComponent<RectTransform>().rect.width;
        Vector2 offset = Vector2.zero;
        // Set parent.
        myEffect.transform.SetParent(UI);
        int index = myEffect.transform.GetSiblingIndex();
        // Calculate the position to put the new UI element in its parent.
        if(index > 0){
            // Offset new element based on last elements position.
            Vector2 prevPos = UI.GetChild(index-1).GetComponent<RectTransform>().anchoredPosition;
            Vector2 sizeOffset = new Vector2(effectWidth + xOffset, 0f);
            // Debuff is opposite from buff.
            if(!isBuff)
                sizeOffset = -(sizeOffset);
            offset = prevPos + sizeOffset;
        }
        else{
            // Calculate first elements position from center of the parent.
            offset = new Vector2(0f - (buffDebuffUIWidth/2f) + (effectWidth/2f), 0f);
            // Debuff is opposite from buff.
            if(!isBuff)
                offset = -(offset);
        }
        // Apply position.
        myEffect.GetComponent<RectTransform>().anchoredPosition = offset;
    }

    /*
    *   StatusEffectUI - Coroutine to animate the UI to show time left on a non-stackable effect.
    *   @param statusEffectManager - StatusEffectManager script for the gameObject the UI is being updated for. 
    *   @param effect - Effect to adjust time left for.
    *   @param effectUI - GameObject of the UI component to be animated.
    */
    public IEnumerator StatusEffectUI(StatusEffects statusEffects, Effect effect, GameObject effectUI, Transform statusEffectsUI){
        float elapsedDuration;
        // Get the timer image component.
        Image slider = effectUI.transform.Find("InnerContainer/Slider").GetComponent<Image>();
        TMP_Text value = null;
        if(effect.effectType is ScriptablePersonalSpell)
            value = effectUI.transform.Find("InnerContainer/Value").GetComponent<TMP_Text>();
        // While the effect still exists on the GameObject.
        while(statusEffects.statusEffects.Contains(effect)){
                if(value != null){
                    value.SetText(((PersonalSpell)effect).stacks.ToString());
                    if(effect.effectDuration == -1f)
                        yield return null;
                }
                // Update status effect timer.
                elapsedDuration = 1f - effect.effectTimer/effect.effectDuration;
                slider.fillAmount = elapsedDuration;
                yield return null;
        }
        // Update UI positions based on what position the ended effect was in.
        UpdateStatusEffectsPositions(effect, effectUI, statusEffectsUI);
        // Remove UI component.
        Destroy(effectUI);
    }

    /*
    *   StackableStatusEffectUI - Coroutine to animate the UI to show time left on a stackable effect.
    *   @param statusEffectManager - StatusEffectManager script for the gameObject the UI is being updated for. 
    *   @param effect - Effect to adjust time left for.
    *   @param effectUI - GameObject of the UI component to be animated.
    */
    public IEnumerator StackableStatusEffectUI(StatusEffects statusEffects, Effect effect, GameObject effectUI, Transform statusEffectsUI){
        // Set stack text active.
        effectUI.transform.Find("InnerContainer/Value").gameObject.SetActive(true);
        // Setup variables.
        Effect displayEffect = effect;
        int stacks = 0;
        float duration = 0f;
        float elapsedDuration;
        // Reduced amount is used when a stack expires.
        // The percentage of fill is calculated based on the duration left on the stack being displayed from the first frame is was displayed.
        // This allows its timer to animate from 100% -> 0% fill instead of starting at a percentage that isn't 100.
        // This is necessary for stacks that falloff over time instead of at the same time.
        float reduceAmount = 0f;
        // Get the timer image component.
        Image slider = effectUI.transform.Find("InnerContainer/Slider").GetComponent<Image>();
        TMP_Text value = effectUI.transform.Find("InnerContainer/Value").GetComponent<TMP_Text>();
        // While the effect still exists on the GameObject.
        while(statusEffects.statusEffects.Contains(effect)){
            // Get how many stacks the effect has.
            int newStacks = statusEffects.GetEffectsByType(effect.effectType.GetType()).Count;
            // If stacks aren't equal then a stack expired or was added.
            if(stacks != newStacks){
                // Set the stacks text and get the next expiring stack to display.
                value.SetText(newStacks.ToString());
                displayEffect = statusEffects.GetNextExpiringStack(effect);
                // If a stack expired.
                if(newStacks < stacks){
                    // Get the duration left on the next expiring stack.
                    duration = displayEffect.effectDuration - displayEffect.effectTimer;
                    // Get the stacks active time.
                    reduceAmount = displayEffect.effectTimer;
                }
                // If a new stack was added use the regular duration.
                else{
                    duration = displayEffect.effectDuration;
                    reduceAmount = 0f;
                }
            }
            // Update status effect timer.
            // 1 - ((effectTimer - effectTimer at frame of first display)/duration left at frame of first display.
            elapsedDuration = 1f - ((displayEffect.effectTimer - reduceAmount)/duration);
            slider.fillAmount = elapsedDuration;
            stacks = newStacks;
            yield return null;
        }
        // Update UI positions based on what position the ended effect was in.
        UpdateStatusEffectsPositions(effect, effectUI, statusEffectsUI);
        // Remove UI component.
        Destroy(effectUI);
    }

    /*
    *   UpdateStatusEffectsPositions - Moves the status effect UI components that were created after the one that ended.
    *   This is to prevent gaps between UI components.
    *   @param effect - Effect that expired.
    *   @param effectUI - GameObject of the status effect UI component to remove.
    */
    public void UpdateStatusEffectsPositions(Effect effect, GameObject effectUI, Transform statusEffectsUI){
        // Get the UI components child index and its current position.
        int index = effectUI.transform.GetSiblingIndex();
        Vector2 newPos = effectUI.GetComponent<RectTransform>().anchoredPosition;
        Vector2 lastPos = Vector2.zero;
        Transform UI;
        // Get the appropriate UI container.
        if(effect.effectType.isBuff)
            UI = statusEffectsUI.Find("BuffsContainer");
        else
            UI = statusEffectsUI.Find("DebuffsContainer");
        // Update every status effect UI components position after the one being removed.
        for(int i = index + 1; i < UI.childCount; i++){
            // Store the position to move the next child to.
            lastPos = UI.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
            // Set the position to the previous components old position.
            UI.GetChild(i).GetComponent<RectTransform>().anchoredPosition = newPos;
            // Set where to move next UI component to.
            newPos = lastPos;
        }
    }

    /*
    *   ShiftStatusEffects - Shifts the status effects UI container a specific amount. Used for overflow and to avoid skill level up UI blocking the status effects UI.
    *   @param shiftAmount - Vector2 of the x and y amounts to shift the UI component.
    */
    public void ShiftStatusEffects(Vector2 shiftAmount, GameObject playerUI){
        playerUI.transform.Find("Player/StatusEffects").GetComponent<RectTransform>().anchoredPosition += shiftAmount;
    }
}
