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

    public static UIManager instance { get; private set; }

    [SerializeField] private GameObject playerBarPrefab;
    [SerializeField] private GameObject playerUIPrefab;

    // Called when the script instance is being loaded.
    private void Awake(){
        if(instance == null)
            instance = this;
    }
    
    /*
    *   InitialValueSetup - Initializes the players resource and stat UI values.
    *   @param playerUI - GameObject of the playerUI.
    *   @param playerBar - GameObject of the playerBar.
    *   @param championStats - ChampionStats of the players stats.
    */
    public void InitialValueSetup(GameObject playerUI, GameObject playerBar, ChampionStats championStats){
        UpdatePlayerUIHealthBar(playerUI, championStats, false);
        UpdatePlayerBarHealthBar(playerBar, championStats, false);
        UpdateManaUIs(playerUI, playerBar, championStats);
        UpdateAllStatsUI(playerUI, championStats);
    }

    /*
    *   UpdatePlayerUIHealthBar - Update the players UI health bar.
    *   @param playerUI - GameObject of the playerUI.
    *   @param championStats - ChampionStats of the players stats.
    *   @param isDead - bool for wether or not the player is dead.
    */
    public void UpdatePlayerUIHealthBar(GameObject playerUI, ChampionStats championStats, bool isDead){
        Slider health = playerUI.transform.Find("Player/Combat/ResourceContainer/HealthContainer/HealthBar").GetComponent<Slider>();
        TMP_Text healthText = health.transform.Find("Value").GetComponent<TMP_Text>();
        // If the champion is dead.
        if(!isDead){
            // Get the health percent the player is at and set the health bar text to currenthp/maxhp.
            float healthPercent = Mathf.Round((championStats.CurrentHealth/championStats.maxHealth.GetValue()) * 100);
            healthText.SetText(Mathf.Ceil(championStats.CurrentHealth) + "/" + Mathf.Ceil(championStats.maxHealth.GetValue()));
            health.value = healthPercent;
        }
        else{
            // Set players health text and fill to 0.
            healthText.SetText(0 + "/" + Mathf.Ceil(championStats.maxHealth.GetValue()));
            health.value = 0;
        }
    }

    /*
    *   UpdatePlayerBarHealthBar - Update the player bar health bar.
    *   @param playerBar - GameObject of the playerBar.
    *   @param championStats - ChampionStats of the players stats.
    *   @param isDead - bool for wether or not the player is dead.
    */
    public void UpdatePlayerBarHealthBar(GameObject playerBar, ChampionStats championStats, bool isDead){
        // If the champion is dead.
        if(!isDead){
            // Get the health percent the player is at and set the health bar text to currenthp/maxhp.
            float healthPercent = Mathf.Round((championStats.CurrentHealth/championStats.maxHealth.GetValue()) * 100);
            // Set the fill based on players health percent.
            playerBar.transform.Find("PlayerBar/Container/Health").GetComponent<Slider>().value = healthPercent;
        }
        else{
            // Set players health text and fill to 0.
            playerBar.SetActive(false);
        }
    }

    /*
    *   UpdateManaUIs - Update the player UI and player bar mana element.
    *   @param playerUI - GameObject of the playerUI.
    *   @param playerBar - GameObject of the playerBar.
    *   @param championStats - ChampionStats of the players stats.
    */
    public void UpdateManaUIs(GameObject playerUI, GameObject playerBar, ChampionStats championStats){
        Slider mana = playerUI.transform.Find("Player/Combat/ResourceContainer/ManaContainer/ManaBar").GetComponent<Slider>();
        // Get the percent of mana the player has left and set the mana bar text to currentmana/maxmana
        float manaPercent = Mathf.Round((championStats.CurrentMana/championStats.maxMana.GetValue()) * 100);
        mana.transform.Find("Value").GetComponent<TMP_Text>()
        .SetText(Mathf.Ceil(championStats.CurrentMana) + "/" + Mathf.Ceil(championStats.maxMana.GetValue()));
        // Set the fill based on the player mana percent.
        playerBar.transform.Find("PlayerBar/Container/Mana").GetComponent<Slider>().value = manaPercent;
        mana.value = manaPercent;
    }

    /*
    *   UpdateAllStatsUI - Update the player UIs stats panel.
    *   @param playerUI - GameObject of the playerUI.
    *   @param championStats - ChampionStats of the players stats.
    */
    public void UpdateAllStatsUI(GameObject playerUI, ChampionStats championStats){
        Transform statsContainer = playerUI.transform.Find("Player/Info/Stats/Container");
        UpdateStat("PhysicalDamage", championStats.physicalDamage.GetValue(), statsContainer);
        UpdateStat("Armor", championStats.armor.GetValue(), statsContainer);
        if(championStats.attackSpeed.GetValue() > 2.5f)
            UpdateStat("AttackSpeed", 2.5f, statsContainer);
        else
            UpdateStat("AttackSpeed", championStats.attackSpeed.GetValue(), statsContainer);
        UpdateStat("Crit", 0f, statsContainer);
        UpdateStat("MagicDamage", championStats.magicDamage.GetValue(), statsContainer);
        UpdateStat("MagicResist", championStats.magicResist.GetValue(), statsContainer);
        UpdateStat("Haste", championStats.haste.GetValue(), statsContainer);
        UpdateStat("Speed", championStats.speed.GetValue(), statsContainer);
    }

    /*
    *   UpdateStat - Updates the UI value for a stat.
    *   @param statName - string of the stat to update.
    *   @param value - float of the value to update the UI to.
    *   @param statsContainer - transform of the stats UI GameObjects parent.
    */
    public void UpdateStat(string statName, float value, Transform statsContainer){
        if(statName != "AttackSpeed")
            statsContainer.Find(statName).Find("Value").GetComponent<TMP_Text>().SetText(Mathf.Round(value).ToString());
        else
            statsContainer.Find(statName).Find("Value").GetComponent<TMP_Text>().SetText((Mathf.Round(value * 100f) * 0.01f).ToString());
    }
}
