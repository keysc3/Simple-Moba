using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class unit_stats
{

    // A Test behaves as an ordinary method
    [Test]
    public void sets_current_health_to_62_with_100_max_health(){
        // Arrange
        ScriptableUnit sUnit = ScriptableObject.CreateInstance<ScriptableUnit>();
        UnitStats unitStats = new UnitStats(sUnit);
        unitStats.maxHealth.BaseValue = 100f;

        // Act
        unitStats.CurrentHealth = 62f;

        // Assert 
        Assert.AreEqual(62f, unitStats.CurrentHealth);

    }

    // A Test behaves as an ordinary method
    [Test]
    public void sets_current_health_to_max_health_given_value_greater_than_max_health(){
        // Arrange
        ScriptableUnit sUnit = ScriptableObject.CreateInstance<ScriptableUnit>();
        UnitStats unitStats = new UnitStats(sUnit);
        unitStats.maxHealth.BaseValue = 100f;
        unitStats.CurrentHealth = 42f;

        // Act
        unitStats.CurrentHealth = 162f;

        // Assert 
        Assert.AreEqual(100f, unitStats.CurrentHealth);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void sets_current_health_to_72_with_max_health_100_with_base_50_and_modifier_50(){
        // Arrange
        ScriptableUnit sUnit = ScriptableObject.CreateInstance<ScriptableUnit>();
        UnitStats unitStats = new UnitStats(sUnit);
        unitStats.maxHealth.BaseValue = 50f;
        unitStats.maxHealth.AddModifier(50f);

        // Act
        unitStats.CurrentHealth = 72f;

        // Assert 
        Assert.AreEqual(72f, unitStats.CurrentHealth);
    }


}