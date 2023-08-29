using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class player_controller
{
    private IUnit nullUnit = null;

    [Test]
    public void set_target_enemy_and_destination_from_valid_right_click_with_no_unit(){
        // Arrange
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(playerMover, player);

        Vector3 targetVec = new Vector3(-12f, 96f, 93f);

        //Act
        pc.RightClick(null, targetVec);

        // Assert
        Assert.AreEqual((nullUnit, new Vector3(-12f, 96f, 93f)), (playerMover.TargetedEnemy, playerMover.Destination));
    }

    [Test]
    public void set_target_enemy_from_valid_right_click_on_enemy_unit(){
        // Arrange
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(playerMover, player);

        MockUnit target = new MockUnit();
        target.GameObject.tag = "Enemy";
        Vector3 targetVec = new Vector3(6f, 31f, -54f);

        // Act
        pc.RightClick(target, targetVec);

        // Assert
        Assert.AreEqual(target, playerMover.TargetedEnemy);
    }

    [Test]
    public void set_target_enemy_and_destination_from_right_click_on_non_enemy_unit(){
        // Arrange
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(playerMover, player);

        MockUnit target = new MockUnit();

        Vector3 targetVec = new Vector3(10f, 3f, 5f);

        // Act
        pc.RightClick(target, targetVec);

        // Assert
        Assert.AreEqual((nullUnit, new Vector3(10f, 3f, 5f)), (playerMover.TargetedEnemy, playerMover.Destination));
    }
    
    [Test]
    public void set_target_and_destination_from_right_click_on_dead_enemy_unit(){
        // Arrange
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(playerMover, player);

        MockUnit target = new MockUnit();
        target.GameObject.tag = "Enemy";
        target.IsDead = true;
        Vector3 targetVec = new Vector3(76f, 67f, 767f);

        // Act
        pc.RightClick(target, targetVec);

        // Assert
        Assert.AreEqual((nullUnit, new Vector3(76f, 67f, 767f)), (playerMover.TargetedEnemy, playerMover.Destination));
    }

    [Test]
    public void set_player_look_direction_to_mouse_on_cast(){
        // Arrange
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(playerMover, player);
        MockSpell spell = new MockSpell();

        spell.CanMove = false;
        player.MouseOnCast = new Vector3(1f, 2f, 3f);
        player.IsCasting = true;
        player.CurrentCastedSpell = spell;

        // Act
        pc.PlayerLookDirection();

        // Assert
        Assert.AreEqual(new Vector3(1f, 2f, 3f), playerMover.CurrentTarget);
    }

    [Test]
    public void set_player_look_direction_to_next_destination_from_is_casting_false(){
        // Arrange
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(playerMover, player);
        MockSpell spell = new MockSpell();

        playerMover.NextDestination = new Vector3(3f, 2f, 1f);
        spell.CanMove = false;
        player.MouseOnCast = new Vector3(1f, 2f, 3f);
        player.IsCasting = false;
        player.CurrentCastedSpell = spell;

        // Act
        pc.PlayerLookDirection();

        // Assert
        Assert.AreEqual(new Vector3(3f, 2f, 1f), playerMover.CurrentTarget);
    }

    [Test]
    public void set_player_look_direction_to_next_destination_from_current_spell_null(){
        // Arrange
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(playerMover, player);

        playerMover.NextDestination = new Vector3(3f, 2f, 1f);
        player.MouseOnCast = new Vector3(1f, 2f, 3f);
        player.IsCasting = true;
        player.CurrentCastedSpell = null;

        // Act
        pc.PlayerLookDirection();

        // Assert
        Assert.AreEqual(new Vector3(3f, 2f, 1f), playerMover.CurrentTarget);
    }

    [Test]
    public void set_player_look_direction_to_next_destination_from_current_spell_can_move_true(){
        // Arrange
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(playerMover, player);
        MockSpell spell = new MockSpell();

        playerMover.NextDestination = new Vector3(3f, 2f, 1f);
        spell.CanMove = true;
        player.MouseOnCast = new Vector3(1f, 2f, 3f);
        player.IsCasting = true;
        player.CurrentCastedSpell = spell;

        // Act
        pc.PlayerLookDirection();

        // Assert
        Assert.AreEqual(new Vector3(3f, 2f, 1f), playerMover.CurrentTarget);
    }
    
    [Test]
    public void does_not_set_destination_from_null_target_enemy(){
        // Arrange
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(playerMover, player);

        playerMover.Destination = new Vector3(10f, 9f, 8f);

        // Act
        pc.MovePlayerToEnemy();

        // Assert
        Assert.AreEqual(new Vector3(10f, 9f, 8f), playerMover.Destination);
    }

    [Test]
    public void does_not_set_destination_from_is_casting_true(){
        // Arrange
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(playerMover, player);
        MockUnit unit = new MockUnit();

        player.IsCasting = true;
        playerMover.TargetedEnemy = unit;
        playerMover.Destination = new Vector3(10f, 9f, 8f);

        // Act
        pc.MovePlayerToEnemy();

        // Assert
        Assert.AreEqual(new Vector3(10f, 9f, 8f), playerMover.Destination);
    }

    [Test]
    public void sets_destination_to_player_position_and_targeted_enemy_to_null_from_dead_target(){
        // Arrange
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(playerMover, player);
        MockUnit unit = new MockUnit();

        player.Position = new Vector3(20f, 15f, 10f);
        unit.IsDead = true;
        playerMover.TargetedEnemy = unit;
        playerMover.Destination = new Vector3(10f, 9f, 8f);

        // Act
        pc.MovePlayerToEnemy();

        // Assert
        Assert.AreEqual(new Vector3(20f, 15f, 10f), playerMover.Destination);
    }

    [Test]
    public void sets_destination_to_player_position_from_player_less_than_max_range_from_target(){
        // Arrange
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(playerMover, player);
        MockUnit unit = new MockUnit();

        unit.Position = new Vector3(5f, 5f, 2f);
        player.unitStats = new UnitStats(player.SUnit);
        player.unitStats.autoRange.BaseValue = 5.1f;
        player.Position = new Vector3(10f, 5f, 2f);
        playerMover.TargetedEnemy = unit;
        playerMover.Destination = new Vector3(10f, 9f, 8f);

        // Act
        pc.MovePlayerToEnemy();

        // Assert
        Assert.AreEqual(new Vector3(10f, 5f, 2f), playerMover.Destination);
    }

    [Test]
    public void sets_destination_to_target_enemy_position_from_player_at_max_range_from_target(){
        // Arrange
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(playerMover, player);
        MockUnit unit = new MockUnit();

        unit.Position = new Vector3(4f, 9f, 3f);
        player.unitStats = new UnitStats(player.SUnit);
        player.unitStats.autoRange.BaseValue = 1f;
        player.Position = new Vector3(4f, 8f, 3f);
        playerMover.TargetedEnemy = unit;
        playerMover.Destination = new Vector3(10f, 9f, 8f);

        // Act
        pc.MovePlayerToEnemy();

        // Assert
        Assert.AreEqual(new Vector3(4f, 8f, 3f), playerMover.Destination);
    }

    [Test]
    public void sets_destination_to_target_enemy_position_from_player_out_of_range_of_target(){
        // Arrange
        MockPlayerMover playerMover = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(playerMover, player);
        MockUnit unit = new MockUnit();

        unit.Position = new Vector3(14f, 2f, 6f);
        player.unitStats = new UnitStats(player.SUnit);
        player.unitStats.autoRange.BaseValue = 3.9f;
        player.Position = new Vector3(14f, 2f, 10f);
        playerMover.TargetedEnemy = unit;
        playerMover.Destination = new Vector3(10f, 9f, 8f);

        // Act
        pc.MovePlayerToEnemy();

        // Assert
        Assert.AreEqual(new Vector3(14f, 2f, 6f), playerMover.Destination);
    }
}
