using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class champion_stats
{
    // A Test behaves as an ordinary method
    [Test]
    public void sets_current_mana_to_232_with_345_max_mana(){
        // Arrange
        ScriptableChampion sChampion = ScriptableObject.CreateInstance<ScriptableChampion>();
        ChampionStats championStats = new ChampionStats(sChampion);
        championStats.maxMana.BaseValue = 345f;

        // Act
        championStats.CurrentMana = 232f;

        // Assert 
        Assert.AreEqual(232f, championStats.CurrentMana);

    }

    // A Test behaves as an ordinary method
    [Test]
    public void sets_current_mana_of_87_to_max_mana_of_100_from_attempted_current_mana_set_of_623(){
        // Arrange
        ScriptableChampion sChampion = ScriptableObject.CreateInstance<ScriptableChampion>();
        ChampionStats championStats = new ChampionStats(sChampion);
        championStats.maxMana.BaseValue = 100f;
        championStats.CurrentMana = 87f;

        // Act
        championStats.CurrentMana = 623f;

        // Assert 
        Assert.AreEqual(100f, championStats.CurrentMana);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void sets_current_mana_to_221_with_max_mana_306_with_base_300_and_modifier_6(){
        // Arrange
        ScriptableChampion sChampion = ScriptableObject.CreateInstance<ScriptableChampion>();
        ChampionStats championStats = new ChampionStats(sChampion);
        championStats.maxMana.BaseValue = 300f;
        championStats.maxMana.AddModifier(6f);

        // Act
        championStats.CurrentMana = 221f;

        // Assert 
        Assert.AreEqual(221f, championStats.CurrentMana);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void resets_current_mana_of_512_to_max_mana_value_of_867(){
        // Arrange
        ScriptableChampion sChampion = ScriptableObject.CreateInstance<ScriptableChampion>();
        ChampionStats championStats = new ChampionStats(sChampion);
        championStats.maxMana.BaseValue = 867f;
        championStats.CurrentMana = 512f;

        // Act
        championStats.ResetMana();

        // Assert 
        Assert.AreEqual(867f, championStats.CurrentMana);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void sets_current_mana_to_491_from_562(){
        // Arrange
        ScriptableChampion sChampion = ScriptableObject.CreateInstance<ScriptableChampion>();
        ChampionStats championStats = new ChampionStats(sChampion);
        championStats.maxMana.BaseValue = 562f;
        championStats.CurrentMana = championStats.maxMana.GetValue();

        // Act
        championStats.UseMana(71f);

        // Assert 
        Assert.AreEqual(491f, championStats.CurrentMana);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void sets_current_mana_to_0_from_302_by_using_more_mana_than_max_mana(){
        // Arrange
        ScriptableChampion sChampion = ScriptableObject.CreateInstance<ScriptableChampion>();
        ChampionStats championStats = new ChampionStats(sChampion);
        championStats.maxMana.BaseValue = 302f;
        championStats.CurrentMana = championStats.maxMana.GetValue();

        // Act
        championStats.UseMana(400f);

        // Assert 
        Assert.AreEqual(0f, championStats.CurrentMana);
    }

    [Test]
    public void adds_item_stats_to_champion_stats_where_item_has_every_stat(){
        // Arrange
        ScriptableChampion sChampion = ScriptableObject.CreateInstance<ScriptableChampion>();
        ChampionStats championStats = new ChampionStats(sChampion);
        championStats.maxHealth.BaseValue = 212f;
        championStats.CurrentHealth = championStats.maxHealth.GetValue() - 50f;
        championStats.maxMana.BaseValue = 178f;
        championStats.CurrentMana = championStats.maxMana.GetValue() - 20f;
        
        Item item = ScriptableObject.CreateInstance<Item>();
        item.magicDamage = 60f;
        item.physicalDamage = 21f;
        item.health = 391f;
        item.mana = 30f;
        item.speed = 1f;
        item.magicResist = 12f;
        item.armor = 19f;
        item.attackSpeed = 8f;

        // Act
        championStats.AddItemStats(item);

        // Assert
        Assert.AreEqual((60f, 21f, 553f, 188f, 1f, 12f, 19f, 8f, 603f, 208f),
        (championStats.magicDamage.GetValue(), championStats.physicalDamage.GetValue(), championStats.CurrentHealth, 
        championStats.CurrentMana, championStats.speed.GetValue(), championStats.magicResist.GetValue(), 
        championStats.armor.GetValue(), championStats.bonusAttackSpeed.GetValue(), championStats.maxHealth.GetValue(), championStats.maxMana.GetValue()));
    }

    [Test]
    public void adds_item_stats_to_champion_stats_where_item_has_no_health_or_mana(){
        // Arrange
        ScriptableChampion sChampion = ScriptableObject.CreateInstance<ScriptableChampion>();
        ChampionStats championStats = new ChampionStats(sChampion);
        championStats.maxHealth.BaseValue = 461f;
        championStats.CurrentHealth = championStats.maxHealth.GetValue() - 80f;
        championStats.maxMana.BaseValue = 98f;
        championStats.CurrentMana = championStats.maxMana.GetValue() - 8f;
        
        Item item = ScriptableObject.CreateInstance<Item>();
        item.magicDamage = 50f;
        item.physicalDamage = 11f;
        item.speed = 1.4f;
        item.magicResist = 28f;
        item.armor = 16f;
        item.attackSpeed = 12f;

        // Act
        championStats.AddItemStats(item);

        // Assert
        Assert.AreEqual((50f, 11f, 381f, 90f, 1.4f, 28f, 16f, 12f, 461f, 98f),
        (championStats.magicDamage.GetValue(), championStats.physicalDamage.GetValue(), championStats.CurrentHealth, 
        championStats.CurrentMana, championStats.speed.GetValue(), championStats.magicResist.GetValue(), 
        championStats.armor.GetValue(), championStats.bonusAttackSpeed.GetValue(), championStats.maxHealth.GetValue(), championStats.maxMana.GetValue()));
    }

    [Test]
    public void does_not_add_any_stats_to_champion_stats_from_null_item(){
        // Arrange
        ScriptableChampion sChampion = ScriptableObject.CreateInstance<ScriptableChampion>();
        ChampionStats championStats = new ChampionStats(sChampion);
        championStats.maxHealth.BaseValue = 222f;
        championStats.CurrentHealth = championStats.maxHealth.GetValue() - 42f;
        championStats.maxMana.BaseValue = 10f;
        championStats.CurrentMana = championStats.maxMana.GetValue() - 1f;
        
        Item item = null;

        // Act
        championStats.AddItemStats(item);

        // Assert
        Assert.AreEqual((0f, 0f, 180f, 9f, 0f, 0f, 0f, 0f, 222f, 10f),
        (championStats.magicDamage.GetValue(), championStats.physicalDamage.GetValue(), championStats.CurrentHealth, 
        championStats.CurrentMana, championStats.speed.GetValue(), championStats.magicResist.GetValue(), 
        championStats.armor.GetValue(), championStats.bonusAttackSpeed.GetValue(), championStats.maxHealth.GetValue(), championStats.maxMana.GetValue()));
    }

    [Test]
    public void removes_item_stats_from_champion_stats_where_item_has_no_health_or_mana(){
        // Arrange
        ScriptableChampion sChampion = ScriptableObject.CreateInstance<ScriptableChampion>();
        ChampionStats championStats = new ChampionStats(sChampion);
        championStats.maxHealth.BaseValue = 988f;
        championStats.CurrentHealth = championStats.maxHealth.GetValue() - 100f;
        championStats.maxMana.BaseValue = 102f;
        championStats.CurrentMana = championStats.maxMana.GetValue() - 52f;
        
        Item item = ScriptableObject.CreateInstance<Item>();
        item.magicDamage = 50f;
        item.physicalDamage = 11f;
        item.speed = 1.4f;
        item.magicResist = 28f;
        item.armor = 16f;
        item.attackSpeed = 12f;
        championStats.AddItemStats(item);

        // Act
        championStats.RemoveItemStats(item);

        // Assert
        Assert.AreEqual((0f, 0f, 888f, 50f, 0f, 0f, 0f, 0f, 988f, 102f),
        (championStats.magicDamage.GetValue(), championStats.physicalDamage.GetValue(), championStats.CurrentHealth, 
        championStats.CurrentMana, championStats.speed.GetValue(), championStats.magicResist.GetValue(), 
        championStats.armor.GetValue(), championStats.bonusAttackSpeed.GetValue(), championStats.maxHealth.GetValue(), championStats.maxMana.GetValue()));
    }

    [Test]
    public void removes_item_stats_from_item_with_health_that_has_not_been_used(){
        // Arrange
        ScriptableChampion sChampion = ScriptableObject.CreateInstance<ScriptableChampion>();
        ChampionStats championStats = new ChampionStats(sChampion);
        championStats.maxHealth.BaseValue = 988f;
        
        Item item = ScriptableObject.CreateInstance<Item>();
        item.health = 252f;
        championStats.AddItemStats(item);

        championStats.CurrentHealth = championStats.maxHealth.GetValue() - 128f;


        // Act
        championStats.RemoveItemStats(item);

        // Assert
        Assert.AreEqual((988f, 988f), (championStats.CurrentHealth, championStats.maxHealth.GetValue()));
    }

    [Test]
    public void removes_item_stats_from_item_with_health_that_has_been_used(){
        // Arrange
        ScriptableChampion sChampion = ScriptableObject.CreateInstance<ScriptableChampion>();
        ChampionStats championStats = new ChampionStats(sChampion);
        championStats.maxHealth.BaseValue = 843f;
        
        Item item = ScriptableObject.CreateInstance<Item>();
        item.health = 106f;
        championStats.AddItemStats(item);

        championStats.CurrentHealth = championStats.maxHealth.GetValue() - 481f;


        // Act
        championStats.RemoveItemStats(item);

        // Assert
        Assert.AreEqual((468f, 843f), (championStats.CurrentHealth, championStats.maxHealth.GetValue()));
    }

    [Test]
    public void removes_item_stats_from_item_with_mana_that_has_not_been_fully_used(){
        // Arrange
        ScriptableChampion sChampion = ScriptableObject.CreateInstance<ScriptableChampion>();
        ChampionStats championStats = new ChampionStats(sChampion);
        championStats.maxMana.BaseValue = 1423f;
        
        Item item = ScriptableObject.CreateInstance<Item>();
        item.mana = 200f;
        championStats.AddItemStats(item);

        championStats.CurrentMana = championStats.maxMana.GetValue() - 61f;


        // Act
        championStats.RemoveItemStats(item);

        // Assert
        Assert.AreEqual((1423f, 1423f), (championStats.CurrentMana, championStats.maxMana.GetValue()));
    }

    [Test]
    public void removes_item_stats_from_item_with_mana_that_has_been_fully_used(){
        // Arrange
        ScriptableChampion sChampion = ScriptableObject.CreateInstance<ScriptableChampion>();
        ChampionStats championStats = new ChampionStats(sChampion);
        championStats.maxMana.BaseValue = 1235f;
        
        Item item = ScriptableObject.CreateInstance<Item>();
        item.mana = 170f;
        championStats.AddItemStats(item);

        championStats.CurrentMana = championStats.maxMana.GetValue() - 231f;


        // Act
        championStats.RemoveItemStats(item);

        // Assert
        Assert.AreEqual((1174f, 1235f), (championStats.CurrentMana, championStats.maxMana.GetValue()));
    }
}
