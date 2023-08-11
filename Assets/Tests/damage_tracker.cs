using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class damage_tracker
{
    // A Test behaves as an ordinary method
    [Test]
    public void add_damage_to_damage_received_list()
    {
        // Arrange
        DamageTracker dt = new DamageTracker();
        GameObject g1 = new GameObject();

        // Act
        dt.AddDamage(g1, 32f, "magic");

        // Assert
        Assert.AreEqual(1, dt.damageReceived.Count);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void resets_damage_received_list_from_not_receiving_damage_for_the_out_of_combat_time()
    {
        // Arrange
        DamageTracker dt = new DamageTracker();
        GameObject g1 = new GameObject();
        dt.AddDamage(g1, 323f, "true");
        dt.AddDamage(g1, 324f, "magic");
        dt.AddDamage(g1, 325f, "physical");

        // Act
        dt.CheckForReset(500f);

        // Assert
        Assert.AreEqual(0, dt.damageReceived.Count);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void returns_list_of_game_objects_that_get_an_assist()
    {
        // Arrange
        DamageTracker dt = new DamageTracker();
        GameObject g1 = new GameObject();
        GameObject g2 = new GameObject();
        GameObject g3 = new GameObject();

        dt.AddDamage(g1, 1f, "true");
        dt.AddDamage(g2, 2f, "magic");
        dt.AddDamage(g3, 3f, "physical");

        // Act
        List<GameObject> assists = dt.CheckForAssists();

        // Assert
        Assert.AreEqual((g1, g2, g3), (assists[0], assists[1], assists[2]));
    }
}
