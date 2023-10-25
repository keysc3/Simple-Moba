using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;

/*
* Purpose: Tests for the DamageCalculator class.
*
* @author: Colin Keys
*/
public class damage_calculator : ScriptableObject
{
    /*
    *   Sets the magic damage to deal to something from 80 incoming damage with 23 magic resist. Should be 65.
    */
    [Test]
    public void sets_magic_damage_taken_to_65_from_80_with_23_magic_resist(){
        // Arrange
        ScriptableUnit unit1 = ScriptableObject.CreateInstance<ScriptableUnit>();
        ScriptableUnit unit2 = ScriptableObject.CreateInstance<ScriptableUnit>();
        UnitStats from = new UnitStats(unit1);
        UnitStats to = new UnitStats(unit2);
        float magicResist = 23f;
        to.magicResist.BaseValue = magicResist;
        float incomingDamage = 80f;
        DamageType damageType = DamageType.Magic;

        // Act
        float finalDamage = DamageCalculator.CalculateDamage(incomingDamage, damageType, from, to);
        finalDamage = Mathf.Round(finalDamage);

        // Assert
        Assert.AreEqual(65f, finalDamage);
    }

    /*
    *   Sets the physical damage to deal to something from 121 incoming damage with 72 magic resist. Should be 70.
    */
    [Test]
    public void sets_physical_damage_taken_to_70_from_121_with_72_armor(){
        // Arrange
        ScriptableUnit unit1 = ScriptableObject.CreateInstance<ScriptableUnit>();
        ScriptableUnit unit2 = ScriptableObject.CreateInstance<ScriptableUnit>();
        UnitStats from = new UnitStats(unit1);
        UnitStats to = new UnitStats(unit2);
        float armor = 72f;
        to.armor.BaseValue = armor;
        float incomingDamage = 121f;
        DamageType damageType = DamageType.Physical;

        // Act
        float finalDamage = DamageCalculator.CalculateDamage(incomingDamage, damageType, from, to);
        finalDamage = Mathf.Round(finalDamage);

        // Assert
        Assert.AreEqual(70f, finalDamage);
    }

    /*
    *   Sets the true damage to deal to something from 81 incoming damage. Should be 81.
    */
    [Test]
    public void sets_true_damage_to_81_from_81(){
        // Arrange
        ScriptableUnit unit1 = ScriptableObject.CreateInstance<ScriptableUnit>();
        ScriptableUnit unit2 = ScriptableObject.CreateInstance<ScriptableUnit>();
        UnitStats from = new UnitStats(unit1);
        UnitStats to = new UnitStats(unit2);
        float armor = 72f;
        to.armor.BaseValue = armor;
        float magicResist = 23f;
        to.magicResist.BaseValue = magicResist;
        DamageType damageType = DamageType.True;
        float incomingDamage = 81f;

        // Act
        float finalDamage = DamageCalculator.CalculateDamage(incomingDamage, damageType, from, to);
        finalDamage = Mathf.Round(finalDamage);

        // Assert
        Assert.AreEqual(81f, finalDamage);
    }

    /*
    *   Sets the damage to deal to something from a negative incoming damage value. Should be 0.
    */
    [Test]
    public void sets_damage_to_0_from_negative_damage(){
        // Arrange
        ScriptableUnit unit1 = ScriptableObject.CreateInstance<ScriptableUnit>();
        ScriptableUnit unit2 = ScriptableObject.CreateInstance<ScriptableUnit>();
        UnitStats from = new UnitStats(unit1);
        UnitStats to = new UnitStats(unit2);
        DamageType damageType = DamageType.True;
        float incomingDamage = -(2f);

        // Act
        float finalDamage = DamageCalculator.CalculateDamage(incomingDamage, damageType, from, to);

        // Assert
        Assert.AreEqual(0f, finalDamage);
    }
}
