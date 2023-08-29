using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;

public class level_manager
{
    private LevelInfo levelInfo = ScriptableObject.CreateInstance<LevelInfo>();

    // A Test behaves as an ordinary method
    [Test]
    public void gain_xp_from_player_kill()
    {
        // Arrange
        MockPlayer player = new MockPlayer();
        MockPlayer enemy = new MockPlayer();
        player.levelManager = new LevelManager(player);
        enemy.levelManager = new LevelManager(enemy);

        // Act
        player.levelManager.GainXP(enemy);

        // Assert
        Assert.AreEqual((1, levelInfo.championKillXP[0]), (player.levelManager.Level, player.levelManager.CurrentXP));
    }

    [Test]
    public void gain_xp_from_non_player_unit_kill()
    {
        // Arrange
        MockPlayer player = new MockPlayer();
        MockUnit unit = new MockUnit();
        player.levelManager = new LevelManager(player);

        // Act
        player.levelManager.GainXP(unit);

        // Assert
        Assert.AreEqual((1, levelInfo.defaultXP), (player.levelManager.Level, player.levelManager.CurrentXP));
    }

    [Test]
    public void level_up_from_level_one_to_three_without_unit_stats()
    {
        // Arrange
        MockPlayer player = new MockPlayer();
        MockPlayer enemy = new MockPlayer();
        player.levelManager = new LevelManager(player);
        enemy.levelManager = new LevelManager(enemy);
        float totalXP = 0f;
        float totalNeeded = levelInfo.requiredXP[1] + levelInfo.requiredXP[2];
        int iterations = 0;

        // Act
        while(totalXP < totalNeeded){
            player.levelManager.GainXP(enemy);
            totalXP += levelInfo.championKillXP[0];
            iterations++;
        }

        // Assert
        float currentXP = (levelInfo.championKillXP[0] * iterations) - totalNeeded;
        Assert.AreEqual((3, currentXP), (player.levelManager.Level, player.levelManager.CurrentXP));
    }

    [Test]
    public void update_unit_stats_from_leveling_from_one_to_four()
    {
        // Arrange
        MockPlayer player = new MockPlayer();
        ((ScriptableChampion) player.SUnit).healthGrowth = 63f;
        ((ScriptableChampion) player.SUnit).manaGrowth = 21f;
        ((ScriptableChampion) player.SUnit).physicalDamageGrowth = 5.2f;
        ((ScriptableChampion) player.SUnit).attackSpeedGrowth = 1.8f;
        player.levelManager = new LevelManager(player);
        player.unitStats = new ChampionStats((ScriptableChampion) player.SUnit);
        player.unitStats.maxHealth.BaseValue = 100f;
        player.unitStats.CurrentHealth = 60f;
        ((ChampionStats) player.unitStats).maxMana.BaseValue = 50f;
        ((ChampionStats) player.unitStats).CurrentMana = 26f;
        player.unitStats.physicalDamage.BaseValue = 39f;

        MockPlayer enemy = new MockPlayer();
        enemy.levelManager = new LevelManager(enemy);

        float totalXP = 0f;
        float totalNeeded = levelInfo.requiredXP[1] + levelInfo.requiredXP[2] + levelInfo.requiredXP[3];
        
        // Act
        while(totalXP < totalNeeded){
            player.levelManager.GainXP(enemy);
            totalXP += levelInfo.championKillXP[0];
        }
        
        // Assert
        List<float> expected = new List<float>(){242.695f, 202.695f, 97.565f, 73.565f, 50.778f, 4.077f};
        List<float> actual = new List<float>(){player.unitStats.maxHealth.BaseValue, player.unitStats.CurrentHealth, 
        ((ChampionStats) player.unitStats).maxMana.BaseValue, ((ChampionStats) player.unitStats).CurrentMana, player.unitStats.physicalDamage.BaseValue, 
        player.unitStats.bonusAttackSpeed.BaseValue};
        Assert.IsTrue(expected.SequenceEqual(actual, new FloatComparer()));
    }
}

public class FloatComparer : IEqualityComparer<float>{

    public bool Equals(float f1, float f2){
        if(Mathf.Abs(f1 - f2) <= 0.001f)
            return true;
        else
            return false;
    }

    public int GetHashCode(float f){
        return f.GetHashCode();
    }
}
