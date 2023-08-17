using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewHealthRegen : MonoBehaviour
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
    private TMP_Text healthRegenText;
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

    // Called when the script instance is being loaded.
    private void Awake(){
        Unit = GetComponent<IUnit>();
    }

    // Start is called before the first frame update.
    private void Start()
    {
        StartCoroutine(RegenHealth());
    }

    // Called once per frame.
    private void Update(){
        HealthToRegen = Mathf.Round(unit.unitStats.HP5.GetValue()/10.0f);
    }

    /*
    *   RegenHealth - Regens the champions health every 0.5s based on their HP5 stat.
    */
    private IEnumerator RegenHealth(){
        while(gameObject){
            // 0.5s intervals over 5s = 10 increments.
            float maxHealth = unit.unitStats.maxHealth.GetValue();
            float currentHealth = unit.unitStats.CurrentHealth;
            // If not at max mana and not dead then regen health.
            if(currentHealth < maxHealth && !unit.IsDead){
                currentHealth += HealthToRegen;
                // If regen goes over max health set current health to max.
                if(currentHealth > maxHealth)
                    currentHealth = maxHealth;
                // Set health and only regen every 0.5s.
                unit.unitStats.CurrentHealth = currentHealth;
                //UIManager.instance.UpdateHealthBar(player);
                // Display per 1s on UI.
                //UIManager.instance.UpdateHealthRegen(championStats.HP5.GetValue()/5.0f, player.playerUI);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
