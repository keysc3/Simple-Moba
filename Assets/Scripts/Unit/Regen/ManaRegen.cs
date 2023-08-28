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
    public IUnit Unit{
        get => unit;
        set {
            unit = value;
            if(value is IPlayer){
                if(((IPlayer) value).playerUI != null)
                    manaRegenText = ((IPlayer) value).playerUI.transform.Find("Player/Combat/ResourceContainer/ManaContainer/ManaBar/Regen").GetComponent<TMP_Text>();
            }
        }
    }
    private float manaToRegen;
    public float ManaToRegen {
        get => manaToRegen;
        private set {
            manaToRegen = value;
            if(manaRegenText != null){
                if(!unit.IsDead && championStats.CurrentMana < championStats.maxMana.GetValue())
                    // Display HP5 per 1s on UI.
                    manaRegenText.SetText("+" + (Mathf.Round((championStats.MP5.GetValue()/5.0f) * 100.0f) * 0.01f));
                else
                    manaRegenText.SetText("");
            }
        }
    }
    
    private TMP_Text manaRegenText;
    private ChampionStats championStats;
    
    // Called when the script instance is being loaded.
    private void Awake(){
        Unit = GetComponent<IUnit>();
    }

    // Start is called before the first frame update.
    private void Start()
    {
        championStats = (ChampionStats) unit.unitStats;
        StartCoroutine(RegenMana());
    }

    // Called once per frame.
    private void Update(){
        ManaToRegen = Mathf.Round(championStats.HP5.GetValue()/10.0f);
    }

    /*
    *   RegenMana - Regens the champions mana every 0.5s based on their HP5 stat.
    */
    private IEnumerator RegenMana(){
        while(gameObject){
            // 0.5s intervals over 5s = 10 increments.
            float maxMana = championStats.maxMana.GetValue();
            float currentMana = championStats.CurrentMana;
            // If not at max mana and not dead then regen mana.
            if(currentMana < maxMana && !unit.IsDead){
                currentMana += ManaToRegen;
                // If regen goes over max Mana set current mana to max.
                if(currentMana > maxMana)
                    currentMana = maxMana;
                // Set mana and only regen every 0.5s.
                championStats.CurrentMana = currentMana;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
