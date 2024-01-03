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
    public BilliaSpell3 billiaSpell3;
    public Transform casted;
    
    private bool hit = false;
    private int hitboxLayer;

    // Called when the script instance is being loaded.
    private void Awake(){
        this.enabled = false;
        hitboxLayer = LayerMask.NameToLayer("Hitbox");
    }

    // Called when the GameObject collides with an another GameObject.
    private void OnTriggerEnter(Collider other){
        // If a unit is hit.
        if(other.gameObject.layer == hitboxLayer && other.gameObject.tag != "Projectile" && !hit && other.transform.parent != casted && this.enabled == true){
            // Avoid same frame multi hits.
            hit = true;
            // Check for hits in a cone in the roll direction.
            billiaSpell3.Spell_3_ConeHitbox(transform, other.transform, forwardDirection);
            Destroy(transform.parent.gameObject);
        }
    }
}
