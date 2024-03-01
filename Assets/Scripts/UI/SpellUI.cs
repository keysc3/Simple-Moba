using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SpellUI : MonoBehaviour
{
    private Dictionary<SpellType, Hashtable> spellComps = new Dictionary<SpellType, Hashtable>();
    private Dictionary<SpellType, SpellData> spellDataDict = new Dictionary<SpellType, SpellData>();
    private bool prevEnabled = false;
    
    // Called when the object becomes enabled and active.
    void OnEnable(){
        if(!prevEnabled){
            PlayerSpells playerSpells = GetComponentInParent<PlayerSpells>();
            if(playerSpells != null)
                playerSpells.SpellAddedCallback += SetupSpellButtons;
            GetSpellUIObjects();
            prevEnabled = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupCallbacks();
    }

    /*
    *   GetSpellUIObjects - Store the UI GameObjects for each spell.
    */
    private void GetSpellUIObjects(){
        // Store the UI components needed.
        foreach(SpellType spellType in Enum.GetValues(typeof(SpellType))){
            if(spellType != SpellType.None){
                string spell = spellType.ToString();
                Transform spellCDTransform = transform.Find(spell + "_Container/SpellContainer/Spell/CD");
                Transform durationSlider = null;
                Image durationImage = null;
                SpellLevelUpButton spellLevelUpButton = null;
                GameObject noManaTransform = null;
                if(!(new List<SpellType>(){SpellType.Passive, SpellType.SummonerSpell1, SpellType.SummonerSpell2}).Contains(spellType)){
                    durationSlider = transform.Find(spell + "_Container/SpellContainer/Outline/Slider");
                    durationImage = durationSlider.transform.Find("Fill").GetComponent<Image>();
                    spellLevelUpButton = transform.Find(spell + "_Container/LevelUp/Button").GetComponent<SpellLevelUpButton>();
                    noManaTransform = transform.Find(spell + "_Container/SpellContainer/Spell/NoMana").gameObject;
                }
                Hashtable hashy = new Hashtable();
                hashy.Add(SpellComponent.CDTransform, spellCDTransform);
                hashy.Add(SpellComponent.CDCover, spellCDTransform.transform.Find("Cover"));
                hashy.Add(SpellComponent.CDImage, spellCDTransform.transform.Find("Slider").GetComponent<Image>());
                hashy.Add(SpellComponent.CDText, spellCDTransform.transform.Find("Value").GetComponent<TMP_Text>());
                hashy.Add(SpellComponent.DurationSlider, durationSlider);
                hashy.Add(SpellComponent.DurationImage, durationImage);
                hashy.Add(SpellComponent.SpellButton, spellCDTransform.parent.Find("Button").GetComponent<SpellButton>());
                hashy.Add(SpellComponent.SpellImage, spellCDTransform.parent.Find("Icon").GetComponent<Image>());
                TMP_Text text = null;
                Transform text2 = spellCDTransform.parent.Find("ManaCost");
                if(text2 != null)
                    text = text2.GetComponent<TMP_Text>();
                hashy.Add(SpellComponent.ManaCost, text);
                hashy.Add(SpellComponent.SpellLevelUpButton, spellLevelUpButton);
                hashy.Add(SpellComponent.NoMana, noManaTransform);
                spellComps.Add(spellType, hashy);
            }
        }
    }

    /*
    *   SetupCallbacks - Setup necessary callbacks.
    */
    private void SetupCallbacks(){
        // Setup callbacks.
        LevelManager levelManager = GetComponentInParent<IPlayer>().levelManager;
        levelManager.SpellLevelUpCallback += SetManaCost;
        Spell[] objSpells = GetComponentsInParent<Spell>();
        foreach(Spell spell in objSpells){
            if(!(new List<SpellType>(){SpellType.Passive, SpellType.SummonerSpell1, SpellType.SummonerSpell2}).Contains(spell.SpellNum)){
                if(spell.SpellNum == SpellType.None)
                    spellDataDict.Add(spell.spellData.defaultSpellNum, spell.spellData);
                else
                    spellDataDict.Add(spell.SpellNum, spell.spellData);
            }
            SpellCallbacks(spell);
        }
    }

    public void SpellCallbacks(Spell spell){
        spell.spellController.SpellCDUpdateCallback += SpellCDTimerUpdate;
        spell.SpellCDSetActiveCallback += SpellCDChildrenSetActive;
        spell.SpellSliderUpdateCallback += UpdateActiveSpellSlider;
        spell.SetComponentActiveCallback += SetComponentActive;
        spell.SetSpriteCallback += SetSprite;
    }

    public void LateUpdate(){
        ChampionStats championStats = (ChampionStats) GetComponentInParent<IPlayer>().unitStats; 
        Spell[] objSpells = GetComponentsInParent<Spell>();
        foreach(Spell spell in objSpells){
            if(!(new List<SpellType>(){SpellType.Passive, SpellType.SummonerSpell1, SpellType.SummonerSpell2}).Contains(spell.SpellNum)){
                if(spell.SpellLevel > -1){
                    GameObject gObject = (GameObject) spellComps[spell.SpellNum][SpellComponent.NoMana];
                    Transform tForm = (Transform) spellComps[spell.SpellNum][SpellComponent.CDCover];
                    bool tFormActive = tForm.gameObject.activeSelf;
                    bool isActive = gObject.activeSelf;
                    if(!tFormActive && !spell.OnCd && championStats.CurrentMana < spell.spellData.baseMana[spell.SpellLevel]){
                        if(!isActive)
                            gObject.SetActive(true);
                    }
                    else{
                        if(isActive)
                            gObject.SetActive(false);
                    }
                }
            }
        } 
    }

    /*
    *   UpdateActiveSpellSlider - Updates a spells UI component representing an active duration.
    *   @param spellType - SpellType of which UI component to update.
    *   @param duration - float of the total duration.
    *   @param active - float of the active duration.
    */
    public void UpdateActiveSpellSlider(SpellType spellType, float duration, float active){
        Image slider = (Image) spellComps[spellType][SpellComponent.DurationImage];
        // Get value between 0 and 1 representing the percent of the spell duration left.
        float fill = 1.0f - (active/duration);
        fill = Mathf.Clamp(fill, 0f, 1f);
        // Set the fill on the active spells slider.
        slider.fillAmount = fill;
    }

    /*
    *   SpellCDTimerUpdate - Updates the UI for a spells cooldown.
    *   @param spellType - SpellType being updated.
    *   @param cooldownLeft - the remaining cooldown on the spell.
    *   @param spell_cd - float representing the spells cooldown.
    */
    private void SpellCDTimerUpdate(SpellType spellType, float cooldownLeft, float spell_cd){
        Image slider = (Image) spellComps[spellType][SpellComponent.CDImage];
        TMP_Text text = (TMP_Text) spellComps[spellType][SpellComponent.CDText];
        if(!text.gameObject.activeSelf && cooldownLeft > 0)
            SpellCDChildrenSetActive(spellType, true);
        else if(text.gameObject.activeSelf && cooldownLeft <= 0)
            SpellCDChildrenSetActive(spellType, false);
        text.SetText(Mathf.Ceil(cooldownLeft).ToString());
        float fill = Mathf.Clamp(cooldownLeft/spell_cd, 0f, 1f);
        slider.fillAmount = fill;
    }

    /*
    *   SpellCDChildrenSetActive - Sets the children of a transform as active or inactive based on given bool.
    *   @param spellType - SpellType of which UI element to adjust
    *   @param isActive - bool of wether to set children active or inactive.
    */
    public void SpellCDChildrenSetActive(SpellType spellType, bool isActive){
        foreach(Transform child in ((Transform) spellComps[spellType][SpellComponent.CDTransform])){
                child.gameObject.SetActive(isActive);
        }
    }

    /*
    *   SetComponentActive - Sets a specific GameObject active or inactive.
    *   @param spellType - SpellType of the spell UI to adjust.
    *   @param component - SpellComponent of the UI component being changed.
    *   @param isActive - bool for setting GameObject active or not.
    */
    public void SetComponentActive(SpellType spellType, SpellComponent component, bool isActive){
        ((Transform) spellComps[spellType][component]).gameObject.SetActive(isActive);
    }

    /*
    *   SetupSpellButtons - Setup for the a spells button click and level up button click.
    *   @param newSpell - ISpell to set the buttons for.
    *   @param levelManager - LevelManager of the player the spell is for.
    */
    private void SetupSpellButtons(ISpell newSpell, LevelManager levelManager){
        SpellType num;
        //Spell button
        if(newSpell.SpellNum == SpellType.None)
            num = newSpell.spellData.defaultSpellNum;
        else
            num = newSpell.SpellNum;
        SpellButton spellButton = (SpellButton) spellComps[num][SpellComponent.SpellButton];
        spellButton.spell = newSpell;
        //TODO: Change this to not be hardcoded using a proper keybind/input system?
        List<KeyCode> inputs = new List<KeyCode>(){KeyCode.None, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.D, KeyCode.F};
        if(num != SpellType.Passive)
            spellButton.keyCode = inputs[((int) num) - 1];
        else
            spellButton.keyCode = inputs[0];
        ISpellInput spellInput = GetComponentInParent<ISpellInput>();
        spellButton.SpellInput = spellInput;
        // Spell level up button.
        ((Image) spellComps[num][SpellComponent.SpellImage]).sprite = newSpell.spellData.sprite;
        SpellLevelUpButton spellLevelUpButton = (SpellLevelUpButton) spellComps[num][SpellComponent.SpellLevelUpButton];
        if(spellLevelUpButton != null){
            spellLevelUpButton.spell = num;
            spellLevelUpButton.LevelManager = levelManager;
            spellLevelUpButton.SpellInput = spellInput;
        }
    }

    /*
    *   SetSprite - Sets the spells sprite.
    *   @param spellType - SpellType for which UI element to adjust.
    *   @param component - SpellComponent of the component to set active.
    *   @param sprite - Sprite to set the image to.
    */
    public void SetSprite(SpellType spellType, SpellComponent component, Sprite sprite){
        ((Image) spellComps[spellType][component]).sprite = sprite;
    }

    /*
    *   SetManaCost - Sets the UI to a mana cost given a spells level.
    *   @param spellType - SpellType enum of what spell is being changed.
    *   @param spellLevel - int of the spell level.
    */
    public void SetManaCost(SpellType spellType, int spellLevel){
        TMP_Text text = (TMP_Text) spellComps[spellType][SpellComponent.ManaCost];
        float newCost = spellDataDict[spellType].baseMana[spellLevel - 1];
        if(text != null){
            text.text = newCost.ToString();
        }
    }
}