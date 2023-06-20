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
    public BahriAbilityHit bahriAbilityHit;
    public GameObject bahri;

    private bool hit = false;

    // Called when the GameObject collides with an another GameObject.
    private void OnTriggerEnter(Collider other){
        // If the GamObject hits an enemy call the spell collision handler.
        // hit variable is to avoid two units getting charmed if the charm hits two objects on the same physics update.
        if(other.gameObject.tag == "Enemy" &&  other.gameObject != bahri && !hit){
            bahriAbilityHit.Spell_3_Hit(other.gameObject);
            hit = true;
            Destroy(gameObject);
        }
    }
}
