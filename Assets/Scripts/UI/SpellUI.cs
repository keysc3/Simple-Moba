using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SpellUI : MonoBehaviour
{
    private Dictionary<SpellType, Tuple<Transform, TMP_Text, Image>> spellComponents = new Dictionary<SpellType, Tuple<Transform, TMP_Text, Image>>();

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
                spellComponents.Add(spellType, new Tuple<Transform, TMP_Text, Image>(spellCDTransform, spellCDText, spellCDImage));
            }
        }
        // Setup callbacks.
        Spell[] objSpells = GetComponentsInParent<Spell>();
        foreach(Spell spell in objSpells){
            spell.spellController.SpellCDUpdateCallback += SpellCDTimerUpdate;
        }
    }

    /*
    *   SpellCDTimerUpdate - Updates the UI for a spells cooldown.
    *   @param spellType - SpellType being updated.
    *   @param cooldownLeft - the remaining cooldown on the spell.
    *   @param spell_cd - float representing the spells cooldown.
    */
    private void SpellCDTimerUpdate(SpellType spellType, float cooldownLeft, float spell_cd){
        Tuple<Transform, TMP_Text, Image> spellComps = spellComponents[spellType];
        spellComps.Item2.SetText(Mathf.Ceil(cooldownLeft).ToString());
        float fill = Mathf.Clamp(cooldownLeft/spell_cd, 0f, 1f);
        spellComps.Item3.fillAmount = fill;
    }
}
