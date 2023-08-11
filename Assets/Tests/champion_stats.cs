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

}
