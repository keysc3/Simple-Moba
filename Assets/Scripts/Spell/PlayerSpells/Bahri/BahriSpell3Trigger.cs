using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Handless the GameObject collision actions for Bahri's spell 3.
*
* @author: Colin Keys
*/
public class BahriSpell3Trigger : MonoBehaviour
{
    public IHasHit hitMethod;
    public IUnit caster;
    private LayerMask hitboxLayer;

    private bool hit = false;

    // Called when the script instance is being loaded.
    private void Awake(){
        hitboxLayer = LayerMask.NameToLayer("Hitbox");
    }

    // Called when the GameObject collides with an another GameObject.
    private void OnTriggerEnter(Collider other){
        IUnit unitHit = other.transform.GetComponentInParent<IUnit>();
        // If the GameObject hits an enemy call the spell collision handler.
        // hit variable is to avoid two units getting charmed if the charm hits two objects on the same physics update.
        if(other.gameObject.layer == hitboxLayer && other.transform.parent.tag == "Enemy" && unitHit != null &&  unitHit != caster && !hit){
            hitMethod.Hit(unitHit);
            hit = true;
            Destroy(transform.parent.gameObject);
        }
    }
}
