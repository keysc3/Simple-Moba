using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
* Purpose: Updates the UI to display a players mana on their UI panel.
*
* @author: Colin Keys
*/
public class UpdateManaUI : MonoBehaviour
{
    public IPlayer player; 
    private Slider mana;
    private TMP_Text manaText;
    private ChampionStats championStats;

    // Called when the script instance is being loaded.
    private void Awake(){
        mana = GetComponent<Slider>();
        manaText = transform.Find("Value").GetComponent<TMP_Text>();
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
                manaText.SetText(Mathf.Ceil(championStats.CurrentMana) + "/" + Mathf.Ceil(championStats.maxMana.GetValue()));
                mana.value = manaPercent;
            }
        } 
    }
}
