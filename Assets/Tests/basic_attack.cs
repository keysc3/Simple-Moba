using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class basic_attack
{
    // A Test behaves as an ordinary method
    [Test]
    public void returns_false_can_auto_from_no_target()
    {
        // Arrange
        IBasicAttack basicAttack = new MockBasicAttack();
        IPlayerMover playerMover = new MockPlayerMover();
        IPlayer player = new MockPlayer();
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);

        // Act
        bool canAuto = controller.CanAuto(1f);

        // Assert
        Assert.AreEqual(false, canAuto);
    }
}
