using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class targeted_projectile
{
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator destroys_game_object_with_target_set_and_null_unit()
    {
        // Arrange
        GameObject projectile = new GameObject("Projectile");
        TargetedProjectile script = projectile.AddComponent<TargetedProjectile>();
        MockPlayer player = new MockPlayer();
        script.TargetUnit = player;
        script.TargetUnit = null;
        
        // Act
        yield return null;

        // Assert
        Assert.True(projectile == null);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator destroys_game_object_with_target_set_and_target_dead()
    {
        // Arrange
        GameObject projectile = new GameObject("Projectile");
        TargetedProjectile targetedProjectile = projectile.AddComponent<TargetedProjectile>();
        MockPlayer player = new MockPlayer();
        player.IsDead = true;
        targetedProjectile.TargetUnit = player;
        
        // Act
        yield return new WaitForSeconds(0.01f);

        // Assert
        Assert.True(projectile == null);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator call_hit_method_from_projectile_and_target_colliding()
    {
        // Arrange
        GameObject projectile = new GameObject("Projectile");
        projectile.transform.position = Vector3.zero;
        BoxCollider collider = projectile.AddComponent<BoxCollider>();
        Rigidbody rb = projectile.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        collider.isTrigger = true;
        TargetedProjectile targetedProjectile = projectile.AddComponent<TargetedProjectile>();
        GameObject player = new GameObject("Target");
        player.transform.position = Vector3.zero;
        player.AddComponent<BoxCollider>();
        MockPlayerBehaviour playerBehaviour = player.AddComponent<MockPlayerBehaviour>();
        targetedProjectile.TargetUnit = playerBehaviour;
        player.AddComponent<Rigidbody>();

        // Act
        yield return new WaitForSeconds(0.01f);

        // Assert
        Assert.True(projectile == null);
    }
}
