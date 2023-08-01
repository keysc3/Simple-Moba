using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements mana regeneration for a unit.
*
* @author: Colin Keys
*/
public class ManaRegen : MonoBehaviour
{
    private Player player;
    private ChampionStats championStats;

    // Called when the script instance is being loaded.
    private void Awake(){
        player = GetComponent<Player>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        championStats = (ChampionStats) player.unitStats;
        StartCoroutine(RegenMana());
    }

    // Called once per frame.
    private void Update(){
        // Activate mana regen UI if the player is not full mana.
        if(championStats.currentMana < championStats.maxMana.GetValue() && !player.isDead){
            UIManager.instance.SetManaRegenActive(true, player.playerUI);
        }
        else{
            UIManager.instance.SetManaRegenActive(false, player.playerUI);
        }
    }

    /*
    *   RegenMana - Regens the champions mana every 0.5s based on their MP5 stat.
    */
    private IEnumerator RegenMana(){
        float manaToRegen;
        while(gameObject){
            // 0.5s intervals over 5s = 10 increments.
             manaToRegen = Mathf.Round(championStats.MP5.GetValue()/10.0f);
            float maxMana = championStats.maxMana.GetValue();
            float currentMana = championStats.currentMana;
            // If not at max mana and not dead then regen mana.
            if(currentMana < maxMana && !player.isDead){
                currentMana += manaToRegen;
                // If regen goes over max mana set current mana to max.
                if(currentMana > maxMana)
                    currentMana = maxMana;
                // Set mana and only regen every 0.5s.
                championStats.SetMana(currentMana);
                UIManager.instance.UpdateManaBar(championStats, player.playerUI, player.playerBar);
                // Display per 1s on UI.
                UIManager.instance.UpdateManaRegen(championStats.MP5.GetValue()/5.0f, player.playerUI);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
