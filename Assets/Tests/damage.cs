using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

public class damage
{
    // A Test behaves as an ordinary method
    [Test]
    public void creates_new_damage_object_with_20_magic_damage(){
        // Arrange
        IUnit unit1 = Substitute.For<IUnit>();

        // Act
        Damage dmg = new Damage(unit1, 20f, "magic");

        // Assert
        Assert.AreEqual((unit1, 20f, "magic"), (dmg.from, dmg.amount, dmg.type));

    }
}
