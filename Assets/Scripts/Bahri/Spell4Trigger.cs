using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Handles the collision checking and GameObject for Bahri's second and fourth spell.
*
* @author: Colin Keys
*/
public class Spell4Trigger : MonoBehaviour
{
    public GameObject target { get; private set; } = null;
    public BahriSpell4 bahriSpell4 { get; private set; }

    private Unit targetUnit = null;

    // Update is called once per frame
    private void Update()
    {
        // Destroy GameObject if target dies.
        if(target != null){
            // Cache Unit component so it isn't being accessed every frame once a target is found.
            if(targetUnit == null)
                targetUnit = target.GetComponent<Unit>();
            else{
                if(targetUnit.isDead)
                    Destroy(gameObject);
            }
        }
    }

    // Called when the GameObject collides with an another GameObject.
    private void OnTriggerEnter(Collider other){
        // Call collision handler for whichever spell the GameObject was for if the target was hit.
        if(other.gameObject == target){
            bahriSpell4.Hit(other.gameObject);
            Destroy(gameObject);
        }
    }

    /*
    *   SetTarget - Sets the target for this GameObject.
    *   @param target - GameObject to set the target to.
    */
    public void SetTarget(GameObject target){
        this.target = target;
    }

    /*
    *   BahriAbilityHit - Sets the bahriAbilityHit script reference.
    *   @param bahriAbilityHit - BahriAbilityHit script reference.
    */
    public void SetBahriSpell4(BahriSpell4 bahriSpell4){
        this.bahriSpell4 = bahriSpell4;
    }
}
