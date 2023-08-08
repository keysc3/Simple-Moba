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
        Score score = new Score();
        GameObject tempObject = new GameObject();

        // Act
        score.ChampionKill(tempObject);

        // Assert
        Assert.AreEqual(1, score.kills);
    }

    /*
    *   Adds one kill with the takedown callback not being null. Should have used the callback.
    */
    [Test]
    public void adds_champion_kill_with_takedown_callback_change_gameobject_name(){
        // Arrange
        Score score = new Score();
        GameObject tempObject = new GameObject();
        tempObject.name = "tempObject";
        score.takedownCallback += (tempObject) => tempObject.name = "Takedown Callback executed.";
        
        // Act
        score.ChampionKill(tempObject);

        // Assert
        Assert.AreEqual("Takedown Callback executed.", tempObject.name);
    }

    /*
    *   Adds two kills with the takedown callback not being null. Should have two kills.
    */
    [Test]
    public void adds_2_champion_kills_with_takedown_callback(){
        // Arrange
        Score score = new Score();
        GameObject tempObject1 = new GameObject();
        GameObject tempObject2 = new GameObject();
        tempObject1.name = "tempObject1";
        tempObject2.name = "tempObject2";

        score.takedownCallback += (tempObject) => tempObject.name = "Takedown Callback executed.";
        
        // Act
        score.ChampionKill(tempObject1);
        score.ChampionKill(tempObject2);

        // Assert
        Assert.AreEqual(2, score.kills);
    }

    /*
    *   Adds one assist with takedown callback being null. Should have one assist.
    */
    [Test]
    public void adds_1_assist_with_null_takedown_callback(){
        // Arrange
        Score score = new Score();
        GameObject tempObject = new GameObject();

        // Act
        score.Assist(tempObject);

        // Assert
        Assert.AreEqual(1, score.assists);
    }

    /*
    *   Adds one assist with the takedown callback not being null. Should have used the callback.
    */
    [Test]
    public void adds_assist_with_takedown_callback_change_gameobject_name(){
        // Arrange
        Score score = new Score();
        GameObject tempObject = new GameObject();
        tempObject.name = "tempObject";
        score.takedownCallback += (tempObject) => tempObject.name = "Takedown Callback executed.";
        
        // Act
        score.Assist(tempObject);

        // Assert
        Assert.AreEqual("Takedown Callback executed.", tempObject.name);
    }

    /*
    *   Adds two assists with the takedown callback not being null. Should have two assists.
    */
    [Test]
    public void adds_2_assists_with_takedown_callback(){
        // Arrange
        Score score = new Score();
        GameObject tempObject1 = new GameObject();
        GameObject tempObject2 = new GameObject();
        tempObject1.name = "tempObject1";
        tempObject2.name = "tempObject2";
        score.takedownCallback += (tempObject) => tempObject.name = "Takedown Callback executed.";
        
        // Act
        score.Assist(tempObject1);
        score.Assist(tempObject2);

        // Assert
        Assert.AreEqual(2, score.assists);
    }

    /*
    *   Adds one cs with the takedown callback being null. Should have one cs.
    */
    [Test]
    public void adds_1_creep_kill_with_null_takedown_callback(){
        // Arrange
        Score score = new Score();
        GameObject tempObject = new GameObject();

        // Act
        score.CreepKill(tempObject);

        // Assert
        Assert.AreEqual(1, score.cs);
    }

    /*
    *   Adds one cs with the takedown callback not being null. Should have used the callback.
    */
    [Test]
    public void adds_creep_kill_with_takedown_callback_change_gameobject_name(){
        // Arrange
        Score score = new Score();
        GameObject tempObject = new GameObject();
        tempObject.name = "tempObject";
        score.takedownCallback += (tempObject) => tempObject.name = "Takedown Callback executed.";
        
        // Act
        score.CreepKill(tempObject);

        // Assert
        Assert.AreEqual("Takedown Callback executed.", tempObject.name);
    }

    /*
    *   Adds two cs with the takedown callback not being null. Should have two cs.
    */
    [Test]
    public void adds_2_creep_kills_with_takedown_callback(){
        // Arrange
        Score score = new Score();
        GameObject tempObject1 = new GameObject();
        GameObject tempObject2 = new GameObject();
        tempObject1.name = "tempObject1";
        tempObject2.name = "tempObject2";


        score.takedownCallback += (tempObject) => tempObject.name = "Takedown Callback executed.";
        
        // Act
        score.CreepKill(tempObject1);
        score.CreepKill(tempObject2);

        // Assert
        Assert.AreEqual(2, score.cs);
    }

    /*
    *   Adds one death. Should have one death.
    */
    [Test]
    public void adds_1_death(){
        // Arrange
        Score score = new Score();

        // Act
        score.Death();

        // Assert
        Assert.AreEqual(1, score.deaths);
    }
}
