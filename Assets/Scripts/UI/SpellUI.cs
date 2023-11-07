using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SpellUI : MonoBehaviour
{
    private Dictionary<SpellType, Tuple<Transform, Image, Image, TMP_Text>> spellComponents = new Dictionary<SpellType, Tuple<Transform, Image, Image, TMP_Text>>();

    // Start is called before the first frame update
    void Start()
    {
        // Store the UI components needed.
        foreach(SpellType spellType in Enum.GetValues(typeof(SpellType))){
            if(spellType != SpellType.None){
                string spell = spellType.ToString();
                Transform spellCDTransform = transform.Find(spell + "_Container/SpellContainer/Spell/CD");
                TMP_Text spellCDText = spellCDTransform.Find("Value").GetComponent<TMP_Text>();
                Image spellCDImage = spellCDTransform.Find("Slider").GetComponent<Image>();
                Image spellCover = spellCDTransform.Find("Cover").GetComponent<Image>();
                spellComponents.Add(spellType, new Tuple<Transform, Image, Image, TMP_Text>(spellCDTransform, spellCover, spellCDImage, spellCDText));
            }
        }
        
        // Setup callbacks.
        Spell[] objSpells = GetComponentsInParent<Spell>();
        foreach(Spell spell in objSpells){
            spell.spellController.SpellCDUpdateCallback += SpellCDTimerUpdate;
            spell.SpellCDSetActiveCallback += SpellCDChildrenSetActive;
        }
    }

    /*
    *   SpellCDTimerUpdate - Updates the UI for a spells cooldown.
    *   @param spellType - SpellType being updated.
    *   @param cooldownLeft - the remaining cooldown on the spell.
    *   @param spell_cd - float representing the spells cooldown.
    */
    private void SpellCDTimerUpdate(SpellType spellType, float cooldownLeft, float spell_cd){
        Tuple<Transform, Image, Image, TMP_Text> spellComps = spellComponents[spellType];
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
        Tuple<Transform, Image, Image, TMP_Text> spellComps = spellComponents[spellType];
        foreach(Transform child in spellComps.Item1){
                child.gameObject.SetActive(isActive);
        }
    }
}
