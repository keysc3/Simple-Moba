using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBilliaSpell3Trigger : MonoBehaviour
{
    public Vector3 forwardDirection;
    public BilliaSpell3 billiaSpell3;
    public GameObject casted;
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
        if(other.gameObject.layer != groundLayer && other.gameObject.layer != projectileLayer && !hit && other.gameObject != casted && this.enabled == true){
            // Avoid same frame multi hits.
            hit = true;
            Debug.Log("Hit on roll: " + other.gameObject.name);
            // Check for hits in a cone in the roll direction.
            billiaSpell3.Spell_3_ConeHitbox(gameObject, other.gameObject, forwardDirection);
            Destroy(gameObject);
        }
    }
}
