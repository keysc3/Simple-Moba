using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements health regeneration for a unit.
*
* @author: Colin Keys
*/
public class HealthRegen : MonoBehaviour
{
    private Player player;
    private ChampionStats championStats;

    // Called when the script instance is being loaded.
    private void Awake(){
        player = GetComponent<Player>();
    }

    // Start is called before the first frame update.
    private void Start()
    {
        championStats = (ChampionStats) player.unitStats;
        StartCoroutine(RegenHealth());
    }

    // Called once per frame.
    private void Update(){
        // Activate the health regen UI if the player is not full health.
        if(championStats.CurrentHealth < championStats.maxHealth.GetValue() && !player.isDead){
            UIManager.instance.SetHealthRegenActive(true, player.playerUI);
        }
        else{
            UIManager.instance.SetHealthRegenActive(false, player.playerUI);
        }
    }

    /*
    *   RegenHealth - Regens the champions health every 0.5s based on their HP5 stat.
    */
    private IEnumerator RegenHealth(){
        float healthToRegen;
        while(gameObject){
            // 0.5s intervals over 5s = 10 increments.
            healthToRegen = Mathf.Round(championStats.HP5.GetValue()/10.0f);
            float maxHealth = championStats.maxHealth.GetValue();
            float currentHealth = championStats.CurrentHealth;
            // If not at max mana and not dead then regen health.
            if(currentHealth < maxHealth && !player.isDead){
                currentHealth += healthToRegen;
                // If regen goes over max health set current health to max.
                if(currentHealth > maxHealth)
                    currentHealth = maxHealth;
                // Set health and only regen every 0.5s.
                championStats.CurrentHealth = currentHealth;
                UIManager.instance.UpdateHealthBar(player);
                // Display per 1s on UI.
                UIManager.instance.UpdateHealthRegen(championStats.HP5.GetValue()/5.0f, player.playerUI);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
