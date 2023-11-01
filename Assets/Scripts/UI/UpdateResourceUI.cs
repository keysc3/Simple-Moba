using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateResourceUI : MonoBehaviour
{
    private Slider healthSlider;
    private Slider manaSlider;
    private TMP_Text healthText;
    private TMP_Text manaText;
    private TMP_Text healthRegenText;
    private TMP_Text manaRegenText;

    // Start is called before the first frame update
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

    private void UpdateHealthSliderUI(UnitStats unitStats){
        // If the unit is dead.
        if(unitStats.CurrentHealth > 0){
            // Get the health percent the player is at and set the health bar text to currenthp/maxhp.
            float healthPercent = Mathf.Clamp(unitStats.CurrentHealth/unitStats.maxHealth.GetValue(), 0f, 1f);
            healthText.SetText(Mathf.Ceil(unitStats.CurrentHealth) + "/" + Mathf.Ceil(unitStats.maxHealth.GetValue()));
            healthSlider.value = healthPercent;
        }
        else{
            // Set players health text and fill to 0.
            healthText.SetText(0 + "/" + Mathf.Ceil(unitStats.maxHealth.GetValue()));
            healthSlider.value = 0;
        }
    }

    private void UpdateManaSliderUI(ChampionStats championStats){
        // Get the percent of mana the player has left and set the mana bar text to currentmana/maxmana
        float manaPercent = Mathf.Clamp(championStats.CurrentMana/championStats.maxMana.GetValue(), 0f, 1f);
        manaText.SetText(Mathf.Ceil(championStats.CurrentMana) + "/" + Mathf.Ceil(championStats.maxMana.GetValue()));
        manaSlider.value = manaPercent;
    }

    private void UpdateHealthRegenText(float value){
        UpdateText(healthRegenText, value);
    }

    private void UpdateManaRegenText(float value){
        UpdateText(manaRegenText, value);
    }

    private void UpdateText(TMP_Text textObject, float value){
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
