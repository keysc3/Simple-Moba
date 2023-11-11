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

    private Bounds bahriBounds;
    public IUnit unit;
    private SphereCollider orbCollider;

    // Start is called before the first frame update.
    private void Start(){
        orbCollider = GetComponent<SphereCollider>();
        bahriBounds = unit.GameObject.GetComponent<Collider>().bounds;
    }

    // Update is called once per frame
    private void Update()
    {
        if(unit.IsDead){
            // If bahri is dead destroy the orb when it has returned to Bahri.
            if(bahriSpell1.returning){
                CheckContained();
            }
        }
    }

    // Called once per physics update for every Collider that is touching the trigger.
    private void OnTriggerStay(Collider other){
        // Call collision handler if enemy is hit.
        if(other.gameObject.tag == "Enemy" && other.gameObject.GetComponent<IUnit>() != unit){
            bahriSpell1.Hit(other.gameObject.GetComponent<IUnit>());
        }
        //  Destroy GameObject if it has returned to Bahri.
        if(other.gameObject.GetComponent<IUnit>() == unit && bahriSpell1.returning){
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
