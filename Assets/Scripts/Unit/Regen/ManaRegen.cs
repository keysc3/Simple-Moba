using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
* Purpose: Implements mana regeneration for a unit.
*
* @author: Colin Keys
*/
public class ManaRegen : MonoBehaviour
{
    private IUnit unit;
    private ChampionStats championStats;
    private float regenAmount = 0;

    public delegate void UpdateManaRegenUI(float value);
    public event UpdateManaRegenUI UpdateManaRegenCallback;

    // Start is called before the first frame update.
    private void Start()
    {
        unit = GetComponent<IUnit>();
        championStats = (ChampionStats) unit.unitStats;
        StartCoroutine(RegenMana());
    }

    // Called once per frame.
    private void Update(){
        // 0.5s intervals over 5s = 10 increments.
        regenAmount = championStats.MP5.GetValue()/10.0f;
        if(unit.IsDead || championStats.CurrentMana >= championStats.maxMana.GetValue())
            UpdateManaRegenCallback?.Invoke(0);
        else
            UpdateManaRegenCallback?.Invoke(championStats.MP5.GetValue());
    }

    /*
    *   RegenMana - Regens the champions mana every 0.5s based on their HP5 stat.
    */
    private IEnumerator RegenMana(){
        while(gameObject){
            if(unit != null && !unit.IsDead){
                // 0.5s intervals over 5s = 10 increments.
                float maxMana = championStats.maxMana.GetValue();
                float currentMana = championStats.CurrentMana;
                // If not at max mana then regen mana.
                if(currentMana < maxMana){
                    currentMana += regenAmount;
                    // If regen goes over max Mana set current mana to max.
                    if(currentMana > maxMana)
                        currentMana = maxMana;
                    // Set mana
                    championStats.CurrentMana = currentMana;
                }
            }
            // Only regen every 0.5s.
            yield return new WaitForSeconds(0.5f);
        }
    }
}
