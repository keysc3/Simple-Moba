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
        MockPlayerMover pm = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(pm, player);

        Vector3 targetVec = new Vector3(-12f, 96f, 93f);

        //Act
        pc.RightClick(null, targetVec);

        // Assert
        Assert.AreEqual((nullUnit, new Vector3(-12f, 96f, 93f)), (pm.TargetedEnemy, pm.Destination));
    }

    [Test]
    public void set_target_enemy_from_valid_right_click_on_enemy_unit(){
        // Arrange
        MockPlayerMover pm = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(pm, player);

        MockUnit target = new MockUnit();
        target.GameObject.tag = "Enemy";
        Vector3 targetVec = new Vector3(6f, 31f, -54f);

        // Act
        pc.RightClick(target, targetVec);

        // Assert
        Assert.AreEqual(target, pm.TargetedEnemy);
    }

    [Test]
    public void set_target_enemy_and_destination_from_right_click_on_non_enemy_unit(){
        // Arrange
        MockPlayerMover pm = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(pm, player);

        MockUnit target = new MockUnit();

        Vector3 targetVec = new Vector3(10f, 3f, 5f);

        // Act
        pc.RightClick(target, targetVec);

        // Assert
        Assert.AreEqual((nullUnit, new Vector3(10f, 3f, 5f)), (pm.TargetedEnemy, pm.Destination));
    }
    
    [Test]
    public void set_target_and_destination_from_right_click_on_dead_enemy_unit(){
        // Arrange
        MockPlayerMover pm = new MockPlayerMover();
        MockPlayer player = new MockPlayer();
        PlayerController pc = new PlayerController(pm, player);

        MockUnit target = new MockUnit();
        target.GameObject.tag = "Enemy";
        target.IsDead = true;
        Vector3 targetVec = new Vector3(76f, 67f, 767f);

        // Act
        pc.RightClick(target, targetVec);

        // Assert
        Assert.AreEqual((nullUnit, new Vector3(76f, 67f, 767f)), (pm.TargetedEnemy, pm.Destination));
    }

    /*[Test]
    public void valid_right_click_on_a_unit()
    {
        MockPlayerMover pm = new MockPlayerMover();
        PlayerController pc = new PlayerController(pm);
        GameObject g1 = new GameObject();
        g1.tag = "Enemy";
        g1.AddComponent<CapsuleCollider>();
        Vector3 targ = new Vector3(10f, 3f, 5f);

        pc.RightClick(g1.GetComponent<Collider>(), targ);

        Assert.AreEqual(g1, pm.TargetedEnemy);
        // Use the Assert class to test conditions
    }*/
}
