using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

public class player_controller
{
    private IUnit nullUnit = null;
    private ISpell nullSpell = null;

    [Test]
    public void set_target_enemy_and_destination_from_valid_right_click_with_no_unit(){
        // Arrange
        IPlayerMover playerMover = Substitute.For<IPlayerMover>();
        IPlayer player = Substitute.For<IPlayer>();
        PlayerController controller = new PlayerController(playerMover, player);
        playerMover.Destination = Vector3.zero;
        playerMover.TargetedEnemy = null;

        Vector3 targetVec = new Vector3(-12f, 96f, 93f);

        //Act
        controller.RightClick(null, targetVec);

        // Assert
        Assert.AreEqual((nullUnit, new Vector3(-12f, 96f, 93f)), (playerMover.TargetedEnemy, playerMover.Destination));
    }

    [Test]
    public void set_target_enemy_from_valid_right_click_on_enemy_unit(){
        // Arrange
        IPlayerMover playerMover = Substitute.For<IPlayerMover>();
        IPlayer player = Substitute.For<IPlayer>();
        PlayerController controller = new PlayerController(playerMover, player);
        IUnit target = Substitute.For<IUnit>();
        GameObject g1 = new GameObject();
        g1.tag = "Enemy";
        target.GameObject.Returns(g1);
        target.IsDead.Returns(false);
        Vector3 targetVec = new Vector3(6f, 31f, -54f);

        // Act
        controller.RightClick(target, targetVec);

        // Assert
        Assert.AreEqual(target, playerMover.TargetedEnemy);
    }

    [Test]
    public void set_target_enemy_and_destination_from_right_click_on_non_enemy_unit(){
        // Arrange
        IPlayerMover playerMover = Substitute.For<IPlayerMover>();
        IPlayer player = Substitute.For<IPlayer>();
        PlayerController controller = new PlayerController(playerMover, player);
        IUnit target = Substitute.For<IUnit>();
        target.GameObject.Returns(new GameObject());

        Vector3 targetVec = new Vector3(10f, 3f, 5f);

        // Act
        controller.RightClick(target, targetVec);

        // Assert
        Assert.AreEqual((nullUnit, new Vector3(10f, 3f, 5f)), (playerMover.TargetedEnemy, playerMover.Destination));
    }
    
    [Test]
    public void set_target_and_destination_from_right_click_on_dead_enemy_unit(){
        // Arrange
        IPlayerMover playerMover = Substitute.For<IPlayerMover>();
        IPlayer player = Substitute.For<IPlayer>();
        PlayerController controller = new PlayerController(playerMover, player);
        IUnit target = Substitute.For<IUnit>();
        GameObject g1 = new GameObject();
        g1.tag = "Enemy";
        target.GameObject.Returns(g1);
        target.IsDead.Returns(true);
        Vector3 targetVec = new Vector3(76f, 67f, 767f);

        // Act
        controller.RightClick(target, targetVec);

        // Assert
        Assert.AreEqual((nullUnit, new Vector3(76f, 67f, 767f)), (playerMover.TargetedEnemy, playerMover.Destination));
    }

    [Test]
    public void set_player_look_direction_to_mouse_on_cast(){
        // Arrange
        IPlayerMover playerMover = Substitute.For<IPlayerMover>();
        IPlayer player = Substitute.For<IPlayer>();
        PlayerController controller = new PlayerController(playerMover, player);
        ISpell spell = Substitute.For<ISpell>();

        spell.CanMove.Returns(false);
        player.MouseOnCast.Returns(new Vector3(1f, 2f, 3f));
        player.IsCasting.Returns(true);
        player.CurrentCastedSpell.Returns(spell);

        // Act
        controller.PlayerLookDirection();

        // Assert
        Assert.AreEqual(new Vector3(1f, 2f, 3f), playerMover.CurrentTarget);
    }

