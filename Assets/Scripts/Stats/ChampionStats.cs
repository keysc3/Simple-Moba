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

    /*
    *   InventoryChanged - Modifies the champions stat modifiers when an item is bought or sold.
    *   @param item - Item to that was added or removed from the inventory.
    *   @param changeType - string of the type of change that was made (buy or sell).
    */
    public void InventoryChanged(Item item, string changeType){
        if(item != null){
            // Bought an item so add the items stats.
            if(changeType == "Buy"){
                magicDamage.AddModifier(item.magicDamage);
                physicalDamage.AddModifier(item.physicalDamage);
                // Increase current health by items health amount.
                if(item.health != 0){
                    maxHealth.AddModifier(item.health);
                    if(!isDead){
                        SetHealth(currentHealth + item.health);
                        GetComponent<UIManager>().UpdateHealthBar();
                    }
                }
                // Increase current mana by items mana amount.
                if(item.mana != 0){
                    maxMana.AddModifier(item.mana);
                    if(!isDead){
                        SetMana(currentMana + item.mana);
                        GetComponent<UIManager>().UpdateManaBar();
                    }
                }
                speed.AddModifier(item.speed);
                bonusAttackSpeed.AddModifier(item.attackSpeed);
            }
            // Selling an item so remove the items stats.
            else if(changeType == "Sell"){
                magicDamage.RemoveModifier(item.magicDamage);
                physicalDamage.RemoveModifier(item.physicalDamage);
                //If the items hp hasn't been used yet, remove the amount that hasn't been used from current health.
                if(item.health != 0){
                    if(currentHealth > (maxHealth.GetValue() - item.health))
                        SetHealth(currentHealth - (item.health - (maxHealth.GetValue() - currentHealth))); 
                    maxHealth.RemoveModifier(item.health);
                    GetComponent<UIManager>().UpdateHealthBar();
                }
                //If the items mana hasn't been used yet, remove the amount that hasn't been used from the current mana.
                if(item.mana != 0){
                    if(currentMana > (maxMana.GetValue() - item.mana))
                        SetMana(currentMana - (item.mana - (maxMana.GetValue() - currentMana)));
                    maxMana.RemoveModifier(item.mana);
                    GetComponent<UIManager>().UpdateManaBar();
                }
                speed.RemoveModifier(item.speed);
                bonusAttackSpeed.RemoveModifier(item.attackSpeed);
            }
        }
    }
}