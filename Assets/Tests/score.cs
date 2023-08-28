using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/*
* Purpose: Tests for the Score class.
*
* @author: Colin Keys
*/
public class score
{
    /*
    *   Adds one kill with the takedown callback being null. Should have one kill.
    */
    [Test]
    public void adds_1_champion_kill_with_null_takedown_callback(){
        // Arrange
        MockUnit mockUnit = new MockUnit();
        Score score = new Score(null);

        // Act
        score.ChampionKill(mockUnit);

        // Assert
        Assert.AreEqual(1, score.Kills);
    }

    /*
    *   Adds one kill with the takedown callback not being null. Should have used the callback.
    */
    [Test]
    public void adds_champion_kill_with_takedown_callback_change_gameobject_name(){
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

    /*
    *   Adds two kills with the takedown callback not being null. Should have two kills.
    */
    [Test]
    public void adds_2_champion_kills_with_takedown_callback(){
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

    /*
    *   Adds one assist with takedown callback being null. Should have one assist.
    */
    [Test]
    public void adds_1_assist_with_null_takedown_callback(){
        // Arrange
        MockUnit mockUnit = new MockUnit();
        Score score = new Score(null);

        // Act
        score.Assist(mockUnit);

        // Assert
        Assert.AreEqual(1, score.Assists);
    }

    /*
    *   Adds one assist with the takedown callback not being null. Should have used the callback.
    */
    [Test]
    public void adds_assist_with_takedown_callback_change_gameobject_name(){
        // Arrange
        MockUnit mockUnit = new MockUnit();
        Score score = new Score(null);
        mockUnit.SUnit.name = "tempName";
        score.takedownCallback += (mockUnit) => mockUnit.SUnit.name = "Takedown Callback executed.";
        
        // Act
        score.Assist(mockUnit);

        // Assert
        Assert.AreEqual("Takedown Callback executed.", mockUnit.SUnit.name);
    }

    /*
    *   Adds two assists with the takedown callback not being null. Should have two assists.
    */
    [Test]
    public void adds_2_assists_with_takedown_callback(){
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

    /*
    *   Adds one cs with the takedown callback being null. Should have one cs.
    */
    [Test]
    public void adds_1_creep_kill_with_null_takedown_callback(){
        // Arrange
        MockUnit mockUnit = new MockUnit();
        Score score = new Score(null);

        // Act
        score.CreepKill(mockUnit);

        // Assert
        Assert.AreEqual(1, score.CS);
    }

    /*
    *   Adds one cs with the takedown callback not being null. Should have used the callback.
    */
    [Test]
    public void adds_creep_kill_with_takedown_callback_change_gameobject_name(){
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

    /*
    *   Adds two cs with the takedown callback not being null. Should have two cs.
    */
    [Test]
    public void adds_2_creep_kills_with_takedown_callback(){
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

    /*
    *   Adds one death. Should have one death.
    */
    [Test]
    public void adds_1_death(){
        // Arrange
        Score score = new Score(null);

        // Act
        score.Death();

        // Assert
        Assert.AreEqual(1, score.Deaths);
    }
}
