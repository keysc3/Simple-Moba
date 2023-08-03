using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;

public class damage_calculator : ScriptableObject
{
    public ScriptableUnit unit1;
    public ScriptableUnit unit2;

    // A Test behaves as an ordinary method
    [Test]
    public void sets_magic_damage_to_65_from_80_with_23_magic_resist()
    {
        // Arrange
        float magicResist = 23f;
        UnitStats from = new UnitStats(unit1);
        UnitStats to = new UnitStats(unit2);
        to.magicResist.BaseValue = magicResist;
        float incomingDamage = 80f;
        string damageType = "magic";

        // Act
        float finalDamage = DamageCalculator.CalculateDamage(incomingDamage, damageType, from, to);
        finalDamage = Mathf.Round(finalDamage);

        // Assert
        Assert.AreEqual(65f, finalDamage);
    }

    [Test]
    public void sets_physical_damage_to_70_from_121_with_72_armor()
    {
        // Arrange
        float armor = 72f;
        UnitStats from = new UnitStats(unit1);
        UnitStats to = new UnitStats(unit2);
        to.armor.BaseValue = armor;
        float incomingDamage = 121f;
        string damageType = "physical";

        // Act
        float finalDamage = DamageCalculator.CalculateDamage(incomingDamage, damageType, from, to);
        finalDamage = Mathf.Round(finalDamage);

        // Assert
        Assert.AreEqual(70f, finalDamage);
    }

    [Test]
    public void sets_true_damage_to_81_from_81()
    {
        // Arrange
        UnitStats from = new UnitStats(unit1);
        UnitStats to = new UnitStats(unit2);
        string damageType = "true";
        float incomingDamage = 81f;

        // Act
        float finalDamage = DamageCalculator.CalculateDamage(incomingDamage, damageType, from, to);
        finalDamage = Mathf.Round(finalDamage);

        // Assert
        Assert.AreEqual(81f, finalDamage);
    }

    [Test]
    public void sets_damage_to_0_from_negative_damage()
    {
        // Arrange
        UnitStats from = new UnitStats(unit1);
        UnitStats to = new UnitStats(unit2);
        string damageType = "true";
        float incomingDamage = -(2f);

        // Act
        float finalDamage = DamageCalculator.CalculateDamage(incomingDamage, damageType, from, to);

        // Assert
        Assert.AreEqual(0f, finalDamage);
    }

    [Test]
    public void sets_damage_to_0_from_invalid_damage_type()
    {
        // Arrange
        UnitStats from = new UnitStats(unit1);
        UnitStats to = new UnitStats(unit2);
        string damageType = "goofball";
        float incomingDamage = 50f;

        // Act
        float finalDamage = DamageCalculator.CalculateDamage(incomingDamage, damageType, from, to);

        // Assert
        Assert.AreEqual(0f, finalDamage);
    }
}
