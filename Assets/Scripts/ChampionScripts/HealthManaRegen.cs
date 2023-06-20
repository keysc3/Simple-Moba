using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Handles health and mana regeneration.
*
* @author: Colin Keys
*/
public class HealthManaRegen : MonoBehaviour
{
    private ChampionStats championStats;
    private UIManager uiManager;

    // Called when the script instance is being loaded.
    private void Awake(){
        championStats = GetComponent<ChampionStats>();
        uiManager = GetComponent<UIManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(RegenMana());
        StartCoroutine(RegenHealth());
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
            if(currentMana < maxMana && !championStats.isDead){
                currentMana += manaToRegen;
                // If regen goes over max mana set current mana to max.
                if(currentMana > maxMana)
                    currentMana = maxMana;
                // Set mana and only regen every 0.5s.
                championStats.SetMana(currentMana);
                uiManager.UpdateManaBar();
                // Display per 1s on UI.
                uiManager.UpdateManaRegen(championStats.MP5.GetValue()/5.0f);
            }
            yield return new WaitForSeconds(0.5f);
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
            float currentHealth = championStats.currentHealth;
            // If not at max mana and not dead then regen health.
            if(currentHealth < maxHealth && !championStats.isDead){
                currentHealth += healthToRegen;
                // If regen goes over max health set current health to max.
                if(currentHealth > maxHealth)
                    currentHealth = maxHealth;
                // Set health and only regen every 0.5s.
                championStats.SetHealth(currentHealth);
                uiManager.UpdateHealthBar();
                // Display per 1s on UI.
                uiManager.UpdateHealthRegen(championStats.HP5.GetValue()/5.0f);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
