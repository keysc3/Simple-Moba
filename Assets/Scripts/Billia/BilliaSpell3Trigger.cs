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
    public Vector3 forwardDirection { get; private set; }
    public BilliaAbilities billiaAbilities { get; private set; }
    public bool hasLanded { get; private set; }
    public GameObject casted { get; private set; }
    private bool hit = false;

    // Called when the GameObject collides with an another GameObject.
    private void OnTriggerEnter(Collider other){
        int groundLayer = LayerMask.NameToLayer("Ground");
        int projectileLayer = LayerMask.NameToLayer("Projectile");
        // If a unit is hit.
        if(other.gameObject.layer != groundLayer && other.gameObject.layer != projectileLayer && !hit && other.gameObject != gameObject && hasLanded){
            // Avoid same frame multi hits.
            hit = true;
            Debug.Log("Hit on roll: " + other.gameObject.name);
            // Check for hits in a cone in the roll direction.
            billiaAbilities.Spell_3_ConeHitbox(gameObject, other.gameObject, forwardDirection);
            Destroy(gameObject);
        }
    }

    /*
    *   SetCaster - Sets the GameObject that created this object.
    *   @param casted - GameObject of the game object creator.
    */
    public void SetCaster(GameObject casted){
        this.casted = casted;
    }

    /*
    *   SetForwardDirection - Sets the forward direction of the seed.
    *   @param forwardDirection - Vector3 of the seeds forward direction.
    */
    public void SetForwardDirection(Vector3 forwardDirection){
        this.forwardDirection = forwardDirection;
    }

    /*
    *   SetBilliaAbilitiesScript - Sets the BilliaAbilities script.
    *   @param billiaAbilities - BilliaAbilities script to use.
    */
    public void SetBilliaAbilitiesScript(BilliaAbilities billiaAbilities){
        this.billiaAbilities = billiaAbilities;
    }

    /*
    *   SetHasLanded - Sets the hasLanded bool
    *   @param hasLanded - bool to set hasLanded to.
    */
    public void SetHasLanded(bool hasLanded){
        this.hasLanded = hasLanded;
    }
}
