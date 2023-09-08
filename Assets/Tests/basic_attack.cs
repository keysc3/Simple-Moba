using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

public class basic_attack
{
    private readonly IUnit nullUnit = null;

    // A Test behaves as an ordinary method
    [Test]
    public void returns_false_can_auto_from_no_target()
    {
        // Arrange
        IBasicAttack basicAttack = Substitute.For<IBasicAttack>();
        IPlayerMover playerMover = CreateMockPlayerMoverWithTarget(nullUnit);
        IPlayer player = Substitute.For<IPlayer>();
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        player.IsCasting.Returns(false);

        // Act
        bool canAuto = controller.CanAuto(1f);

        // Assert
        Assert.False(canAuto);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void returns_false_can_auto_from_player_casting()
    {
        // Arrange
        IPlayer player = Substitute.For<IPlayer>();
        IUnit unit = Substitute.For<IUnit>();
        IBasicAttack basicAttack = Substitute.For<IBasicAttack>();
        IPlayerMover playerMover = CreateMockPlayerMoverWithTarget(unit);
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        player.IsCasting.Returns(true);

        // Act
        bool canAuto = controller.CanAuto(1f);

        // Assert
        Assert.False(canAuto);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void returns_false_can_auto_from_dead_targeted_enemy()
    {
        // Arrange
        IPlayer player = Substitute.For<IPlayer>();
        IUnit unit = Substitute.For<IUnit>();
        IBasicAttack basicAttack = Substitute.For<IBasicAttack>();
        IPlayerMover playerMover = CreateMockPlayerMoverWithTarget(unit);
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        unit.IsDead.Returns(true);

        // Act
        bool canAuto = controller.CanAuto(1f);

        // Assert
        Assert.False(canAuto);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void returns_false_can_auto_from_out_of_range()
    {
        // Arrange
        IPlayer player = CreateMockPlayerWithRange(4.9f);
        IUnit unit = Substitute.For<IUnit>();
        IBasicAttack basicAttack = Substitute.For<IBasicAttack>();
        IPlayerMover playerMover = CreateMockPlayerMoverWithTarget(unit);
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        // Magnitude 5 vector.
        player.Position.Returns(new Vector3(7f, 0f, 9f));
        unit.Position.Returns(new Vector3(4f, 0f, 5f));

        // Act
        bool canAuto = controller.CanAuto(1f);

        // Assert
        Assert.False(canAuto);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void returns_false_can_auto_from_next_auto_not_available_yet()
    {
        // Arrange
            IPlayer player = CreateMockPlayerWithRange(5.1f);
        IUnit unit = Substitute.For<IUnit>();
        IBasicAttack basicAttack = Substitute.For<IBasicAttack>();
        IPlayerMover playerMover = CreateMockPlayerMoverWithTarget(unit);
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        basicAttack.NextAuto.Returns(2f);
        // Magnitude 5 vector.
        player.Position.Returns(new Vector3(7f, 0f, 9f));
        unit.Position.Returns(new Vector3(4f, 0f, 5f));

        // Act
        bool canAuto = controller.CanAuto(1.9f);

        // Assert
        Assert.False(canAuto);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void returns_false_can_auto_from_winding_up()
    {
        // Arrange
        IPlayer player = CreateMockPlayerWithRange(5.1f);
        IUnit unit = Substitute.For<IUnit>();
        IBasicAttack basicAttack = Substitute.For<IBasicAttack>();
        IPlayerMover playerMover = CreateMockPlayerMoverWithTarget(unit);
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        basicAttack.NextAuto.Returns(2f);
        basicAttack.WindingUp.Returns(true);
        // Magnitude 5 vector.
        player.Position.Returns(new Vector3(7f, 0f, 9f));
        unit.Position.Returns(new Vector3(4f, 0f, 5f));

        // Act
        bool canAuto = controller.CanAuto(2.1f);

        // Assert
        Assert.False(canAuto);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void returns_true_can_auto()
    {
        // Arrange
        IPlayer player = CreateMockPlayerWithRange(5.1f);
        IUnit unit = Substitute.For<IUnit>();
        IBasicAttack basicAttack = Substitute.For<IBasicAttack>();
        IPlayerMover playerMover = CreateMockPlayerMoverWithTarget(unit);
        BasicAttackController controller = new BasicAttackController(basicAttack, playerMover, player);
        basicAttack.NextAuto.Returns(2f);
        basicAttack.WindingUp.Returns(false);
        // Magnitude 5 vector.
        player.Position.Returns(new Vector3(7f, 0f, 9f));
        unit.Position.Returns(new Vector3(4f, 0f, 5f));

        // Act
        bool canAuto = controller.CanAuto(2.1f);

        // Assert
        Assert.True(canAuto);
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

    private IPlayer CreateMockPlayerWithRange(float range){
        IPlayer player = Substitute.For<IPlayer>();
        UnitStats unitStats = new UnitStats(ScriptableObject.CreateInstance<ScriptableUnit>());
        unitStats.autoRange.BaseValue = range;
        player.unitStats.Returns(unitStats);
        return player;
    }

    private IPlayerMover CreateMockPlayerMoverWithTarget(IUnit target){
        IPlayerMover playerMover = Substitute.For<IPlayerMover>();
        playerMover.TargetedEnemy.Returns(target);
        return playerMover;
    }
}
