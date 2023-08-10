using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class stat
{
    // A Test behaves as an ordinary method
    [Test]
    public void initializes_stat_with_82_base_value()
    {
        // Arrange
        float value = 82f;
        
        // Act
        Stat stat = new Stat(value);

        // Assert
        Assert.AreEqual(82f, stat.BaseValue);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void does_not_set_stat_base_value_given_negative_value()
    {
        // Arrange
        float value = -1f;
        Stat stat = new Stat(100f);
        
        // Act
        stat.BaseValue = value;

        // Assert
        Assert.AreEqual(100f, stat.BaseValue);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void gets_stat_value_without_any_modifiers()
    {
        // Arrange
        float value = 21f;
        Stat stat = new Stat(value);
        
        // Act
        float statValue = stat.GetValue();

        // Assert
        Assert.AreEqual(21f, statValue);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void gets_stat_value_with_modifiers_value()
    {
        // Arrange
        float value = 1014f;
        Stat stat = new Stat(value);
        stat.AddModifier(0f);
        stat.AddModifier(320f);
        stat.AddModifier(-20f);
        stat.AddModifier(953f);
        stat.AddModifier(108f);
        stat.RemoveModifier(320f);
        
        // Act
        float statValue = stat.GetValue();

        // Assert
        Assert.AreEqual(2055f, statValue);
    }
    
    // A Test behaves as an ordinary method
    [Test]
    public void clears_every_modifier_from_stats_modifier_list()
    {
        // Arrange
        float value = 1014f;
        Stat stat = new Stat(value);
        stat.AddModifier(320f);
        stat.AddModifier(-20f);
        stat.AddModifier(953f);
        stat.AddModifier(108f);
        stat.RemoveModifier(320f);
        
        // Act
        stat.ClearModifiers();

        // Assert
        Assert.AreEqual(value, stat.GetValue());
    }
}
