using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class unit_stats
{
    // A Test behaves as an ordinary method
    [Test]
    public void unit_statsSimplePasses()
    {
        // Arrange
        ScriptableUnit sUnit = ScriptableObject.CreateInstance<ScriptableUnit>();
        UnitStats unitStats = new UnitStats(sUnit);

        unitStats.maxHealth.BaseValue = 100f;


    }
}
