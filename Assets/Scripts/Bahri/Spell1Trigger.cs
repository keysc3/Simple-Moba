using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Handles the collision checking and GameObject for Bahri's first spell.
*
* @author: Colin Keys
*/
public class Spell1Trigger : MonoBehaviour
{
    public BahriSpell1 bahriSpell1 { get; private set; }
    public GameObject bahri { get; private set; }

    private Bounds bahriBounds;

    private Unit unit;
    private Collider unitCollider;
    private SphereCollider orbCollider;

    private void Start(){
        unit = bahri.GetComponent<Unit>();
        unitCollider = bahri.GetComponent<Collider>();
        orbCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Get Bahri's bounds for when they die.
        if(!unit.isDead)
            bahriBounds = unitCollider.bounds;
        // If bahri is dead destroy the orb when it has returned to Bahri.
        else{
            if(bahriSpell1.returning){
                if(bahriBounds.Contains(orbCollider.bounds.min) && bahriBounds.Contains(orbCollider.bounds.max)){
                    Destroy(gameObject);
                }
            }
        }
    }

    // Called once per physics update for every Collider that is touching the trigger.
    private void OnTriggerStay(Collider other){
        // Call collision handler if enemy is hit.
        if(other.gameObject.tag == "Enemy" && other.gameObject != bahri){
            bahriSpell1.Hit(other.gameObject);
        }
        //  Destroy GameObject if it has returned to Bahri.
        if(other.gameObject == bahri && bahriSpell1.returning){
            if(other.bounds.Contains(orbCollider.bounds.min) && other.bounds.Contains(orbCollider.bounds.max)){
                Destroy(gameObject);
            }
        }
    }

    /*
    *   BahriAbilityHit - Sets the bahriAbilityHit script reference.
    *   @param bahriAbilityHit - BahriAbilityHit script reference.
    */
    public void SetBahriSpell1(BahriSpell1 bahriSpell1){
        this.bahriSpell1 = bahriSpell1;
    }

    /*
    *   SetBahri - Sets the GameObject that casted the spell.
    *   @param bahri - GameObject of the caster.
    */
    public void SetBahri(GameObject bahri){
        this.bahri = bahri;
    }
}