    [Test]
    public void set_player_look_direction_to_next_destination_from_is_casting_false(){
        // Arrange
        IPlayerMover playerMover = Substitute.For<IPlayerMover>();
        IPlayer player = Substitute.For<IPlayer>();
        PlayerController controller = new PlayerController(playerMover, player);
        ISpell spell = Substitute.For<ISpell>();

        playerMover.NextDestination.Returns(new Vector3(3f, 2f, 1f));
        spell.CanMove.Returns(false);
        player.MouseOnCast.Returns(new Vector3(1f, 2f, 3f));
        player.IsCasting.Returns(false);
        player.CurrentCastedSpell.Returns(spell);

        // Act
        controller.PlayerLookDirection();

        // Assert
        Assert.AreEqual(new Vector3(3f, 2f, 1f), playerMover.CurrentTarget);
    }

    [Test]
    public void set_player_look_direction_to_next_destination_from_current_spell_null(){
        // Arrange
        IPlayerMover playerMover = Substitute.For<IPlayerMover>();
        IPlayer player = Substitute.For<IPlayer>();
        PlayerController controller = new PlayerController(playerMover, player);

        playerMover.NextDestination.Returns(new Vector3(3f, 2f, 1f));
        player.MouseOnCast.Returns(new Vector3(1f, 2f, 3f));
        player.IsCasting.Returns(true);
        player.CurrentCastedSpell.Returns(nullSpell);

        // Act
        controller.PlayerLookDirection();

        // Assert
        Assert.AreEqual(new Vector3(3f, 2f, 1f), playerMover.CurrentTarget);
    }

    [Test]
    public void set_player_look_direction_to_next_destination_from_current_spell_can_move_true(){
        // Arrange
        IPlayerMover playerMover = Substitute.For<IPlayerMover>();
        IPlayer player = Substitute.For<IPlayer>();
        PlayerController controller = new PlayerController(playerMover, player);
        ISpell spell = Substitute.For<ISpell>();

        playerMover.NextDestination.Returns(new Vector3(3f, 2f, 1f));
        spell.CanMove.Returns(true);
        player.MouseOnCast.Returns(new Vector3(1f, 2f, 3f));
        player.IsCasting.Returns(true);
        player.CurrentCastedSpell.Returns(spell);

        // Act
        controller.PlayerLookDirection();

        // Assert
        Assert.AreEqual(new Vector3(3f, 2f, 1f), playerMover.CurrentTarget);
    }
    
    [Test]
    public void does_not_set_destination_from_null_target_enemy(){
        // Arrange
        IPlayerMover playerMover = CreateMockPlayerMoverWithDest(new Vector3(10f, 9f, 8f));
        IPlayer player = Substitute.For<IPlayer>();
        PlayerController controller = new PlayerController(playerMover, player);
        playerMover.TargetedEnemy.Returns(nullUnit);
        player.IsCasting.Returns(false);

        // Act
        controller.SetPlayerDestinationUsingTarget();

        // Assert
        Assert.AreEqual(new Vector3(10f, 9f, 8f), playerMover.Destination);
    }

    [Test]
    public void does_not_set_destination_from_is_casting_true(){
        // Arrange
        IPlayerMover playerMover = CreateMockPlayerMoverWithDest(new Vector3(10f, 9f, 8f));
        IPlayer player = Substitute.For<IPlayer>();
        PlayerController controller = new PlayerController(playerMover, player);
        IUnit unit = Substitute.For<IUnit>();

        player.IsCasting.Returns(true);
        playerMover.TargetedEnemy.Returns(unit);

        // Act
        controller.SetPlayerDestinationUsingTarget();

        // Assert
        Assert.AreEqual(new Vector3(10f, 9f, 8f), playerMover.Destination);
    }

    [Test]
    public void sets_destination_to_player_position_and_targeted_enemy_to_null_from_dead_target(){
        // Arrange
        IPlayerMover playerMover = CreateMockPlayerMoverWithDest(new Vector3(10f, 9f, 8f));
        IPlayer player = Substitute.For<IPlayer>();
        PlayerController controller = new PlayerController(playerMover, player);
        IUnit unit = Substitute.For<IUnit>();

        player.Position.Returns(new Vector3(20f, 15f, 10f));
        unit.IsDead.Returns(true);
        playerMover.TargetedEnemy.Returns(unit);

        // Act
        controller.SetPlayerDestinationUsingTarget();

        // Assert
        Assert.AreEqual(new Vector3(20f, 15f, 10f), playerMover.Destination);
    }

