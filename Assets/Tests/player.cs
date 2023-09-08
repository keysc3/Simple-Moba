using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

public class player
{
    // A Test behaves as an ordinary method
    [Test]
    public void sets_player_health_to_90_from_taking_10_non_dot_true_damage_at_100_current_health()
    {
        // Arrange
        IPlayer player = CreatePlayerWithUnitStats(100f);
        IPlayer damager = CreateDamager();

        // Act
        player.TakeDamage(10f, "true", damager, false);

        // Assert
        Assert.AreEqual(90f, player.unitStats.CurrentHealth);
    }

    [Test]
    public void sets_player_health_to_138_from_taking_14_damage_and_112_bonus_damage_at_264_current_health(){
        // Arrange
        IPlayer player = CreatePlayerWithUnitStats(264f);
        IPlayer damager = CreateDamager();
        void damageMethod(IUnit unit, bool isDot){ 
            unit.bonusDamage -= damageMethod;
            unit.TakeDamage(112f, "magic", damager, isDot); 
        }
        player.bonusDamage += damageMethod;

        // Act
        player.TakeDamage(14f, "magic", damager, false);

        // Assert
        Assert.AreEqual(138f, player.unitStats.CurrentHealth);
    }

    [Test]
    public void sets_player_health_to_0_from_taking_321_non_dot_true_damage_at_1_current_health(){
        // Arrange
        IPlayer player = CreatePlayerWithUnitStats(350f);
        IPlayer damager = CreateDamager();
        player.unitStats.CurrentHealth = 1f;

        // Act
        player.TakeDamage(321f, "true", damager, false);

        // Assert
        Assert.AreEqual(0, player.unitStats.CurrentHealth);
    }

    [Test]
    public void player_death_from_taking_143_non_dot_physical_damage_at_20_current_health(){
        // Arrange
        IPlayer player = CreatePlayerWithUnitStats(150f);
        IPlayer damager = CreateDamager();
        player.unitStats.CurrentHealth = 20f;

        // Act
        player.TakeDamage(143f, "physical", damager, false);

        // Assert
        Assert.True(player.IsDead);
    }

    [Test]
    public void player_taking_lethal_damage_updates_damager_score(){
        // Arrange
        IPlayer player = CreatePlayerWithUnitStats(150f);
        IPlayer damager = CreateDamager();
        damager.score = new Score(null);
        player.unitStats.CurrentHealth = 20f;

        // Act
        player.TakeDamage(143f, "physical", damager, false);

        // Assert
        Assert.AreEqual(1, damager.score.Kills);
    }

    [Test]
    public void player_taking_lethal_damage_updates_damager_and_assisters_score(){
        // Arrange
        IPlayer player = CreatePlayerWithUnitStats(150f);
        IPlayer damager1 = CreateDamager();
        damager1.score = new Score(null);
        IPlayer damager2 = CreateDamager();
        damager2.score = new Score(null);
        player.damageTracker = new DamageTracker();
        player.unitStats.CurrentHealth = 150f;

        // Act
        player.TakeDamage(143f, "physical", damager1, false);
        player.TakeDamage(37f, "physical", damager2, false);

        // Assert
        Assert.AreEqual((1, 0, 0, 1), (damager2.score.Kills, damager2.score.Assists, damager1.score.Kills, damager1.score.Assists));
    }

    private IPlayer CreatePlayerWithUnitStats(float maxHealth){
        GameObject g1 = new GameObject();
        g1.SetActive(false);
        IPlayer player = g1.AddComponent<Player>();
        UnitStats unitStats = new ChampionStats(ScriptableObject.CreateInstance<ScriptableChampion>());
        unitStats.maxHealth.BaseValue = maxHealth;
        unitStats.CurrentHealth = unitStats.maxHealth.GetValue();
        player.unitStats = unitStats;
        return player;
    }

    private IPlayer CreateDamager(){
        IPlayer damager = Substitute.For<IPlayer>();
        UnitStats damagerStats = new ChampionStats(ScriptableObject.CreateInstance<ScriptableChampion>());
        damager.unitStats = damagerStats;
        return damager;
    }
}
