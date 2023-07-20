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
    public void sets_magicdamage_to_65_from_80_with_23_magicResist()
    {
        // Arrange
        float magicResist = 23f;
        UnitStats from = new UnitStats(unit1);
        UnitStats to = new UnitStats(unit2);
        to.magicResist.SetBaseValue(magicResist);
        float incomingDamage = 80f;
        string damageType = "magic";

        // Act
        float finalDamage = DamageCalculator.CalculateDamage(incomingDamage, damageType, from, to);
        finalDamage = Mathf.Round(finalDamage);

        // Assert
        Assert.AreEqual(65f, finalDamage);
    }

    [Test]
    public void sets_physicaldamage_to_70_from_121_with_72_armor()
    {
        // Arrange
        float armor = 72f;
        UnitStats from = new UnitStats(unit1);
        UnitStats to = new UnitStats(unit2);
        to.armor.SetBaseValue(armor);
        float incomingDamage = 121f;
        string damageType = "physical";

        // Act
        float finalDamage = DamageCalculator.CalculateDamage(incomingDamage, damageType, from, to);
        finalDamage = Mathf.Round(finalDamage);

        // Assert
        Assert.AreEqual(70f, finalDamage);
    }

    [Test]
    public void sets_truedamage_to_81_from_81()
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
}
