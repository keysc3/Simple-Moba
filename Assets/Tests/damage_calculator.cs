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
    public void sets_damage_to_65_from_80_with_23__magicResist()
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

}
