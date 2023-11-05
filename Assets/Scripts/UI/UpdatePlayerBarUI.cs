using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
* Purpose: Updates the players player bar.
*
* @author: Colin Keys
*/
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

    /*
    *   SetupCallback - Adds the UI updating methods to the necessary events.
    */
    private void SetupCallback(){
        IUnit unit = GetComponentInParent<IUnit>();
        unit.unitStats.UpdateHealthCallback += UpdateHealthSliderUI;
        if(unit is IPlayer){
            ((ChampionStats) unit.unitStats).UpdateManaCallback += UpdateManaSliderUI;
            ((IPlayer) unit).levelManager.UpdateLevelCallback += UpdateLevelUI;
        }
    }

    /*
    *   UpdateHealthSliderUI - Updates the health slider fill value.
    *   @param unitStats - Unit stats to use for updating the displayed health values.
    */
    private void UpdateHealthSliderUI(UnitStats unitStats){
        UpdateResource(healthSlider, unitStats.CurrentHealth, unitStats.maxHealth.GetValue());
    }

    /*
    *   UpdateManaSliderUI - Updates the mana slider fill value.
    *   @param unitStats - Unit stats to use for updating the displayed mana values.
    */
    private void UpdateManaSliderUI(ChampionStats championStats){
        UpdateResource(manaSlider, championStats.CurrentMana, championStats.maxMana.GetValue());
    }

    /*
    *   UpdateResource - Updates the player bar resource with its current values.
    *   @param slider - Slider object to update the fill for.
    *   @param current - float of the current resource value.
    *   @param max - float of the max resource value.
    */
    private void UpdateResource(Slider slider, float current, float max){
        if(current > 0){
            // Get the percent of the resource the player has left.
            float percent = Mathf.Clamp(current/max, 0f, 1f);
            slider.value = percent;
        }
    }

    /*
    *   UpdateLevelUI - Updates the players level text.
    *   @param level - int of the level to display.
    */
    private void UpdateLevelUI(int level){
        levelText.SetText(level.ToString());
    }
}
