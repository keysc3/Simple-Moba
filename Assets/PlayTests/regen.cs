using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class regen
{
    // A Test behaves as an ordinary method
    [Test]
    public void regenSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator does_not_regen_health_at_max_health()
    {
        GameObject regenObject =  new GameObject("Health Regen!");
        HealthRegen script = regenObject.AddComponent<HealthRegen>();
        MockUnit unit = new MockUnit();
        UnitStats unitStats = new UnitStats(ScriptableObject.CreateInstance<ScriptableUnit>());
        unitStats.HP5.BaseValue = 10.0f;
        unitStats.maxHealth.BaseValue = 100.0f;
        unitStats.CurrentHealth = unitStats.maxHealth.GetValue();
        unit.unitStats = unitStats;
        script.Unit = unit;
        yield return null;
        Assert.AreEqual(100f, unit.unitStats.CurrentHealth);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator sets_health_to_max_health_from_overflow_regen()
    {
        GameObject regenObject =  new GameObject("Health Regen!");
        HealthRegen script = regenObject.AddComponent<HealthRegen>();
        MockUnit unit = new MockUnit();
        UnitStats unitStats = new UnitStats(ScriptableObject.CreateInstance<ScriptableUnit>());
        unitStats.HP5.BaseValue = 50.0f;
        unitStats.maxHealth.BaseValue = 100.0f;
        unitStats.CurrentHealth = 99f;
        unit.unitStats = unitStats;
        script.Unit = unit;
        yield return null;
        Assert.AreEqual(100f, unit.unitStats.CurrentHealth);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator does_not_regen_health_while_dead()
    {
        GameObject regenObject =  new GameObject("Health Regen!");
        HealthRegen script = regenObject.AddComponent<HealthRegen>();
        MockUnit unit = new MockUnit();
        UnitStats unitStats = new UnitStats(ScriptableObject.CreateInstance<ScriptableUnit>());
        unitStats.HP5.BaseValue = 10.0f;
        unitStats.maxHealth.BaseValue = 100.0f;
        unitStats.CurrentHealth = 50f;
        unit.unitStats = unitStats;
        unit.IsDead = true;
        script.Unit = unit;
        yield return null;
        Assert.AreEqual(50f, unit.unitStats.CurrentHealth);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator regens_from_50_health_to_51_with_10_HP5()
    {
        GameObject regenObject =  new GameObject("Health Regen!");
        HealthRegen script = regenObject.AddComponent<HealthRegen>();
        MockUnit unit = new MockUnit();
        UnitStats unitStats = new UnitStats(ScriptableObject.CreateInstance<ScriptableUnit>());
        unitStats.HP5.BaseValue = 10.0f;
        unitStats.maxHealth.BaseValue = 100.0f;
        unitStats.CurrentHealth = 50f;
        unit.unitStats = unitStats;
        script.Unit = unit;
        yield return null;
        Assert.AreEqual(51f, unit.unitStats.CurrentHealth);
    }
}


public class MockUnit : IUnit{
    public ScriptableUnit SUnit { get; set; }
    public UnitStats unitStats { get; set; }
    public StatusEffects statusEffects { get; set; }
    public DamageTracker damageTracker { get; set; }
    public Inventory inventory { get; set; }
    public bool IsDead { get; set; }
    public BonusDamage bonusDamage { get; set; }
    public GameObject GameObject { get; }
    public Vector3 Position { get; set; }

    public void TakeDamage(float damageAmount, string damageType, IUnit damager, bool isDot){}
}
