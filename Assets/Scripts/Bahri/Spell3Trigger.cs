using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Handles the collision checking and GameObject for Bahri's third spell.
*
* @author: Colin Keys
*/
public class Spell3Trigger : MonoBehaviour
{
    public BahriAbilityHit bahriAbilityHit { get; private set; }
    public GameObject bahri { get; private set; }

    private bool hit = false;

    // Called when the GameObject collides with an another GameObject.
    private void OnTriggerEnter(Collider other){
        // If the GameObject hits an enemy call the spell collision handler.
        // hit variable is to avoid two units getting charmed if the charm hits two objects on the same physics update.
        if(other.gameObject.tag == "Enemy" &&  other.gameObject != bahri && !hit){
            bahriAbilityHit.Spell_3_Hit(other.gameObject);
            hit = true;
            Destroy(gameObject);
        }
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
