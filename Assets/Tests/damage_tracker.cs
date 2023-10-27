using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

public class damage_tracker
{
    // A Test behaves as an ordinary method
    [Test]
    public void add_damage_to_damage_received_list()
    {
        // Arrange
        DamageTracker dt = new DamageTracker();
        IUnit unit1 = Substitute.For<IUnit>();

        // Act
        dt.AddDamage(unit1, 32f, DamageType.Magic);

        // Assert
        Assert.AreEqual(1, dt.damageReceived.Count);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void resets_damage_received_list_from_not_receiving_damage_for_the_out_of_combat_time()
    {
        // Arrange
        DamageTracker dt = new DamageTracker();
        IUnit unit1 = Substitute.For<IUnit>();
        dt.AddDamage(unit1, 323f, DamageType.True);
        dt.AddDamage(unit1, 324f, DamageType.Magic);
        dt.AddDamage(unit1, 325f, DamageType.Physical);

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
        IPlayer player1 = new MockPlayer();
        IPlayer player2 = new MockPlayer();
        IUnit unit1 = Substitute.For<IUnit>();
        IUnit unit2 = Substitute.For<IUnit>();
        IUnit unit3 = Substitute.For<IUnit>();

        dt.AddDamage(player2, 3f, DamageType.Physical);
        dt.AddDamage(unit1, 1f, DamageType.True);
        dt.AddDamage(unit2, 2f, DamageType.Magic);
        dt.AddDamage(unit3, 3f, DamageType.Physical);
        dt.AddDamage(player1, 15f, DamageType.Physical);

        // Act
        List<IPlayer> assists = dt.CheckForAssists(unit3, 10f);

        // Assert
        Assert.AreEqual((player2, player1), (assists[0], assists[1]));
    }
}
