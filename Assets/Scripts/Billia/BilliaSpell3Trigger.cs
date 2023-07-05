using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Handles the collision checking and GameObject for Billia's third spell.
*
* @author: Colin Keys
*/
public class BilliaSpell3Trigger : MonoBehaviour
{
    public Vector3 forwardDirection;
    public BilliaAbilities billiaAbilities;
    public bool hasLanded;

    private bool hit = false;

    // Called when the GameObject collides with an another GameObject.
    private void OnTriggerEnter(Collider other){
        int groundLayer = LayerMask.NameToLayer("Ground");
        int projectileLayer = LayerMask.NameToLayer("Projectile");
        // If a unit is hit.
        if(other.gameObject.layer != groundLayer && other.gameObject.layer != projectileLayer && !hit && other.tag != "Player" && hasLanded){
            // Avoid same frame multi hits.
            hit = true;
            Debug.Log("Hit on roll: " + other.gameObject.name);
            // Check for hits in a cone in the roll direction.
            billiaAbilities.Spell_3_ConeHitbox(gameObject, other.gameObject, forwardDirection);
            Destroy(gameObject);
        }
    }

}
