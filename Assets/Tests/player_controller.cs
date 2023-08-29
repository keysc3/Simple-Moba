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
}
