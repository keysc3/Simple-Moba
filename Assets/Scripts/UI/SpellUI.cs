using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SpellUI : MonoBehaviour
{
    //TODO: Store component refs that are needed instead of GameObject?
    private Dictionary<SpellType, Dictionary<string, GameObject>> spellObjects = new Dictionary<SpellType, Dictionary<string, GameObject>>();
    
    // Start is called before the first frame update
    void Start()
    {
        GetSpellUIObjects();   
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
                GameObject spellCDTransform = transform.Find(spell + "_Container/SpellContainer/Spell/CD").gameObject;
                GameObject durationSlider = null;
                GameObject durationImage = null;
                if(!(new List<SpellType>(){SpellType.Passive, SpellType.SummonerSpell1, SpellType.SummonerSpell2}).Contains(spellType)){
                    durationSlider = transform.Find(spell + "_Container/SpellContainer/Outline/Slider").gameObject;
                    durationImage = durationSlider.transform.Find("Fill").gameObject;
                }
                spellObjects.Add(spellType, new Dictionary<string, GameObject>());
                spellObjects[spellType].Add("CDTransform", spellCDTransform);
                spellObjects[spellType].Add("CDCover", spellCDTransform.transform.Find("Cover").gameObject);
                spellObjects[spellType].Add("CDImage", spellCDTransform.transform.Find("Slider").gameObject);
                spellObjects[spellType].Add("CDText", spellCDTransform.transform.Find("Value").gameObject);
                spellObjects[spellType].Add("DurationSlider", durationSlider);
                spellObjects[spellType].Add("DurationImage", durationImage);
            }
        }
    }

    /*
    *   SetupCallbacks - Setup necessary callbacks.
    */
    private void SetupCallbacks(){
       // Setup callbacks.
        Spell[] objSpells = GetComponentsInParent<Spell>();
        foreach(Spell spell in objSpells){
            spell.spellController.SpellCDUpdateCallback += SpellCDTimerUpdate;
            spell.SpellCDSetActiveCallback += SpellCDChildrenSetActive;
            spell.SpellSliderUpdateCallback += UpdateActiveSpellSlider;
            spell.SetComponentActiveCallback += SetComponentActive;
        } 
    }

    /*
    *   UpdateActiveSpellSlider - Updates a spells UI component representing an active duration.
    *   @param spellType - SpellType of which UI component to update.
    *   @param duration - float of the total duration.
    *   @param active - float of the active duration.
    */
    public void UpdateActiveSpellSlider(SpellType spellType, float duration, float active){
        Image slider = spellObjects[spellType]["DurationImage"].GetComponent<Image>();
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
        Image slider = spellObjects[spellType]["CDImage"].GetComponent<Image>();
        GameObject textObject = spellObjects[spellType]["CDText"];
        TMP_Text text = textObject.GetComponent<TMP_Text>();
        if(!textObject.activeSelf && cooldownLeft > 0)
            SpellCDChildrenSetActive(spellType, true);
        else if(textObject.activeSelf && cooldownLeft <= 0)
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
        foreach(Transform child in spellObjects[spellType]["CDTransform"].transform){
                child.gameObject.SetActive(isActive);
        }
    }

    /*
    *   SetComponentActive - Sets a specific GameObject active or inactive.
    *   @param spellType - SpellType of the spell UI to adjust.
    *   @param component - string of the UI component being changed.
    *   @param isActive - bool for setting GameObject active or not.
    */
    public void SetComponentActive(SpellType spellType, string component, bool isActive){
        spellObjects[spellType][component].SetActive(isActive);
    }
}
