using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

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
        IUnit unit = Substitute.For<IUnit>();
        Score score = new Score(null);

        // Act
        score.ChampionKill(unit);

        // Assert
        Assert.AreEqual(1, score.Kills);
    }

    /*
    *   Adds one kill with the takedown callback not being null. Should have used the callback.
    */
    [Test]
    public void adds_champion_kill_with_takedown_callback_that_changes_bool(){
        // Arrange
        IUnit unit = Substitute.For<IUnit>();
        Score score = new Score(null);
        bool wasCalled = false;
        score.takedownCallback += (unit) => wasCalled = true;
        
        // Act
        score.ChampionKill(unit);

        // Assert
        Assert.True(wasCalled);
    }

    /*
    *   Adds two kills with the takedown callback not being null. Should have two kills.
    */
    [Test]
    public void adds_2_champion_kills_with_takedown_callback(){
        // Arrange
        IUnit unit = Substitute.For<IUnit>();
        Score score = new Score(null);
        int calls = 0;

        score.takedownCallback += (unit) => calls++;
        
        // Act
        score.ChampionKill(unit);
        score.ChampionKill(unit);

        // Assert
        Assert.AreEqual((2, 2), (score.Kills, calls));
    }

    /*
    *   Adds one assist with takedown callback being null. Should have one assist.
    */
    [Test]
    public void adds_1_assist_with_null_takedown_callback(){
        // Arrange
        IUnit unit = Substitute.For<IUnit>();
        Score score = new Score(null);

        // Act
        score.Assist(unit);

        // Assert
        Assert.AreEqual(1, score.Assists);
    }

    /*
    *   Adds one assist with the takedown callback not being null. Should have used the callback.
    */
    [Test]
    public void adds_assist_with_takedown_callback_that_changes_bool(){
        // Arrange
        IUnit unit = Substitute.For<IUnit>();
        Score score = new Score(null);
        bool wasCalled = false;

        score.takedownCallback += (unit) => wasCalled = true;
        
        // Act
        score.Assist(unit);

        // Assert
        Assert.True(wasCalled);
    }

    /*
    *   Adds two assists with the takedown callback not being null. Should have two assists.
    */
    [Test]
    public void adds_2_assists_with_takedown_callback(){
        // Arrange
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();
        Score score = new Score(null);
        
        // Act
        score.Assist(unit1);
        score.Assist(unit2);

        // Assert
        Assert.AreEqual(2, score.Assists);
    }

    /*
    *   Adds one cs with the takedown callback being null. Should have one cs.
    */
    [Test]
    public void adds_1_creep_kill_with_null_takedown_callback(){
        // Arrange
        IUnit unit = Substitute.For<IUnit>();
        Score score = new Score(null);

        // Act
        score.CreepKill(unit);

        // Assert
        Assert.AreEqual(1, score.CS);
    }

    /*
    *   Adds one cs with the takedown callback not being null. Should have used the callback.
    */
    [Test]
    public void adds_creep_kill_with_takedown_callback_that_changes_bool(){
        // Arrange
        IUnit unit = Substitute.For<IUnit>();
        Score score = new Score(null);
        bool wasCalled = false;

        score.takedownCallback += (unit) => wasCalled = true;
        
        // Act
        score.CreepKill(unit);

        // Assert
        Assert.True(wasCalled);
    }

    /*
    *   Adds two cs with the takedown callback not being null. Should have two cs.
    */
    [Test]
    public void adds_2_creep_kills_with_takedown_callback(){
        // Arrange
        IUnit unit = Substitute.For<IUnit>();
        Score score = new Score(null);
        int calls = 0;

        score.takedownCallback += (unit) => calls++;
        
        // Act
        score.CreepKill(unit);
        score.CreepKill(unit);

        // Assert
        Assert.AreEqual((2, 2), (score.CS, calls));
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
