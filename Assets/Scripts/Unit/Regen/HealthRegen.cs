using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
* Purpose: Implements health regeneration for a unit.
*
* @author: Colin Keys
*/
public class HealthRegen : MonoBehaviour
{
    private IUnit unit;
    private float regenAmount = 0;

    public delegate void UpdateHealthRegenUI(float value);
    public event UpdateHealthRegenUI UpdateHealthRegenCallback;
    
    // Start is called before the first frame update.
    private void Start()
    {
        unit = GetComponent<IUnit>();
        StartCoroutine(RegenHealth());
    }

    // Called once per frame.
    private void Update(){
        // 0.5s intervals over 5s = 10 increments.
        regenAmount = unit.unitStats.HP5.GetValue()/10.0f;
        if(unit.IsDead || unit.unitStats.CurrentHealth >= unit.unitStats.maxHealth.GetValue())
            UpdateHealthRegenCallback?.Invoke(0);
        else
            UpdateHealthRegenCallback?.Invoke(unit.unitStats.HP5.GetValue());
    }

    /*
    *   RegenHealth - Regens the champions health every 0.5s based on their HP5 stat.
    */
    private IEnumerator RegenHealth(){
        while(gameObject){
            if(unit != null && !unit.IsDead){
                float maxHealth = unit.unitStats.maxHealth.GetValue();
                float currentHealth = unit.unitStats.CurrentHealth;
                // If not at max health then regen health.
                if(currentHealth < maxHealth){
                    currentHealth += regenAmount;
                    // If regen goes over max health set current health to max.
                    if(currentHealth > maxHealth)
                        currentHealth = maxHealth;
                    // Set health
                    unit.unitStats.CurrentHealth = currentHealth;
                }
            }
            // Only regen every 0.5s.
            yield return new WaitForSeconds(0.5f);
        }
    }
}