    [Test]
    public void sets_destination_to_player_position_from_player_less_than_max_range_from_target(){
        // Arrange
        IPlayerMover playerMover = CreateMockPlayerMoverWithDest(new Vector3(10f, 9f, 8f));
        IPlayer player = CreateMockPlayerWithRange(5.1f);
        PlayerController controller = new PlayerController(playerMover, player);
        IUnit unit = Substitute.For<IUnit>();
        unit.Position.Returns(new Vector3(5f, 5f, 2f));
        player.Position.Returns(new Vector3(10f, 5f, 2f));
        playerMover.TargetedEnemy.Returns(unit);

        // Act
        controller.SetPlayerDestinationUsingTarget();

        // Assert
        Assert.AreEqual(new Vector3(10f, 5f, 2f), playerMover.Destination);
    }

    [Test]
    public void sets_destination_to_target_enemy_position_from_player_at_max_range_from_target(){
        // Arrange
        IPlayerMover playerMover = CreateMockPlayerMoverWithDest(new Vector3(10f, 9f, 8f));
        IPlayer player = CreateMockPlayerWithRange(1f);
        PlayerController controller = new PlayerController(playerMover, player);
        IUnit unit = Substitute.For<IUnit>();
        unit.Position.Returns(new Vector3(4f, 9f, 3f));
        player.Position.Returns(new Vector3(4f, 8f, 3f));
        playerMover.TargetedEnemy.Returns(unit);

        // Act
        controller.SetPlayerDestinationUsingTarget();

        // Assert
        Assert.AreEqual(new Vector3(4f, 8f, 3f), playerMover.Destination);
    }

    [Test]
    public void sets_destination_to_target_enemy_position_from_player_out_of_range_of_target(){
        // Arrange
        IPlayerMover playerMover = CreateMockPlayerMoverWithDest(new Vector3(10f, 9f, 8f));
        IPlayer player = CreateMockPlayerWithRange(3.9f);
        PlayerController controller = new PlayerController(playerMover, player);
        IUnit unit = Substitute.For<IUnit>();
        unit.Position.Returns(new Vector3(14f, 2f, 6f));
        player.Position.Returns(new Vector3(14f, 2f, 10f));
        playerMover.TargetedEnemy.Returns(unit);

        // Act
        controller.SetPlayerDestinationUsingTarget();

        // Assert
        Assert.AreEqual(new Vector3(14f, 2f, 6f), playerMover.Destination);
    }

    [Test]
    public void sets_targeted_enemy_to_null_and_destination_to_player_position(){
        // Arrange
        IPlayerMover playerMover = CreateMockPlayerMoverWithDest(new Vector3(10f, 9f, 8f));
        IPlayer player = Substitute.For<IPlayer>();
        PlayerController controller = new PlayerController(playerMover, player);
        IUnit unit = Substitute.For<IUnit>();

        player.Position.Returns(new Vector3(5f, 4f, 3f)) ;
        playerMover.TargetedEnemy = unit;

        // Act
        controller.StopPlayer();

        // Assert
        Assert.AreEqual((nullUnit, new Vector3(5f, 4f, 3f)), (playerMover.TargetedEnemy, playerMover.Destination));
    }

    private IPlayer CreateMockPlayerWithRange(float range){
        IPlayer player = Substitute.For<IPlayer>();
        UnitStats unitStats = new UnitStats(ScriptableObject.CreateInstance<ScriptableUnit>());
        unitStats.autoRange.BaseValue = range;
        player.unitStats.Returns(unitStats);
        return player;
    }

    private IPlayerMover CreateMockPlayerMoverWithDest(Vector3 destination){
        IPlayerMover playerMover = Substitute.For<IPlayerMover>();
        playerMover.Destination = destination;
        return playerMover;
    }
}
