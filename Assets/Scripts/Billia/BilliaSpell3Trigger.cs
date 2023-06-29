using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaSpell3Trigger : MonoBehaviour
{

    private bool hit = false;
    public BilliaAbilities billiaAbilities;
    public Vector3 forwardDirection;

    // Called when the GameObject collides with an another GameObject.
    private void OnTriggerEnter(Collider other){
        int groundLayer = LayerMask.NameToLayer("Ground");
        int projectileLayer = LayerMask.NameToLayer("Projectile");
        // If a unit is hit.
        if(other.gameObject.layer != groundLayer && other.gameObject.layer != projectileLayer && !hit){
            // Avoid same frame multi hits.
            hit = true;
            Debug.Log("Hit on roll: " + other.gameObject.name);
            // TODO: Handle collision hit.
            // Check for hits in a cone in the roll direction.
            billiaAbilities.Spell_3_ConeHitbox(gameObject, other.gameObject, forwardDirection);
            Destroy(gameObject);
        }
    }

}
