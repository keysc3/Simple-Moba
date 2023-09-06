using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

public class basic_attack
{
    // A Test behaves as an ordinary method
    [Test]
    public void returns_false_can_auto_from_no_target()
    {
        // Arrange
        MockBasicAttack basicAttack = new MockBasicAttack();
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);

        // Act
        bool canAuto = controller.CanAuto(1f);

        // Assert
        Assert.AreEqual(false, canAuto);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void returns_false_can_auto_from_player_casting()
    {
        // Arrange
        MockBasicAttack basicAttack = new MockBasicAttack();
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        MockUnit unit = new MockUnit();
        player.IsCasting = true;
        playerMover.TargetedEnemy = unit;

        // Act
        bool canAuto = controller.CanAuto(1f);

        // Assert
        Assert.AreEqual(false, canAuto);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void returns_false_can_auto_from_dead_targeted_enemy()
    {
        // Arrange
        MockBasicAttack basicAttack = new MockBasicAttack();
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        MockUnit unit = new MockUnit();
        playerMover.TargetedEnemy = unit;
        unit.IsDead = true;

        // Act
        bool canAuto = controller.CanAuto(1f);

        // Assert
        Assert.AreEqual(false, canAuto);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void returns_false_can_auto_from_out_of_range()
    {
        // Arrange
        MockBasicAttack basicAttack = new MockBasicAttack();
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        MockUnit unit = new MockUnit();
        player.unitStats = new ChampionStats((ScriptableChampion) player.SUnit);
        player.unitStats.autoRange.BaseValue = 4.9f;
        playerMover.TargetedEnemy = unit;
        // Magnitude 5 vector.
        player.Position = new Vector3(7f, 0f, 9f);
        unit.Position = new Vector3(4f, 0f, 5f);

        // Act
        bool canAuto = controller.CanAuto(1f);

        // Assert
        Assert.AreEqual(false, canAuto);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void returns_false_can_auto_from_next_auto_not_available_yet()
    {
        // Arrange
        MockBasicAttack basicAttack = new MockBasicAttack();
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        MockUnit unit = new MockUnit();
        basicAttack.NextAuto = 2f;
        player.unitStats = new ChampionStats((ScriptableChampion) player.SUnit);
        player.unitStats.autoRange.BaseValue = 5.1f;
        playerMover.TargetedEnemy = unit;
        // Magnitude 5 vector.
        player.Position = new Vector3(7f, 0f, 9f);
        unit.Position = new Vector3(4f, 0f, 5f);

        // Act
        bool canAuto = controller.CanAuto(1.9f);

        // Assert
        Assert.AreEqual(false, canAuto);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void returns_false_can_auto_from_winding_up()
    {
        // Arrange
        MockBasicAttack basicAttack = new MockBasicAttack();
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        MockUnit unit = new MockUnit();
        basicAttack.NextAuto = 2f;
        basicAttack.WindingUp = true;
        player.unitStats = new ChampionStats((ScriptableChampion) player.SUnit);
        player.unitStats.autoRange.BaseValue = 5.1f;
        playerMover.TargetedEnemy = unit;
        // Magnitude 5 vector.
        player.Position = new Vector3(7f, 0f, 9f);
        unit.Position = new Vector3(4f, 0f, 5f);

        // Act
        bool canAuto = controller.CanAuto(2.1f);

        // Assert
        Assert.AreEqual(false, canAuto);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void returns_true_can_auto()
    {
        // Arrange
        MockBasicAttack basicAttack = new MockBasicAttack();
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        MockUnit unit = new MockUnit();
        basicAttack.NextAuto = 2f;
        basicAttack.WindingUp = false;
        player.unitStats = new ChampionStats((ScriptableChampion) player.SUnit);
        player.unitStats.autoRange.BaseValue = 5.1f;
        playerMover.TargetedEnemy = unit;
        // Magnitude 5 vector.
        player.Position = new Vector3(7f, 0f, 9f);
        unit.Position = new Vector3(4f, 0f, 5f);

        // Act
        bool canAuto = controller.CanAuto(2.1f);

        // Assert
        Assert.AreEqual(true, canAuto);
    }

    [Test]
    public void basic_attack_hits_non_damageable_unit(){
         // Arrange
        MockBasicAttack basicAttack = new MockBasicAttack();
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        MockUnit unit = new MockUnit();
        basicAttack.PhysicalDamage = 10f;
        unit.GameObject.name = "No damage please";

        // Act
        controller.AttackHit(unit);

        // Assert
        Assert.AreEqual("No damage please", unit.GameObject.name);
    }

    [Test]
    public void basic_attack_hits_damageable_unit(){
        // Arrange
        IPlayer player = Substitute.For<IPlayer>();
        IPlayer enemy = Substitute.For<IPlayer>();
        IBasicAttack basicAttack = Substitute.For<IBasicAttack>();
        IPlayerMover playerMover = Substitute.For<IPlayerMover>();
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        basicAttack.PhysicalDamage.Returns(14f);

        // Act
        controller.AttackHit(enemy);

        // Assert
        enemy.Received().TakeDamage(14f, "physical", player, false);
    }

}
