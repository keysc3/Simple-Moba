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
        if(other.gameObject.layer != groundLayer && other.gameObject.layer != projectileLayer && !hit){
            hit = true;
            Debug.Log("Hit on roll: " + other.gameObject.name);
            billiaAbilities.Spell_3_ConeHitbox(gameObject, forwardDirection);
            // TODO: Handle hit
            Destroy(gameObject);
        }
    }

}
