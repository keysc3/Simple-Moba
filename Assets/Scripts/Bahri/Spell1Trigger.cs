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
    public bool isReturning { get; private set; } = false;
    public BahriAbilityHit bahriAbilityHit { get; private set; }
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
            if(isReturning){
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
            bahriAbilityHit.Spell_1_Hit(other.gameObject, isReturning);
        }
        //  Destroy GameObject if it has returned to Bahri.
        if(other.gameObject == bahri && isReturning){
            if(other.bounds.Contains(orbCollider.bounds.min) && other.bounds.Contains(orbCollider.bounds.max)){
                Destroy(gameObject);
            }
        }
    }

    /*
    *   SetIsReturning - Sets the isReturning bool
    *   @param isReturning - bool to set isReturning to.
    */
    public void SetIsReturning(bool isReturning){
        this.isReturning = isReturning;
    }

    /*
    *   BahriAbilityHit - Sets the bahriAbilityHit script reference.
    *   @param bahriAbilityHit - BahriAbilityHit script reference.
    */
    public void SetBahriAbilityHit(BahriAbilityHit bahriAbilityHit){
        this.bahriAbilityHit = bahriAbilityHit;
    }

    /*
    *   SetBahri - Sets the GameObject that casted the spell.
    *   @param bahri - GameObject of the caster.
    */
    public void SetBahri(GameObject bahri){
        this.bahri = bahri;
    }
}
