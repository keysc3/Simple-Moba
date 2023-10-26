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
    public IUnit Unit{
        get => unit;
        set {
            unit = value;
            if(value is IPlayer){
                if(((IPlayer) value).playerUI != null)
                    healthRegenText = ((IPlayer) value).playerUI.transform.Find("Player/Combat/ResourceContainer/HealthContainer/HealthBar/Regen").GetComponent<TMP_Text>();
            }
        }
    }
    private float healthToRegen;
    public float HealthToRegen {
        get => healthToRegen;
        private set {
            healthToRegen = value;
            if(healthRegenText != null){
                if(!unit.IsDead && unit.unitStats.CurrentHealth < unit.unitStats.maxHealth.GetValue())
                    // Display HP5 per 1s on UI.
                    healthRegenText.SetText("+" + (Mathf.Round((unit.unitStats.HP5.GetValue()/5.0f) * 100.0f) * 0.01f));
                else
                    healthRegenText.SetText("");
            }
        }
    }

    private TMP_Text healthRegenText;

    // Start is called before the first frame update.
    private void Start()
    {
        Unit = GetComponent<IUnit>();
        HealthToRegen = unit.unitStats.HP5.GetValue()/10.0f;
        StartCoroutine(RegenHealth());
    }

    // Called once per frame.
    private void Update(){
        HealthToRegen = unit.unitStats.HP5.GetValue()/10.0f;
    }

    /*
    *   RegenHealth - Regens the champions health every 0.5s based on their HP5 stat.
    */
    private IEnumerator RegenHealth(){
        while(gameObject){
            if(Unit != null){
                // 0.5s intervals over 5s = 10 increments.
                float maxHealth = unit.unitStats.maxHealth.GetValue();
                float currentHealth = unit.unitStats.CurrentHealth;
                // If not at max health and not dead then regen health.
                if(currentHealth < maxHealth && !unit.IsDead){
                    Debug.Log($"REGENNING: {HealthToRegen}");
                    currentHealth += HealthToRegen;
                    // If regen goes over max health set current health to max.
                    if(currentHealth > maxHealth)
                        currentHealth = maxHealth;
                    // Set health and only regen every 0.5s.
                    unit.unitStats.CurrentHealth = currentHealth;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
