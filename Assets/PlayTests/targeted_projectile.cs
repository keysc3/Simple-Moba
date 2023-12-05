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
        GameObject hitbox = new GameObject("Hitbox");
        hitbox.transform.SetParent(projectile.transform);
        TargetedProjectile script = hitbox.AddComponent<TargetedProjectile>();
        MockPlayer player = new MockPlayer();
        script.TargetUnit = player;
        script.TargetUnit = null;
        
        // Act
        yield return new WaitForSeconds(0.01f);

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
        GameObject hitbox = new GameObject("Hitbox");
        hitbox.transform.SetParent(projectile.transform);
        TargetedProjectile targetedProjectile = hitbox.AddComponent<TargetedProjectile>();
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
        GameObject hitbox = new GameObject("Hitbox");
        BoxCollider collider = hitbox.AddComponent<BoxCollider>();
        Rigidbody rb = hitbox.AddComponent<Rigidbody>();
        hitbox.transform.SetParent(projectile.transform);
        hitbox.transform.position = Vector3.zero;
        rb.isKinematic = true;
        collider.isTrigger = true;
        TargetedProjectile targetedProjectile = hitbox.AddComponent<TargetedProjectile>();
        GameObject player = new GameObject("Target");
        GameObject playerHitbox = new GameObject("Hitbox");
        player.transform.position = Vector3.zero;
        playerHitbox.transform.SetParent(player.transform);
        playerHitbox.transform.position = Vector3.zero;
        playerHitbox.AddComponent<BoxCollider>();
        MockPlayerBehaviour playerBehaviour = player.AddComponent<MockPlayerBehaviour>();
        targetedProjectile.TargetUnit = playerBehaviour;
        playerHitbox.AddComponent<Rigidbody>();

        // Act
        yield return new WaitForSeconds(0.1f);

        // Assert
        Assert.True(projectile == null);
    }
}
