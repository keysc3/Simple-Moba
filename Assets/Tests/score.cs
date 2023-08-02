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
        Score score = new Score();
        GameObject tempObject = new GameObject();

        // Act
        score.ChampionKill(tempObject);

        // Assert
        Assert.AreEqual(1, score.kills);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void add_champion_kill_with_takedown_callback_change_gameobject_name()
    {
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

    // A Test behaves as an ordinary method
    [Test]
    public void add_2_champion_kills_with_takedown_callback()
    {
        // Arrange
        Score score = new Score();
        GameObject tempObject = new GameObject();
        tempObject.name = "tempObject";

        score.takedownCallback += (tempObject) => tempObject.name = "Takedown Callback executetd.";
        
        // Act
        score.ChampionKill(tempObject);
        score.ChampionKill(tempObject);

        // Assert
        Assert.AreEqual(2, score.kills);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void add_1_assist()
    {
        // Arrange
        Score score = new Score();

        // Act
        score.Assist();

        // Assert
        Assert.AreEqual(1, score.assists);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void add_2_assists()
    {
        // Arrange
        Score score = new Score();
        
        // Act
        score.Assist();
        score.Assist();

        // Assert
        Assert.AreEqual(2, score.assists);
    }
}
