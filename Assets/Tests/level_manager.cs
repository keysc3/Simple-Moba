using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;
using NSubstitute;

public class level_manager
{
    private LevelInfo levelInfo = ScriptableObject.CreateInstance<LevelInfo>();

    // A Test behaves as an ordinary method
    [Test]
    public void gain_xp_from_player_kill()
    {
        // Arrange
        IPlayer player = CreateMockPlayerWithLevelManager(1);
        IPlayer enemy = CreateMockPlayerWithLevelManager(1);

        // Act
        player.levelManager.GainXP(enemy);

        // Assert
        Assert.AreEqual((1, levelInfo.championKillXP[0]), (player.levelManager.Level, player.levelManager.CurrentXP));
    }

    [Test]
    public void gain_xp_from_non_player_unit_kill()
    {
        // Arrange
        IPlayer player = CreateMockPlayerWithLevelManager(1);
        IUnit unit = Substitute.For<IUnit>();
        player.levelManager = new LevelManager(player);

        // Act
        player.levelManager.GainXP(unit);

        // Assert
        Assert.AreEqual((1, levelInfo.defaultXP), (player.levelManager.Level, player.levelManager.CurrentXP));
    }

    [Test]
    public void validates_overflow_xp_from_level_up()
    {
        // Arrange
        IPlayer player = CreateMockPlayerWithLevelManager(1);
        IPlayer enemy = CreateMockPlayerWithLevelManager(1);
        player.levelManager = new LevelManager(player);
        enemy.levelManager = new LevelManager(enemy);
        float totalXP = 0f;
        float totalNeeded = levelInfo.requiredXP[1];
        int iterations = 0;

        // Act
        while(totalXP < totalNeeded){
            player.levelManager.GainXP(enemy);
            totalXP += levelInfo.championKillXP[enemy.levelManager.Level - 1];
            iterations++;
        }

        // Assert
        float currentXP = (levelInfo.championKillXP[0] * iterations) - totalNeeded;
        Assert.AreEqual(currentXP, player.levelManager.CurrentXP);
    }

    [Test]
    public void level_up_from_level_one_to_three_without_unit_stats()
    {
        // Arrange
        IPlayer player = CreateMockPlayerWithLevelManager(1);
        IPlayer enemy = CreateMockPlayerWithLevelManager(1);
        player.levelManager = new LevelManager(player);
        enemy.levelManager = new LevelManager(enemy);

        // Act
        while(player.levelManager.Level < 3){
            player.levelManager.LevelUp();
        }

        // Assert
        Assert.AreEqual((3, 0f), (player.levelManager.Level, player.levelManager.CurrentXP));
    }

    [Test]
    public void update_unit_stats_from_leveling_from_one_to_four()
    {
        // Arrange
        IPlayer player = CreateMockPlayerWithLevelManager(1);
        ScriptableChampion champ = ScriptableObject.CreateInstance<ScriptableChampion>();
        champ.healthGrowth = 63f;
        champ.manaGrowth = 21f;
        champ.physicalDamageGrowth = 5.2f;
        champ.attackSpeedGrowth = 1.8f;
        player.SUnit.Returns(champ);
        ChampionStats unitStats = new ChampionStats(champ);
        unitStats.maxHealth.BaseValue = 100f;
        unitStats.CurrentHealth = 60f;
        unitStats.maxMana.BaseValue = 50f;
        unitStats.CurrentMana = 26f;
        unitStats.physicalDamage.BaseValue = 39f;
        player.unitStats.Returns(unitStats);
        
        // Act
        while(player.levelManager.Level < 4){
            player.levelManager.LevelUp();
        }
        
        // Assert
        List<float> expected = new List<float>(){242.695f, 202.695f, 97.565f, 73.565f, 50.778f, 4.077f};
        List<float> actual = new List<float>(){unitStats.maxHealth.BaseValue, unitStats.CurrentHealth, 
        unitStats.maxMana.BaseValue, unitStats.CurrentMana, unitStats.physicalDamage.BaseValue, 
        unitStats.bonusAttackSpeed.BaseValue};
        Assert.IsTrue(expected.SequenceEqual(actual, new FloatComparer()));
    }

