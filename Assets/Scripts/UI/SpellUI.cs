using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SpellUI : MonoBehaviour
{
    private Dictionary<SpellType, Tuple<Transform, Image, Image, TMP_Text, Image>> spellComponents = new Dictionary<SpellType, Tuple<Transform, Image, Image, TMP_Text, Image>>();

    // Start is called before the first frame update
    void Start()
    {
        // Store the UI components needed.
        foreach(SpellType spellType in Enum.GetValues(typeof(SpellType))){
            if(spellType != SpellType.None){
                string spell = spellType.ToString();
                Transform spellCDTransform = transform.Find(spell + "_Container/SpellContainer/Spell/CD");
                Image spellCover = spellCDTransform.Find("Cover").GetComponent<Image>();
                Image spellCDImage = spellCDTransform.Find("Slider").GetComponent<Image>();
                TMP_Text spellCDText = spellCDTransform.Find("Value").GetComponent<TMP_Text>();
                //GameObject spellDurationSlider = transform.Find(spell + "_Container/SpellContainer/Outline/Slider").gameObject;
                Image durationImage = null;
                if(!(new List<SpellType>(){SpellType.Passive, SpellType.SummonerSpell1, SpellType.SummonerSpell2}).Contains(spellType))
                    durationImage = transform.Find(spell + "_Container/SpellContainer/Outline/Slider/Fill").GetComponent<Image>();
                spellComponents.Add(spellType, new Tuple<Transform, Image, Image, TMP_Text, Image>(spellCDTransform, spellCover, spellCDImage, spellCDText, durationImage));
            }
        }
        
        // Setup callbacks.
        Spell[] objSpells = GetComponentsInParent<Spell>();
        foreach(Spell spell in objSpells){
            spell.spellController.SpellCDUpdateCallback += SpellCDTimerUpdate;
            spell.SpellCDSetActiveCallback += SpellCDChildrenSetActive;
            spell.SpellSliderUpdateCallback += UpdateActiveSpellSlider;
        }
    }

    /*
    *   UpdateActiveSpellSlider - Updates a spells UI component representing an active duration.
    *   @param spellType - SpellType of which UI component to update.
    *   @param duration - float of the total duration.
    *   @param active - float of the active duration.
    */
    public void UpdateActiveSpellSlider(SpellType spellType, float duration, float active){
        Tuple<Transform, Image, Image, TMP_Text, Image> spellComps = spellComponents[spellType];
        // Get value between 0 and 1 representing the percent of the spell duration left.
        float fill = 1.0f - (active/duration);
        fill = Mathf.Clamp(fill, 0f, 1f);
        // Set the fill on the active spells slider.
        spellComps.Item5.fillAmount = fill;
    }

    /*
    *   SpellCDTimerUpdate - Updates the UI for a spells cooldown.
    *   @param spellType - SpellType being updated.
    *   @param cooldownLeft - the remaining cooldown on the spell.
    *   @param spell_cd - float representing the spells cooldown.
    */
    private void SpellCDTimerUpdate(SpellType spellType, float cooldownLeft, float spell_cd){
        Tuple<Transform, Image, Image, TMP_Text, Image> spellComps = spellComponents[spellType];
        if(!spellComps.Item4.gameObject.activeSelf && cooldownLeft > 0)
            SpellCDChildrenSetActive(spellType, true);
        else if(spellComps.Item4.gameObject.activeSelf && cooldownLeft <= 0)
            SpellCDChildrenSetActive(spellType, false);
        spellComps.Item4.SetText(Mathf.Ceil(cooldownLeft).ToString());
        float fill = Mathf.Clamp(cooldownLeft/spell_cd, 0f, 1f);
        spellComps.Item3.fillAmount = fill;
    }

    /*
    *   SpellCDChildrenSetActive - Sets the children of a transform as active or inactive based on given bool.
    *   @param spellType - SpellType of which UI element to adjust
    *   @param isActive - bool of wether to set children active or inactive.
    */
    public void SpellCDChildrenSetActive(SpellType spellType, bool isActive){
        Tuple<Transform, Image, Image, TMP_Text, Image> spellComps = spellComponents[spellType];
        foreach(Transform child in spellComps.Item1){
                child.gameObject.SetActive(isActive);
        }
    }
}
