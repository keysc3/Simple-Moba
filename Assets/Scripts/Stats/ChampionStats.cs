using UnityEngine;

/*
* Purpose: Implements champion stats. Extends UnitStats.
*
* @author: Colin Keys
*/
public class ChampionStats : UnitStats
{
    [field: SerializeField] public float currentMana { get; private set; }
    [field: SerializeField] public Stat maxMana { get; private set; }
    [field: SerializeField] public Stat MP5 { get; private set; }
    [field: SerializeField] public Stat haste { get; private set; }

    public ChampionStats(ScriptableChampion champion) : base(champion){
        MP5 = new Stat(((ScriptableChampion) unit).MP5);
        maxMana = new Stat(((ScriptableChampion) unit).baseMana);
        haste = new Stat(0f);
        currentMana = maxMana.GetValue();
    }

    /*
    *   UseMana - Uses an amount of mana from the champions mana pool.
    *   @param cost - float of the amount of mana to use.
    */
    public void UseMana(float cost){
        currentMana -= cost;
        if(currentMana < 0)
            currentMana = 0;
        //GetComponent<UIManager>().UpdateManaBar();
    }

    /*
    *   SetMana - Set the champions current mana value.
    *   @param value - float of the value to change current mana to.
    */
    public void SetMana(float value){
        currentMana = value;
    }

    /*
    *   ResetMana - Set the champions current mana value to the max mana value.
    */
    public void ResetMana(){
        currentMana = maxMana.GetValue();
    }
}