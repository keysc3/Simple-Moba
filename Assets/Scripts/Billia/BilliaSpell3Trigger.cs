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
    public BilliaSpell3 billiaSpell3 { get; private set; }
    public GameObject casted { get; private set; }
    private bool hit = false;
    private int groundLayer;
    private int projectileLayer;

    // Called when the script instance is being loaded.
    private void Awake(){
        this.enabled = false;
        groundLayer = LayerMask.NameToLayer("Ground");
        projectileLayer = LayerMask.NameToLayer("Projectile");
    }

    // Called when the GameObject collides with an another GameObject.
    private void OnTriggerEnter(Collider other){
        // If a unit is hit.
        if(other.gameObject.layer != groundLayer && other.gameObject.layer != projectileLayer && !hit && other.gameObject != gameObject && this.enabled == true){
            // Avoid same frame multi hits.
            hit = true;
            Debug.Log("Hit on roll: " + other.gameObject.name);
            // Check for hits in a cone in the roll direction.
            billiaSpell3.Spell_3_ConeHitbox(gameObject, other.gameObject, forwardDirection);
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
    public void SetBilliaSpell3Script(BilliaSpell3 billiaSpell3){
        this.billiaSpell3 = billiaSpell3;
    }
}
