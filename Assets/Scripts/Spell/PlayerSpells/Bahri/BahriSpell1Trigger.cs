using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Handless the GameObject collision actions for Bahri's spell 1.
*
* @author: Colin Keys
*/
public class BahriSpell1Trigger : MonoBehaviour
{
    public BahriSpell1 bahriSpell1;
    public IUnit unit;
    private Bounds bahriBounds;
    private SphereCollider orbCollider;
    private bool deathBoundsSet = false;
    private LayerMask hitboxLayer;

    // Called when the script instance is being loaded.
    private void Awake(){
        hitboxLayer = LayerMask.NameToLayer("Hitbox");
    }
    
    // Start is called before the first frame update.
    private void Start(){
        orbCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Store current bounds. If unit died set bounds once.
        if(!unit.IsDead){
            bahriBounds = unit.hitbox.bounds;
        }
        else{
            if(!deathBoundsSet){
                unit.hitbox.enabled = true;
                bahriBounds = unit.hitbox.bounds;
                unit.hitbox.enabled = false;
                deathBoundsSet = true;
            }
            // If bahri is dead destroy the orb when it has returned to Bahri.
            if(bahriSpell1.returning){
                CheckContained();
            }
        }
    }

    // Called once per physics update for every Collider that is touching the trigger.
    private void OnTriggerStay(Collider other){
        IUnit hitUnit = other.gameObject.GetComponentInParent<IUnit>();
        // Call collision handler if enemy is hit.
        if(other.gameObject.layer == hitboxLayer && other.transform.parent.tag == "Enemy" && hitUnit != unit){
            bahriSpell1.Hit(hitUnit);
        }
        //  Destroy GameObject if it has returned to Bahri.
        if(hitUnit == unit && bahriSpell1.returning){
            CheckContained();
        }
    }

    /*
    *   CheckContained - Checks if the orb is contained within Bahri.
    */
    private void CheckContained(){
        Vector3 min = orbCollider.bounds.min;
        Vector3 max = orbCollider.bounds.max;
        if(bahriBounds.Contains(new Vector3(min.x, bahriBounds.center.y, min.z)) && bahriBounds.Contains(new Vector3(max.x, bahriBounds.center.y, max.z))){
            Destroy(transform.parent.gameObject);
        } 
    }
}
