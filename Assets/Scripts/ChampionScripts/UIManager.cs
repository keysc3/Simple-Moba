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

    [SerializeField] private GameObject championUI;
    [SerializeField] private GameObject playerBar;
    [SerializeField] private GameObject statusEffectPrefab;
    private float buffDebuffUIWidth;
    private float xOffset = 2f;
    private Transform spellsHPManaUI;
    private Transform iconXPUI;
    private Transform scoreUI;
    private Transform itemsUI;
    private Transform statsUI;
    private Transform statusEffectsUI;

    private Slider health;
    private Slider mana;
    private ChampionStats championStats;
    private Gradient gradient;
    private GradientColorKey[] colorKey;
    private GradientAlphaKey[] alphaKey;
    private Color defaultBorderColor;

    // Called when the script instance is being loaded.
    void Awake(){
        championUI = Instantiate(championUI, championUI.transform.position, championUI.transform.rotation);
        championUI.name = gameObject.name + "UI";
        championUI.transform.SetParent(GameObject.Find("/Canvas").transform);
        championUI.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        championUI.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        playerBar = Instantiate(playerBar, playerBar.transform.position, playerBar.transform.rotation);
        playerBar.name = gameObject.name + "PlayerBar";
        Vector3 playerBarPos = playerBar.GetComponent<RectTransform>().anchoredPosition;
        playerBar.transform.SetParent(gameObject.transform);
        playerBar.GetComponent<RectTransform>().anchoredPosition3D = playerBarPos;
        championStats = GetComponent<ChampionStats>();
        defaultBorderColor = new Color(167f/255f, 126f/255f, 69f/255f);
        SetUpGradient();
        spellsHPManaUI = championUI.transform.GetChild(0);
        statsUI = championUI.transform.GetChild(1);
        scoreUI = championUI.transform.GetChild(2);
        itemsUI = championUI.transform.GetChild(3);
        iconXPUI = championUI.transform.GetChild(4);
        statusEffectsUI = championUI.transform.GetChild(5);
        health = spellsHPManaUI.Find("HealthBar").GetComponent<Slider>();
        mana = spellsHPManaUI.Find("ManaBar").GetComponent<Slider>();
        buffDebuffUIWidth = statusEffectsUI.GetChild(0).GetComponent<RectTransform>().rect.width;
    }

    // Start is called before the first frame update.
    void Start(){
        UpdateHealthBar();
        UpdateManaBar();
        SetUpIcons();
        UpdateAllStats();
    }

    void SetUpIcons(){
        Champion champ = (Champion) championStats.unit;
        spellsHPManaUI.GetChild(0).GetChild(1).GetComponent<Image>().sprite = champ.passive_sprite;
        spellsHPManaUI.GetChild(1).GetChild(2).GetComponent<Image>().sprite = champ.spell_1_sprite;
        spellsHPManaUI.GetChild(2).GetChild(2).GetComponent<Image>().sprite = champ.spell_2_sprite;
        spellsHPManaUI.GetChild(3).GetChild(2).GetComponent<Image>().sprite = champ.spell_3_sprite;
        spellsHPManaUI.GetChild(4).GetChild(2).GetComponent<Image>().sprite = champ.spell_4_sprite;
        iconXPUI.GetChild(2).GetComponent<Image>().sprite = champ.icon;
    }
    
    public void UpdateAllStats(){
        UpdateStat("PhysicalDamage", championStats.physicalDamage.GetValue());
        UpdateStat("Armor", championStats.armor.GetValue());
        if(championStats.attackSpeed.GetValue() > 2.5f)
            UpdateStat("AttackSpeed", 2.5f);
        else
            UpdateStat("AttackSpeed", championStats.attackSpeed.GetValue());
        UpdateStat("Crit", 0f);
        UpdateStat("MagicDamage", championStats.magicDamage.GetValue());
        UpdateStat("MagicResist", championStats.magicResist.GetValue());
        UpdateStat("Haste", championStats.haste.GetValue());
        UpdateStat("Speed", championStats.speed.GetValue());
    }

    public void UpdateStat(string statName, float value){
        if(statName != "AttackSpeed")
            statsUI.GetChild(0).Find(statName).GetChild(1).GetComponent<TMP_Text>().SetText(Mathf.Round(value).ToString());
        else
            statsUI.GetChild(0).Find(statName).GetChild(1).GetComponent<TMP_Text>().SetText((Mathf.Round(value * 100f) * 0.01f).ToString());
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
    public void UpdateCooldown(string spell, float cooldownLeft, float totalCooldown){
        Transform spellCD = spellsHPManaUI.Find(spell).GetChild(3);
        // Set the cooldown panel children to be active.
        for(int i = 0; i < 3; i++){
            spellCD.GetChild(i).gameObject.SetActive(true);
        }

        TMP_Text text = spellCD.GetChild(2).GetComponent<TMP_Text>();
        // If off cooldown.
        if(cooldownLeft == 0f)
            OffCooldown(spellCD);
        else{
            // Update the UI cooldown text and slider.
            text.SetText(Mathf.Ceil(cooldownLeft).ToString());
            float fill = Mathf.Clamp(cooldownLeft/totalCooldown, 0f, 1f);
            spellCD.GetChild(1).GetComponent<Image>().fillAmount = fill;
        }
    }

    /*
    *   OffCooldown - Sets the spells cooldown UI to inactive.
    *   @param OffCooldown - transform of the spell cover to turn off.
    */
    public void OffCooldown(Transform spellCD){
        for(int i = 0; i < 3; i++){
            spellCD.GetChild(i).gameObject.SetActive(false);
        }
    }

    /*
    *   SpellLearned - Sets the spell cover for the given spell to inactive.
    *   @param spell - string of the spell that was learned.
    */
    public void SpellLearned(string spell){
        Transform spellCD = spellsHPManaUI.Find(spell).GetChild(3);
        spellCD.GetChild(0).gameObject.SetActive(false);
        SpellLeveled(spell, 1);
    }
    
    /*
    *   SpellLeveled - Updates the UI to show the given spells new level.
    *   @param spell - string of the spell that was leveled.
    *   @param spellLevel - int of the new level of the spell.
    */
    public void SpellLeveled(string spell, int spellLevel){
        Transform spellUI = spellsHPManaUI.Find(spell);
        spellUI.GetChild(4).GetChild(spellLevel-1).GetChild(0).gameObject.SetActive(false);
    }

    /*
    *   UpdateHealthBar - Updates the health bar UI.
    */
    public void UpdateHealthBar(){
        // If the champion is dead.
        if(!championStats.isDead){
            // Get the health percent the player is at and set the health bar text to currenthp/maxhp.
            float healthPercent = Mathf.Round((championStats.currentHealth/championStats.maxHealth.GetValue()) * 100);
            health.transform.GetChild(2).GetComponent<TMP_Text>()
            .SetText(Mathf.Ceil(championStats.currentHealth) + "/" + Mathf.Ceil(championStats.maxHealth.GetValue()));
            // Set the fill based on players health percent.
            playerBar.transform.GetChild(0).GetChild(1).GetComponent<Slider>().value = healthPercent;
            health.value = healthPercent;
        }
        else{
            // Set players health text and fill to 0.
            playerBar.SetActive(false);
            health.transform.GetChild(2).GetComponent<TMP_Text>()
            .SetText(0 + "/" + Mathf.Ceil(championStats.maxHealth.GetValue()));
            health.value = 0;
        }
    }

    /*
    *   UpdateHealthRegen - Updates the health regen text UI.
    *   @param healthRegen - float of the value to update the text to.
    */
    public void UpdateHealthRegen(float healthRegen){
        health.transform.GetChild(3).GetComponent<TMP_Text>().SetText("+" + Mathf.Round(healthRegen * 100.0f) * 0.01f);
    }

    /*
    *   SetHealthRegenActive - Updates the health regen UI to be showing or not.
    *   @param isActive - bool of whether to active the UI or not.
    */
    public void SetHealthRegenActive(bool isActive){
        health.transform.GetChild(3).gameObject.SetActive(isActive);
    }

    /*
    *   UpdateManaBar - Updates the mana bar UI.
    */
    public void UpdateManaBar(){
        // Get the percent of mana the player has left and set the mana bar text to currentmana/maxmana
        float manaPercent = Mathf.Round((championStats.currentMana/championStats.maxMana.GetValue()) * 100);
        mana.transform.GetChild(2).GetComponent<TMP_Text>()
        .SetText(Mathf.Ceil(championStats.currentMana) + "/" + Mathf.Ceil(championStats.maxMana.GetValue()));
        // Set the fill based on the player mana percent.
        playerBar.transform.GetChild(0).GetChild(2).GetComponent<Slider>().value = manaPercent;
        mana.value = manaPercent;
    }

    /*
    *   UpdateManaRegen - Updates the mana regen text UI.
    *   @param manaRegen - float of the value to update the text to.
    */
    public void UpdateManaRegen(float manaRegen){
        mana.transform.GetChild(3).GetComponent<TMP_Text>().SetText("+" + Mathf.Round(manaRegen * 100.0f) * 0.01f);
    }

    /*
    *   SetManaRegenActive - Updates the mana regen UI to be showing or not.
    *   @param isActive - bool of whether to active the UI or not.
    */
    public void SetManaRegenActive(bool isActive){
        mana.transform.GetChild(3).gameObject.SetActive(isActive);
    }

    /*
    *   SetSkillLevelUpActive - Sets the skill level up available to active or not for each spell.
    *   @param spellLists - Dictionary of <string, int> pairs representing spell:level.
    *   @param level - int of the players level.
    *   @param isActive - bool of whether to set the UI elements active or not.
    */
    public void SetSkillLevelUpActive(Dictionary<string, int> spellLevels, int level, bool isActive){
        // For each spell.
        foreach(KeyValuePair<string, int> spell in spellLevels){
            // If the UI should be active.
            if(isActive){
                // If the kvp is spell 4.
                if(spell.Key == "Spell_4"){
                    int spell_4_level = spellLevels["Spell_4"];
                    // If spell 4 can be leveled.
                    if((spell_4_level < 1 && level > 5) || (spell_4_level < 2 && level > 10) || (spell_4_level < 3 && level > 15)){
                        spellsHPManaUI.GetChild(4).GetChild(0).gameObject.SetActive(true);
                    }
                    else
                        spellsHPManaUI.Find(spell.Key).transform.GetChild(0).gameObject.SetActive(false);
                }
                // If a basic spell then activate its level up available UI if it isn't max spell level.
                else{
                    if(spell.Value < 5){
                        spellsHPManaUI.Find(spell.Key).transform.GetChild(0).gameObject.SetActive(true);  
                    }
                    else{
                        spellsHPManaUI.Find(spell.Key).transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
            }
            else{
                spellsHPManaUI.Find(spell.Key).transform.GetChild(0).gameObject.SetActive(false);  
            }
        }
    }

    /*
    * SkillLevelUpGradient - Increments the skill level up available gradient time to animate it pulsing.
    */
    public void SkillLevelUpGradient(){
        float value = Time.time;
        // For each spell.
        for(int i = 0; i < 4; i++)
            spellsHPManaUI.GetChild(i+1).GetChild(0).GetChild(0).GetComponent<Image>().color = gradient.Evaluate(Mathf.PingPong(value, 1));
    }

    /*
    *   SetSpellInUse - Sets a spells border to white or default color to show if it is active or not.
    *   @param spell - string of the spell UI to update.
    *   @param inUse - bool of whether or not the spell is in use.
    */
    public void SetSpellInUse(string spell, bool inUse){
        GameObject spellOutline = spellsHPManaUI.Find(spell).transform.GetChild(1).gameObject;
        if(inUse)
            spellOutline.GetComponent<Image>().color = Color.white;
        else
            spellOutline.GetComponent<Image>().color = defaultBorderColor;
    }

    /*
    *   SetSpellActiveDuration - Animates the border of a spell using a slider to represent the spells active duration left.
    *   @param spell - int of the spell number that is active.
    *   @param duration - float of the total duration of the spell.
    *   @param active - float of the amount of time the spell has been active so far.
    */
    public void SetSpellActiveDuration(int spell, float duration, float active){
        // Get value between 0 and 1 representing the percent of the spell duration left.
        float fill = 1.0f - (active/duration);
        fill = Mathf.Clamp(fill, 0f, 1f);
        // Set the fill on the active spells slider.
        GameObject spellDurationSlider = spellsHPManaUI.GetChild(spell).GetChild(1).GetChild(1).gameObject;
        spellDurationSlider.SetActive(true);
        spellDurationSlider.transform.GetChild(1).GetComponent<Image>().fillAmount = fill;
    }
    
    /*
    *   SetSpellDurationOver - Set the spells active slider to inactive.
    *   @param spell - int of the spell to deactivate the UI for.
    */
    public void SetSpellDurationOver(int spell){
        spellsHPManaUI.GetChild(spell).GetChild(1).GetChild(1).gameObject.SetActive(false);
    }

    /*
    *   SetSpellCoverActive - Sets the spells UI cover to active or not.
    *   @param spell - int of the spell to update the UI for.
    *   @param isActive - bool of whether or not to activate the spell cover.
    */
    public void SetSpellCoverActive(int spell, bool isActive){
        spellsHPManaUI.GetChild(spell).GetChild(3).GetChild(0).gameObject.SetActive(isActive);
    }

    /*
    *   UpdateExperienceBar - Updates the experience slider to the percent of experience earned towards the next level.
    *   @param currentXP - float of the players current experience.
    *   @param requiredXP float of the total required experience for the players next level.
    */
    public void UpdateExperienceBar(float currentXP, float requiredXP){
        iconXPUI.GetChild(3).GetComponent<Slider>().value = Mathf.Round((currentXP/requiredXP) * 100);
    }

    /*
    *   UpdateLevelText - Updates the player level UI text.
    *   @param currentLevel - int of the players current level.
    */
    public void UpdateLevelText(int currentLevel){
        iconXPUI.GetChild(5).GetChild(1).GetComponent<TMP_Text>().SetText(currentLevel.ToString());
        playerBar.transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<TMP_Text>().SetText(currentLevel.ToString());
    }

    /*
    *   UpdateDeathTimer - Updates the death timer UI of the player.
    *   @param timeLeft - float of the time left before respawn.
    */
    public void UpdateDeathTimer(float timeLeft){
        GameObject iconCover = iconXPUI.GetChild(4).gameObject;
        // Active icon cover if not already activated.
        if(!iconCover.activeSelf)
            iconCover.SetActive(true);
        if(timeLeft != 0f){
            iconCover.transform.GetChild(0).GetComponent<TMP_Text>().SetText(Mathf.Ceil(timeLeft).ToString());
        }
        else{
            iconCover.SetActive(false); 
        }
    }

    /*
    *   UpdateKills - Updates the UI text with the players current amount of kills.
    *   @param kills - string of the amount of kills to update to.
    */
    public void UpdateKills(string kills){
        TMP_Text killsText = scoreUI.GetChild(0).GetComponent<TMP_Text>();
        killsText.SetText(kills);
    }

    /*
    *   UpdateDeaths - Updates the UI text with the players current amount of deaths.
    *   @param deaths - string of the amount of deaths to update to.
    */
    public void UpdateDeaths(string deaths){
        TMP_Text deathsText = scoreUI.GetChild(1).GetComponent<TMP_Text>();
        deathsText.SetText(deaths);
    }

    /*
    *   UpdateAssists - Updates the UI text with the players current amount of assists.
    *   @param assists - string of the amount of assists to update to.
    */
    public void UpdateAssists(string assists){
        TMP_Text assistsText = scoreUI.GetChild(2).GetComponent<TMP_Text>();
        assistsText.SetText(assists);
    }

    /*
    *   UpdateCS - Updates the UI text with the players current amount of cs.
    *   @param cs - string of the amount of cs to update to.
    */
    public void UpdateCS(string cs){
        TMP_Text csText = scoreUI.GetChild(3).GetComponent<TMP_Text>();
        csText.SetText(cs);
    }

    /*
    *   AddItem - Updates the inventory UI with a new item.
    *   @param itemSlot - int of the UI item slot to add the item to.
    *   @param itemSprite - sprite of the item.
    */
    public void AddItem(int itemSlot, Sprite itemSprite){
        GameObject itemImage = itemsUI.GetChild(itemSlot-1).GetChild(0).gameObject;
        itemImage.GetComponent<Image>().sprite = itemSprite;
        itemImage.SetActive(true);
    }

    /*
    *   AddItem - Removes an item from the inventory UI.
    *   @param itemSlot - int of the UI item slot to remove the item from.
    */
    public void RemoveItem(int itemSlot){
        GameObject itemImage = itemsUI.GetChild(itemSlot-1).GetChild(0).gameObject;
        itemImage.SetActive(false);
        itemImage.GetComponent<Image>().sprite = null;
    }

    /*
    *   SetPlayerBarActive - Sets the player bar UI to active or not.
    *   @param isActive - bool of whether or not to activate the spell cover.
    */
    public void SetPlayerBarActive(bool isActive){
        playerBar.SetActive(isActive);
    }

    /*
    *   SetChampionUIActive - Sets the champion UI to active or not.
    *   @param isActive - bool of whether or not to activate the spell cover.
    */
    public void SetChampionUIActive(bool isActive){
        championUI.SetActive(isActive);
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M)){
            if(ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion] == gameObject)
                gameObject.GetComponent<ChampionStats>().TakeDamage(10, "magic", gameObject, false);
        }
        // Activate mana and health regen UI if the player is not full mana or full health.
        if(championStats.currentMana < championStats.maxMana.GetValue() && !championStats.isDead){
            SetManaRegenActive(true);
        }
        else{
            SetManaRegenActive(false);
        }
        if(championStats.currentHealth < championStats.maxHealth.GetValue() && !championStats.isDead){
            SetHealthRegenActive(true);
        }
        else{
            SetHealthRegenActive(false);
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
    public void AddStatusEffectUI(StatusEffectManager statusEffectManager, Effect effect){
        // If a stackable effect already has 1 stack, don't create a new UI element.
        if(effect.effectType.isStackable)
            if(statusEffectManager.GetEffectsByType(effect.effectType.GetType()).Count > 1)
                return;
        //Instantiate status effect prefab.
        GameObject myEffect = (GameObject)Instantiate(statusEffectPrefab, Vector3.zero, Quaternion.identity);
        myEffect.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        myEffect.name = effect.effectType.name;
        myEffect.transform.GetChild(1).GetComponent<Image>().sprite = effect.effectType.sprite;
        // Set color and position of the UI element.
        if(effect.effectType.isBuff){
            myEffect.transform.GetChild(0).GetComponent<Image>().color = Color.blue;
            SetStatusEffectUIPosition(statusEffectsUI.GetChild(0), myEffect, true);
        }
        else{
            myEffect.transform.GetChild(0).GetComponent<Image>().color = Color.red;
            SetStatusEffectUIPosition(statusEffectsUI.GetChild(1), myEffect, false);
        }
        // Start effect timer animation coroutine.
        if(effect.effectType.isStackable)
            StartCoroutine(StackableStatusEffectUI(statusEffectManager, effect, myEffect));
        else
            StartCoroutine(StatusEffectUI(statusEffectManager, effect, myEffect));
    }

    /*
    *   SetStatusEffectUIPosition - Sets the position of the given status effect UI element.
    *   @param UI - Transform of the new elements parent to set.
    *   @param myEffect - GameObject of the new UI element.
    *   @param isBuff - bool for if the status effect is a buff or debuff.
    */
    public void SetStatusEffectUIPosition(Transform UI, GameObject myEffect, bool isBuff){
        // Set up variables
        float effectWidth = myEffect.transform.GetChild(0).GetComponent<RectTransform>().rect.width;
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
    public IEnumerator StatusEffectUI(StatusEffectManager statusEffectManager, Effect effect, GameObject effectUI){
        float elapsedDuration;
        // Get the timer image component.
        Image timer = effectUI.transform.GetChild(2).GetComponent<Image>();
        // While the effect still exists on the GameObject.
        while(statusEffectManager.statusEffects.Contains(effect)){
                // Update status effect timer.
                elapsedDuration = 1f - effect.effectTimer/effect.effectDuration;
                timer.fillAmount = elapsedDuration;
                yield return null;
        }
        // Update UI positions based on what position the ended effect was in.
        UpdateStatusEffectsPositions(effect, effectUI);
        // Remove UI component.
        Destroy(effectUI);
    }

    /*
    *   StackableStatusEffectUI - Coroutine to animate the UI to show time left on a stackable effect.
    *   @param statusEffectManager - StatusEffectManager script for the gameObject the UI is being updated for. 
    *   @param effect - Effect to adjust time left for.
    *   @param effectUI - GameObject of the UI component to be animated.
    */
    public IEnumerator StackableStatusEffectUI(StatusEffectManager statusEffectManager, Effect effect, GameObject effectUI){
        // Set stack text active.
        effectUI.transform.GetChild(3).gameObject.SetActive(true);
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
        Image timer = effectUI.transform.GetChild(2).GetComponent<Image>();
        // While the effect still exists on the GameObject.
        while(statusEffectManager.statusEffects.Contains(effect)){
            // Get how many stacks the effect has.
            int newStacks = statusEffectManager.GetEffectsByType(effect.effectType.GetType()).Count;
            // If stacks aren't equal then a stack expired or was added.
            if(stacks != newStacks){
                // Set the stacks text and get the next expiring stack to display.
                effectUI.transform.GetChild(3).gameObject.GetComponent<TMP_Text>().text = newStacks.ToString();
                displayEffect = GetNextExpiringStack(statusEffectManager, effect);
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
            timer.fillAmount = elapsedDuration;
            stacks = newStacks;
            yield return null;
        }
        // Update UI positions based on what position the ended effect was in.
        UpdateStatusEffectsPositions(effect, effectUI);
        // Remove UI component.
        Destroy(effectUI);
    }

    /*
    *   GetNextExpiringStack - Gets the next expiring stack of a stackable effect.
    *   @param statusEffectManager - StatusEffectManager script of the GameObject the UI is for.
    *   @param effect - Effect to get the next expiring stack for.
    */
    public Effect GetNextExpiringStack(StatusEffectManager statusEffectManager, Effect effect){
        List<Effect> myEffects = statusEffectManager.GetEffectsByName(effect.effectType.name);
        Effect nextExipiring = myEffects[0];
        float timeTillExpired = myEffects[0].effectDuration - myEffects[0].effectTimer;
        if(myEffects.Count > 1){
            for(int i = 1; i < myEffects.Count; i++){
                float check = myEffects[i].effectDuration - myEffects[i].effectTimer;
                if(check < timeTillExpired){
                    timeTillExpired = check;
                    nextExipiring = myEffects[i];
                }
            }
        }
        return nextExipiring;
    }

    /*
    *   UpdateStatusEffectsPositions - Moves the status effect UI components that were created after the one that ended.
    *   This is to prevent gaps between UI components.
    *   @param effect - Effect of the status effect that ended.
    *   @param effectUI - GameObject of the status effect UI component to remove.
    */
    public void UpdateStatusEffectsPositions(Effect effect, GameObject effectUI){
        // Get the UI components child index and its current position.
        int index = effectUI.transform.GetSiblingIndex();
        Vector2 newPos = effectUI.GetComponent<RectTransform>().anchoredPosition;
        Vector2 lastPos = Vector2.zero;
        Transform UI;
        // Get the appropriate UI container.
        if(effect.effectType.isBuff)
            UI = statusEffectsUI.GetChild(0);
        else
            UI = statusEffectsUI.GetChild(1);
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
    public void ShiftStatusEffects(Vector2 shiftAmount){
        statusEffectsUI.GetComponent<RectTransform>().anchoredPosition += shiftAmount;
    }
}
