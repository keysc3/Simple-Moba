using UnityEngine;

/*
* Purpose: Implements champion stats. Extends UnitStats.
*
* @author: Colin Keys
*/
public class ChampionStats : UnitStats
{
    [field: SerializeField] public float currentMana { get; private set; }
    [field: SerializeField] public float displayCurrentMana { get; private set; }
    [field: SerializeField] public Stat maxMana { get; private set; }
    [field: SerializeField] public Stat MP5 { get; private set; }
    [field: SerializeField] public Stat haste { get; private set; }

    // Called when the script instance is being loaded.
    protected override void Awake(){
        base.Awake();
        MP5 = new Stat(((Champion) unit).MP5);
        maxMana = new Stat(((Champion) unit).baseMana);
        haste = new Stat(0f);
        currentMana = maxMana.GetValue();
    }

    private void Update(){
        displayCurrentMana = currentMana; 
        displayCurrentHealth = currentHealth;
    }

    /*
    *   UseMana - Uses an amount of mana from the champions mana pool.
    *   @param cost - float of the amount of mana to use.
    */
    public void UseMana(float cost){
        currentMana -= cost;
        if(currentMana < 0)
            currentMana = 0;
        GetComponent<UIManager>().UpdateManaBar();
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

    public override void TakeDamage(float incomingDamage, string damageType, GameObject from, bool isDot){
        base.TakeDamage(incomingDamage, damageType, from, isDot);
        GetComponent<DamageTracker>().DamageTaken(from, incomingDamage, damageType);
        GetComponent<UIManager>().UpdateHealthBar();
    }
}