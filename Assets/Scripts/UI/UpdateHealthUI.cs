using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
* Purpose: Updates the UI to display a players health on their UI panel.
*
* @author: Colin Keys
*/
public class UpdateHealthUI : MonoBehaviour
{
    public IPlayer player; 
    private Slider health;
    private TMP_Text healthText;
    private ChampionStats championStats;

    // Called when the script instance is being loaded.
    private void Awake(){
        health = GetComponent<Slider>();
        healthText = transform.Find("Value").GetComponent<TMP_Text>();
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        championStats = (ChampionStats) player.unitStats; 
    }

    // Update is called once per frame
    private void Update()
    {
        if(player != null){
            // If the champion is dead.
            if(!player.IsDead){
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
    }
}