    [Test]
    public void levels_up_spell_3_to_level_1(){
        // Arrange
        IPlayer player = CreateMockPlayerWithLevelManager(1);
        
        // Act
        player.levelManager.SpellLevelUpRequest("Spell_3");

        // Assert
        Assert.AreEqual(1, player.levelManager.spellLevels["Spell_3"]);
    }

    [Test]
    public void does_not_level_up_spell_1_past_max_level(){
        // Arrange
        int maxSpellLevel = levelInfo.maxSpellLevel;
        IPlayer player = CreateMockPlayerWithLevelManager(maxSpellLevel);
        
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
        IPlayer player = CreateMockPlayerWithLevelManager(1);
        player.levelManager.SpellLevelUpRequest("Spell_3");

        // Act
        player.levelManager.SpellLevelUpRequest("Spell_2");

        // Assert
        Assert.AreEqual(0, player.levelManager.spellLevels["Spell_2"]);
    }

    [Test]
    public void does_not_level_spell_4_to_level_1_from_invalid_level_requirements(){
        // Arrange
        IPlayer player = CreateMockPlayerWithLevelManager(4);

        // Act
        player.levelManager.SpellLevelUpRequest("Spell_4");

        // Assert
        Assert.AreEqual(0, player.levelManager.spellLevels["Spell_4"]);
    }

    [Test]
    public void levels_spell_4_to_level_1(){
        // Arrange
        IPlayer player = CreateMockPlayerWithLevelManager(6);

        // Act
        player.levelManager.SpellLevelUpRequest("Spell_4");

        // Assert
        Assert.AreEqual(1, player.levelManager.spellLevels["Spell_4"]);
    }

    [Test]
    public void does_not_level_spell_4_to_level_2_from_invalid_level_requirements(){
        // Arrange
        IPlayer player = CreateMockPlayerWithLevelManager(10);

        // Act
        player.levelManager.SpellLevelUpRequest("Spell_4");
        player.levelManager.SpellLevelUpRequest("Spell_4");

        // Assert
        Assert.AreEqual(1, player.levelManager.spellLevels["Spell_4"]);
    }

    [Test]
    public void levels_spell_4_to_level_2(){
        // Arrange
        IPlayer player = CreateMockPlayerWithLevelManager(11);

        // Act
        player.levelManager.SpellLevelUpRequest("Spell_4");
        player.levelManager.SpellLevelUpRequest("Spell_4");

        // Assert
        Assert.AreEqual(2, player.levelManager.spellLevels["Spell_4"]);
    }

    [Test]
    public void does_not_level_spell_4_to_level_3_from_invalid_level_requirements(){
        // Arrange
        IPlayer player = CreateMockPlayerWithLevelManager(15);

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
        IPlayer player = CreateMockPlayerWithLevelManager(16);

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
        int maxUltLevel = levelInfo.maxUltLevel;
        IPlayer player = CreateMockPlayerWithLevelManager(17);
        
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
        IPlayer player = CreateMockPlayerWithLevelManager(4);

        // Act
        float respawnTime = player.levelManager.RespawnTime();

        // Assert
        Assert.AreEqual(12f, respawnTime);
    }

    [Test]
    public void gets_respawn_time_from_player_at_level_7(){
        // Arrange
        IPlayer player = CreateMockPlayerWithLevelManager(7);

        // Act
        float respawnTime = player.levelManager.RespawnTime();

        // Assert
        Assert.AreEqual(21f, respawnTime);
    }

    [Test]
    public void gets_respawn_time_from_player_above_level_7(){
        // Arrange
        IPlayer player = CreateMockPlayerWithLevelManager(15);

        // Act
        float respawnTime = player.levelManager.RespawnTime();

        // Assert
        Assert.AreEqual(45f, respawnTime);
    }

    private IPlayer CreateMockPlayerWithLevelManager(int level){
        IPlayer player = Substitute.For<IPlayer>();
        if(level > 1)
            player.levelManager = new LevelManager(player, level);
        else
            player.levelManager = new LevelManager(player);
        return player;
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
