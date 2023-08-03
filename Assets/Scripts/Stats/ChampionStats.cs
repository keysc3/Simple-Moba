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
        MP5 = new Stat(((ScriptableChampion) champion).MP5);
        maxMana = new Stat(((ScriptableChampion) champion).baseMana);
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

    /*
    *   AddItemStats - Add an items stats to the champions stats.
    *   @param item - Item whose stats to add.
    */
    public void AddItemStats(Item item){
        if(item != null){
            // Bought an item so add the items stats.
            magicDamage.AddModifier(item.magicDamage);
            physicalDamage.AddModifier(item.physicalDamage);
            // Increase current health by items health amount.
            if(item.health != 0){
                maxHealth.AddModifier(item.health);
                CurrentHealth = CurrentHealth + item.health;
            }
            // Increase current mana by items mana amount.
            if(item.mana != 0){
                maxMana.AddModifier(item.mana);
                SetMana(currentMana + item.mana);
            }
            speed.AddModifier(item.speed);
            bonusAttackSpeed.AddModifier(item.attackSpeed);
            UpdateAttackSpeed();
        }
    }

    /*
    *   RemoveItemStats - Remove an items stats from the champions stats.
    *   @param item - Item whose stats to remove.
    */
    public void RemoveItemStats(Item item){
        if(item != null){
            // Selling an item so remove the items stats.
            magicDamage.RemoveModifier(item.magicDamage);
            physicalDamage.RemoveModifier(item.physicalDamage);
            //If the items hp hasn't been used yet, remove the amount that hasn't been used from current health.
            if(item.health != 0){
                if(CurrentHealth > (maxHealth.GetValue() - item.health))
                    CurrentHealth = CurrentHealth - (item.health - (maxHealth.GetValue() - CurrentHealth)); 
                maxHealth.RemoveModifier(item.health);
            }
            //If the items mana hasn't been used yet, remove the amount that hasn't been used from the current mana.
            if(item.mana != 0){
                if(currentMana > (maxMana.GetValue() - item.mana))
                    SetMana(currentMana - (item.mana - (maxMana.GetValue() - currentMana)));
                maxMana.RemoveModifier(item.mana);
            }
            speed.RemoveModifier(item.speed);
            bonusAttackSpeed.RemoveModifier(item.attackSpeed);
            UpdateAttackSpeed();
        }
    }
}