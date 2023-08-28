using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
* Purpose: Updates the UI to display a players stats on their UI panel.
*
* @author: Colin Keys
*/
public class UpdateAllStatsUI : MonoBehaviour
{
    public IPlayer player;
    private ChampionStats championStats;
    private Dictionary<string, TMP_Text> statTextComponents = new Dictionary<string, TMP_Text>();
    
    private void Awake(){
        for (int i = 0; i < transform.childCount; i++){
            statTextComponents.Add(transform.GetChild(i).name, 
            transform.GetChild(i).Find("Value").GetComponent<TMP_Text>());
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        championStats = (ChampionStats) player.unitStats;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateStats();
    }

    /*
    *   UpdateAllStatsUI - Update the player UIs stats panel.
    */
    private void UpdateStats(){
        if(statTextComponents.Count == 8 && player != null){
            UpdateStat("PhysicalDamage", championStats.physicalDamage.GetValue());
            UpdateStat("Armor", championStats.armor.GetValue());
            if(championStats.attackSpeed.GetValue() > 2.5f)
                UpdateStat("AttackSpeed", 2.5f);
            else
                UpdateStat("AttackSpeed", championStats.attackSpeed.GetValue());
            UpdateStat("Crit", 0f);
            UpdateStat("MagicDamage", championStats.magicDamage.GetValue());
            UpdateStat("MagicResist", championStats.magicResist.GetValue());
            UpdateStat("Haste", championStats.haste.GetValue());
            UpdateStat("Speed", championStats.speed.GetValue());
        }
    }

    /*
    *   UpdateStat - Updates the UI value for a stat.
    *   @param statName - string of the stat to update.
    *   @param value - float of the value to update the UI to.
    */
    private void UpdateStat(string statName, float value){
        if(statName != "AttackSpeed")
            statTextComponents[statName].SetText(Mathf.Round(value).ToString());
        else
            statTextComponents[statName].SetText((Mathf.Round(value * 100f) * 0.01f).ToString());
    }
}
