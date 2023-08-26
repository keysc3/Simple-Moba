using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
* Purpose: Updates the UI to display a players mana on their player bar.
*
* @author: Colin Keys
*/
public class UpdateManaBarUI : MonoBehaviour
{
    public IPlayer player; 
    private Slider mana;
    private ChampionStats championStats;

    // Called when the script instance is being loaded.
    private void Awake(){
        mana = GetComponent<Slider>();
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
                // Get the percent of mana the player has left and set the mana bar text to currentmana/maxmana
                float manaPercent = Mathf.Round((championStats.CurrentMana/championStats.maxMana.GetValue()) * 100);
                // Set the fill based on players mana percent.
                mana.value = manaPercent;
            }
        }
    } 
}
