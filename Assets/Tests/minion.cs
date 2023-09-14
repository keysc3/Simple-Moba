using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

public class minion
{
    // A Test behaves as an ordinary method
    [Test]
    public void sets_minion_health_to_5_from_taking_15_non_dot_magic_damage_at_20_current_health()
    {
        // Arrange
        IMinion minion = CreateMinionWithUnitStats(50f);
        IPlayer damager = CreateDamager();
        minion.unitStats.CurrentHealth = 20f;

        // Act
        minion.TakeDamage(15f, "magic", damager, false);

        // Assert
        Assert.AreEqual(5f, minion.unitStats.CurrentHealth);
    }

    [Test]
    public void minion_death_from_taking_20_non_dot_true_damage_at_5_current_health(){
        // Arrange
        IMinion minion = CreateMinionWithUnitStats(50f);
        IPlayer damager = CreateDamager();
        minion.unitStats.CurrentHealth = 5f;
        LogAssert.Expect(LogType.Error, "Destroy may not be called from edit mode! Use DestroyImmediate instead.\nDestroying an object in edit mode destroys it permanently.");

        // Act
        minion.TakeDamage(20f, "true", damager, false);

        // Assert
        Assert.True(minion.IsDead);
    }

    [Test]
    public void minion_taking_lethal_damage_updates_damager_creep_score(){
        // Arrange
        IMinion minion = CreateMinionWithUnitStats(50f);
        IPlayer damager = CreateDamager();
        minion.unitStats.CurrentHealth = 5f;
        damager.score = new Score(null);
        LogAssert.Expect(LogType.Error, "Destroy may not be called from edit mode! Use DestroyImmediate instead.\nDestroying an object in edit mode destroys it permanently.");

        // Act
        minion.TakeDamage(143f, "physical", damager, false);

        // Assert
        Assert.AreEqual(1, damager.score.CS);
    }

    private IMinion CreateMinionWithUnitStats(float maxHealth){
        GameObject g1 = new GameObject();
        g1.SetActive(false);
        IMinion minion = g1.AddComponent<Minion>();
        UnitStats unitStats = new MinionStats(ScriptableObject.CreateInstance<ScriptableMinion>());
        unitStats.maxHealth.BaseValue = maxHealth;
        unitStats.CurrentHealth = unitStats.maxHealth.GetValue();
        minion.unitStats = unitStats;
        return minion;
    }

    private IPlayer CreateDamager(){
        IPlayer damager = Substitute.For<IPlayer>();
        UnitStats damagerStats = new ChampionStats(ScriptableObject.CreateInstance<ScriptableChampion>());
        damager.unitStats = damagerStats;
        return damager;
    }
}
