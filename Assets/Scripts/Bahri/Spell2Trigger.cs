using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Handles the collision checking and GameObject for Bahri's second and fourth spell.
*
* @author: Colin Keys
*/
public class Spell2Trigger : MonoBehaviour
{
    public int spellCast;
    public GameObject target = null;
    public BahriAbilityHit bahriAbilityHit;

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
            if(spellCast == 2)
                bahriAbilityHit.Spell_2_Hit(other.gameObject);
            else
                bahriAbilityHit.Spell_4_Hit(other.gameObject);
            Destroy(gameObject);
        }
    }
}
