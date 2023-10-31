using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdatePlayerBarUI : MonoBehaviour
{
    private Slider healthSlider;
    private Slider manaSlider;
    private TMP_Text levelText;

    // Start is called before the first frame update
    void Start()
    {
        SetupCallback();
        Transform healthBar = transform.Find("Health");
        healthSlider = healthBar.GetComponent<Slider>();
        Transform manaBar = transform.Find("Mana");
        if(manaBar != null)
            manaSlider = manaBar.GetComponent<Slider>();
        Transform levelTransform = transform.Find("Level/Value");
        if(levelTransform != null)
            levelText = levelTransform.GetComponent<TMP_Text>();
    }

    private void SetupCallback(){
        IUnit unit = GetComponentInParent<IUnit>();
        unit.unitStats.UpdateHealthCallback += UpdateHealthSliderUI;
        if(unit is IPlayer){
            ((ChampionStats) unit.unitStats).UpdateManaCallback += UpdateManaSliderUI;
            ((IPlayer) unit).levelManager.UpdateLevelCallback += UpdateLevelUI;
        }
    }

    private void UpdateHealthSliderUI(UnitStats unitStats){
        if(unitStats.CurrentHealth > 0){
            // Get the health percent the unit is at and set the health bar text to currenthp/maxhp.
            float healthPercent = Mathf.Clamp(unitStats.CurrentHealth/unitStats.maxHealth.GetValue(), 0f, 1f);
            // Set the fill based on units health percent.
            healthSlider.value = healthPercent;
        }
    }

    private void UpdateManaSliderUI(ChampionStats championStats){
        if(championStats.CurrentMana > 0){
            // Get the percent of mana the player has left and set the mana bar text to currentmana/maxmana
            float manaPercent = Mathf.Clamp(championStats.CurrentMana/championStats.maxMana.GetValue(), 0f, 1f);
            // Set the fill based on players mana percent.
            manaSlider.value = manaPercent;
        }
    }

    private void UpdateLevelUI(int level){
        levelText.SetText(level.ToString());
    }
}
