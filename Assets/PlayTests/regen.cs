using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class regen
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator does_not_regen_health_at_max_health()
    {
        GameObject regenObject =  new GameObject("Health Regen!");
        HealthRegen script = regenObject.AddComponent<HealthRegen>();
        MockUnitBehaviour unit = regenObject.AddComponent<MockUnitBehaviour>();
        UnitStats unitStats = new UnitStats(ScriptableObject.CreateInstance<ScriptableUnit>());
        unitStats.HP5.BaseValue = 10.0f;
        unitStats.maxHealth.BaseValue = 100.0f;
        unitStats.CurrentHealth = unitStats.maxHealth.GetValue();
        unit.unitStats = unitStats;
        yield return null;
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(100f, unit.unitStats.CurrentHealth);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator sets_health_to_max_health_from_overflow_regen()
    {
        GameObject regenObject =  new GameObject("Health Regen!");
        HealthRegen script = regenObject.AddComponent<HealthRegen>();
        MockUnitBehaviour unit = regenObject.AddComponent<MockUnitBehaviour>();
        UnitStats unitStats = new UnitStats(ScriptableObject.CreateInstance<ScriptableUnit>());
        unitStats.HP5.BaseValue = 50.0f;
        unitStats.maxHealth.BaseValue = 100.0f;
        unitStats.CurrentHealth = 99f;
        unit.unitStats = unitStats;
        yield return null;
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(100f, unit.unitStats.CurrentHealth);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator does_not_regen_health_while_dead()
    {
        GameObject regenObject =  new GameObject("Health Regen!");
        HealthRegen script = regenObject.AddComponent<HealthRegen>();
        MockUnitBehaviour unit = regenObject.AddComponent<MockUnitBehaviour>();
        UnitStats unitStats = new UnitStats(ScriptableObject.CreateInstance<ScriptableUnit>());
        unitStats.HP5.BaseValue = 10.0f;
        unitStats.maxHealth.BaseValue = 100.0f;
        unitStats.CurrentHealth = 50f;
        unit.unitStats = unitStats;
        unit.IsDead = true;
        yield return null;
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(50f, unit.unitStats.CurrentHealth);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator regens_from_50_health_to_51_with_10_HP5()
    {
        GameObject regenObject =  new GameObject("Health Regen!");
        HealthRegen script = regenObject.AddComponent<HealthRegen>();
        MockUnitBehaviour unit = regenObject.AddComponent<MockUnitBehaviour>();
        UnitStats unitStats = new UnitStats(ScriptableObject.CreateInstance<ScriptableUnit>());
        unitStats.HP5.BaseValue = 10.0f;
        unitStats.maxHealth.BaseValue = 100.0f;
        unitStats.CurrentHealth = 50f;
        unit.unitStats = unitStats;
        yield return null;
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(51f, unit.unitStats.CurrentHealth);
    }
}