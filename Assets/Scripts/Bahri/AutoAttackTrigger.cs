using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Detects when Bahri's auto attack collides with its target.
*
* @author: Colin Keys
*/
public class AutoAttackTrigger : MonoBehaviour
{
    public GameObject target = null;
    public BahriAbilityHit bahriAbilityHit;

    // Update is called once per frame
    private void Update()
    {
        // Destroy the attack object if the target dies.
        if(target != null){
            if(target.GetComponent<UnitStats>().isDead){
                Destroy(gameObject);
            }
        }
    }

    // Called when the GameObject collides with an another GameObject.
    private void OnTriggerEnter(Collider other){
        // If the attack has hit its target call the attack function.
        if(other.gameObject == target){
            bahriAbilityHit.AutoAttack(other.gameObject);
            Destroy(gameObject);
        }
    }
}
