using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class score
{
    // A Test behaves as an ordinary method
    [Test]
    public void add_1_champion_kill_with_null_takedown_callback()
    {
        // Arrange
        MockUnit mockUnit = new MockUnit();
        Score score = new Score(null);

        // Act
        score.ChampionKill(mockUnit);

        // Assert
        Assert.AreEqual(1, score.Kills);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void add_champion_kill_with_takedown_callback_change_gameobject_name()
    {
        // Arrange
        MockUnit mockUnit = new MockUnit();
        Score score = new Score(null);
        mockUnit.SUnit.name = "tempName";
        score.takedownCallback += (mockUnit) => mockUnit.SUnit.name = "Takedown Callback executed.";
        
        // Act
        score.ChampionKill(mockUnit);

        // Assert
        Assert.AreEqual("Takedown Callback executed.", mockUnit.SUnit.name);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void add_2_champion_kills_with_takedown_callback()
    {
        // Arrange
        MockUnit mockUnit = new MockUnit();
        Score score = new Score(null);
        mockUnit.SUnit.name = "tempName";

        score.takedownCallback += (mockUnit) => mockUnit.SUnit.name = "Takedown Callback executed.";
        
        // Act
        score.ChampionKill(mockUnit);
        score.ChampionKill(mockUnit);

        // Assert
        Assert.AreEqual(2, score.Kills);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void add_1_assist()
    {
        // Arrange
        MockUnit mockUnit = new MockUnit();
        Score score = new Score(null);

        // Act
        score.Assist(mockUnit);

        // Assert
        Assert.AreEqual(1, score.Assists);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void add_2_assists()
    {
        // Arrange
        MockUnit mockUnit1 = new MockUnit();
        MockUnit mockUnit2 = new MockUnit();
        Score score = new Score(null);
        
        // Act
        score.Assist(mockUnit1);
        score.Assist(mockUnit2);

        // Assert
        Assert.AreEqual(2, score.Assists);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void add_1_creep_kill_with_null_takedown_callback()
    {
        // Arrange
        MockUnit mockUnit = new MockUnit();
        Score score = new Score(null);

        // Act
        score.CreepKill(mockUnit);

        // Assert
        Assert.AreEqual(1, score.CS);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void add_creep_kill_with_takedown_callback_change_gameobject_name()
    {
        // Arrange
        MockUnit mockUnit = new MockUnit();
        Score score = new Score(null);
        mockUnit.SUnit.name = "tempName";

        score.takedownCallback += (mockUnit) => mockUnit.SUnit.name = "Takedown Callback executed.";
        
        // Act
        score.CreepKill(mockUnit);

        // Assert
        Assert.AreEqual("Takedown Callback executed.", mockUnit.SUnit.name);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void add_2_creep_kills_with_takedown_callback()
    {
        // Arrange
        MockUnit mockUnit = new MockUnit();
        Score score = new Score(null);
        mockUnit.SUnit.name = "tempName";

        score.takedownCallback += (mockUnit) => mockUnit.SUnit.name = "Takedown Callback executed.";
        
        // Act
        score.CreepKill(mockUnit);
        score.CreepKill(mockUnit);

        // Assert
        Assert.AreEqual(2, score.CS);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void add_1_death()
    {
        // Arrange
        Score score = new Score(null);

        // Act
        score.Death();

        // Assert
        Assert.AreEqual(1, score.Deaths);
    }
}
