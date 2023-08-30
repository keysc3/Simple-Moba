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

    [Test]
    public void levels_up_spell_3_to_level_1(){
        // Arrange
        MockPlayer player = new MockPlayer();
        player.levelManager = new LevelManager(player);
        
        // Act
        player.levelManager.SpellLevelUpRequest("Spell_3");

        // Assert
        Assert.AreEqual(1, player.levelManager.spellLevels["Spell_3"]);
    }

    [Test]
    public void does_not_level_up_spell_1_past_max_level(){
        // Arrange
        MockPlayer player = new MockPlayer();
        int maxSpellLevel = levelInfo.maxSpellLevel;
        player.levelManager = new LevelManager(player, maxSpellLevel);
        
        // Act
        for(int i = 0; i < maxSpellLevel+1; i++){
            player.levelManager.SpellLevelUpRequest("Spell_1");
        }

        // Assert
        Assert.AreEqual(maxSpellLevel, player.levelManager.spellLevels["Spell_1"]);
    }

    [Test]
    public void does_not_level_up_spell_2_to_level_1_from_insufficient_skill_points(){
        // Arrange
        MockPlayer player = new MockPlayer();
        player.levelManager = new LevelManager(player);
        player.levelManager.SpellLevelUpRequest("Spell_3");

        // Act
        player.levelManager.SpellLevelUpRequest("Spell_2");

        // Assert
        Assert.AreEqual(0, player.levelManager.spellLevels["Spell_2"]);
    }

    [Test]
    public void does_not_level_spell_4_to_level_1_from_invalid_level_requirements(){
        // Arrange
        MockPlayer player = new MockPlayer();
        player.levelManager = new LevelManager(player, 4);

        // Act
        player.levelManager.SpellLevelUpRequest("Spell_4");

        // Assert
        Assert.AreEqual(0, player.levelManager.spellLevels["Spell_4"]);
    }

    [Test]
    public void levels_spell_4_to_level_1(){
        // Arrange
        MockPlayer player = new MockPlayer();
        player.levelManager = new LevelManager(player, 6);

        // Act
        player.levelManager.SpellLevelUpRequest("Spell_4");

        // Assert
        Assert.AreEqual(1, player.levelManager.spellLevels["Spell_4"]);
    }

    [Test]
    public void does_not_level_spell_4_to_level_2_from_invalid_level_requirements(){
        // Arrange
        MockPlayer player = new MockPlayer();
        player.levelManager = new LevelManager(player, 10);

        // Act
        player.levelManager.SpellLevelUpRequest("Spell_4");
        player.levelManager.SpellLevelUpRequest("Spell_4");

        // Assert
        Assert.AreEqual(1, player.levelManager.spellLevels["Spell_4"]);
    }

    [Test]
    public void levels_spell_4_to_level_2(){
        // Arrange
        MockPlayer player = new MockPlayer();
        player.levelManager = new LevelManager(player, 11);

        // Act
        player.levelManager.SpellLevelUpRequest("Spell_4");
        player.levelManager.SpellLevelUpRequest("Spell_4");

        // Assert
        Assert.AreEqual(2, player.levelManager.spellLevels["Spell_4"]);
    }

    [Test]
    public void does_not_level_spell_4_to_level_3_from_invalid_level_requirements(){
        // Arrange
        MockPlayer player = new MockPlayer();
        player.levelManager = new LevelManager(player, 15);

        // Act
        for(int i = 0; i < 3; i++){
            player.levelManager.SpellLevelUpRequest("Spell_4");
        }

        // Assert
        Assert.AreEqual(2, player.levelManager.spellLevels["Spell_4"]);
    }

    [Test]
    public void levels_spell_4_to_level_3(){
        // Arrange
        MockPlayer player = new MockPlayer();
        player.levelManager = new LevelManager(player, 16);

        // Act
        for(int i = 0; i < 3; i++){
            player.levelManager.SpellLevelUpRequest("Spell_4");
        }

        // Assert
        Assert.AreEqual(3, player.levelManager.spellLevels["Spell_4"]);
    }

    [Test]
    public void does_not_level_up_spell_4_past_max_level(){
        // Arrange
        MockPlayer player = new MockPlayer();
        int maxUltLevel = levelInfo.maxUltLevel;
        player.levelManager = new LevelManager(player, 17);
        
        // Act
        for(int i = 0; i < maxUltLevel+1; i++){
            player.levelManager.SpellLevelUpRequest("Spell_4");
        }

        // Assert
        Assert.AreEqual(maxUltLevel, player.levelManager.spellLevels["Spell_4"]);
    }

    [Test]
    public void gets_respawn_time_from_player_under_level_7(){
        // Arrange
        MockPlayer player = new MockPlayer();
        player.levelManager = new LevelManager(player, 4);

        // Act
        float respawnTime = player.levelManager.RespawnTime();

        // Assert
        Assert.AreEqual(12f, respawnTime);
    }

    [Test]
    public void gets_respawn_time_from_player_at_level_7(){
        // Arrange
        MockPlayer player = new MockPlayer();
        player.levelManager = new LevelManager(player, 7);

        // Act
        float respawnTime = player.levelManager.RespawnTime();

        // Assert
        Assert.AreEqual(21f, respawnTime);
    }

    [Test]
    public void gets_respawn_time_from_player_above_level_7(){
        // Arrange
        MockPlayer player = new MockPlayer();
        player.levelManager = new LevelManager(player, 15);

        // Act
        float respawnTime = player.levelManager.RespawnTime();

        // Assert
        Assert.AreEqual(45f, respawnTime);
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
