using UnityEngine;

/*
* Purpose: Implements champion stats. Extends UnitStats.
*
* @author: Colin Keys
*/
public class ChampionStats : UnitStats
{
    private float currentMana;
    public float CurrentMana {
        get => currentMana;
        set{ 
            currentMana = value < maxMana.GetValue() ? value : maxMana.GetValue();
            UpdateManaCallback?.Invoke(this);
        }
    }
    public Stat maxMana { get; }
    public Stat MP5 { get; }

    public delegate void UpdateManaUI(ChampionStats championStats);
    public event UpdateManaUI UpdateManaCallback;

    /*
    *   ChampionStat - Creates a champion stats object.
    *   champion - ScriptableChampion to intialize with. 
    */
    public ChampionStats(ScriptableChampion champion) : base(champion){
        MP5 = new Stat(((ScriptableChampion) champion).MP5);
        maxMana = new Stat(((ScriptableChampion) champion).baseMana);
        currentMana = maxMana.GetValue();
    }

    /*
    *   UseMana - Uses an amount of mana from the champions mana pool.
    *   @param cost - float of the amount of mana to use.
    */
    public void UseMana(float cost){
        currentMana -= cost;
        if(currentMana < 0f)
            currentMana = 0;
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
                CurrentMana = CurrentMana + item.mana;
            }
            magicResist.AddModifier(item.magicResist);
            armor.AddModifier(item.armor);
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
            float oldMax;
            // Selling an item so remove the items stats.
            magicDamage.RemoveModifier(item.magicDamage);
            physicalDamage.RemoveModifier(item.physicalDamage);
            //If the items hp hasn't been used yet, remove the amount that hasn't been used from current health.
            if(item.health != 0){
                oldMax = maxHealth.GetValue();
                maxHealth.RemoveModifier(item.health);
                if(CurrentHealth > (oldMax - item.health))
                    CurrentHealth = CurrentHealth - (item.health - (oldMax - CurrentHealth)); 
            }
            //If the items mana hasn't been used yet, remove the amount that hasn't been used from the current mana.
            if(item.mana != 0){
                oldMax = maxMana.GetValue();
                maxMana.RemoveModifier(item.mana);
                if(currentMana > (oldMax - item.mana))
                    CurrentMana = CurrentMana - (item.mana - (oldMax - CurrentMana));
            }
            magicResist.RemoveModifier(item.magicResist);
            armor.RemoveModifier(item.armor);
            speed.RemoveModifier(item.speed);
            bonusAttackSpeed.RemoveModifier(item.attackSpeed);
            UpdateAttackSpeed();
        }
    }
}