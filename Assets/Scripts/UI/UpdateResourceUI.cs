using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
* Purpose: Updates the players resource section of the UI.
*
* @author: Colin Keys
*/
public class UpdateResourceUI : MonoBehaviour
{
    private Slider healthSlider;
    private Slider manaSlider;
    private TMP_Text healthText;
    private TMP_Text manaText;
    private TMP_Text healthRegenText;
    private TMP_Text manaRegenText;

    // Start is called before the first frame update.
    void Start()
    {
        Transform healthBar = transform.Find("HealthContainer/HealthBar");
        healthSlider = healthBar.GetComponent<Slider>();
        healthText = healthBar.Find("Value").GetComponent<TMP_Text>();
        healthRegenText = healthBar.Find("Regen").GetComponent<TMP_Text>();
        Transform manaBar = transform.Find("ManaContainer/ManaBar");
        manaSlider = manaBar.GetComponent<Slider>();
        manaText = manaBar.Find("Value").GetComponent<TMP_Text>();
        manaRegenText = manaBar.Find("Regen").GetComponent<TMP_Text>();
        SetupCallback();
    }

    /*
    *   SetupCallback - Adds the UI updating methods to the necessary events.
    */
    private void SetupCallback(){
        IUnit unit = GetComponentInParent<IUnit>();
        unit.unitStats.UpdateHealthCallback += UpdateHealthSliderUI;
        UpdateHealthSliderUI(unit.unitStats);
        if(unit is IPlayer){
            ((ChampionStats) unit.unitStats).UpdateManaCallback += UpdateManaSliderUI;
            UpdateManaSliderUI((ChampionStats) unit.unitStats);
        }
        HealthRegen healthRegenScript = GetComponentInParent<HealthRegen>();
        healthRegenScript.UpdateHealthRegenCallback += UpdateHealthRegenText;
        ManaRegen manaRegenScript = GetComponentInParent<ManaRegen>();
        if(manaRegenScript != null)
            manaRegenScript.UpdateManaRegenCallback += UpdateManaRegenText;
    }

    /*
    *   UpdateHealthSliderUI - Updates the health slider fill and text value.
    *   @param unitStats - Unit stats to use for updating the displayed health values.
    */
    private void UpdateHealthSliderUI(UnitStats unitStats){
        // If the unit is dead.
        if(unitStats.CurrentHealth > 0){
            UpdateResource(healthText, healthSlider, unitStats.CurrentHealth, unitStats.maxHealth.GetValue());
        }
        else{
            // Set players health text and fill to 0.
            healthText.SetText(0 + "/" + Mathf.Ceil(unitStats.maxHealth.GetValue()));
            healthSlider.value = 0;
        }
    }

    /*
    *   UpdateManaSliderUI - Updates the mana slider fill and text value.
    *   @param unitStats - Unit stats to use for updating the displayed mana values.
    */
    private void UpdateManaSliderUI(ChampionStats championStats){
        UpdateResource(manaText, manaSlider, unitStats.CurrentMana, unitStats.maxMana.GetValue());
    }

    /*
    *   UpdateResource - Updates the UI objects for a resource with its current values.
    *   @param textObject - TMP_Text object to update the text for.
    *   @param slider - Slider object to update the fill for.
    *   @param current - float of the current resource value.
    *   @param max - float of the max resource value.
    */
    private void UpdateResource(TMP_Text textObject, Slider slider, float current, float max){
        // Get the percent of the resource the player has left and set the text to current/max
        float percent = Mathf.Clamp(current/max, 0f, 1f);
        textObject.SetText(Mathf.Ceil(current) + "/" + Mathf.Ceil(max));
        slider.value = percent;
    }

    /*
    *   UpdateHealthRegenText - Updates the text for the players health regeneration.
    *   @param value - float of the value to display.
    */
    private void UpdateHealthRegenText(float value){
        UpdateRegenText(healthRegenText, value);
    }

    /*
    *   UpdateManaRegenText - Updates the text for the players mana regeneration.
    *   @param value - float of the value to use.
    */
    private void UpdateManaRegenText(float value){
        UpdateRegenText(manaRegenText, value);
    }

    /*
    *   UpdateRegenText - Updates a text object with a given value rounded to 2 decimals.
    *   @param textObject - TMP_Text object to update.
    *   @param value - float of the value to use.
    */
    private void UpdateRegenText(TMP_Text textObject, float value){
        if(value != 0f){
           textObject.SetText($"+{Mathf.Round((value/5.0f) * 100.0f) * 0.01f}");
            if(!textObject.gameObject.activeSelf)
               textObject.gameObject.SetActive(true);
        }
        else{
            if(textObject.gameObject.activeSelf)
               textObject.gameObject.SetActive(false);
        }
    }
}
