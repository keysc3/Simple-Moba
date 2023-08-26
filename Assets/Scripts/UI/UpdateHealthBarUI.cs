using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
* Purpose: Updates the UI to display a players health on their player bar.
*
* @author: Colin Keys
*/
public class UpdateHealthBarUI : MonoBehaviour
{
    public IPlayer player; 
    private Slider health;
    private ChampionStats championStats;

    // Called when the script instance is being loaded.
    private void Awake(){
        health = GetComponent<Slider>();
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
                // Set the fill based on players health percent.
                health.value = healthPercent;
            }
            else{
                // Set players health text and fill to 0.
                player.playerBar.SetActive(false);
            }
        }
    } 
}
