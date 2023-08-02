using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Detects when a basic attack collides with its target.
*
* @author: Colin Keys
*/
public class BasicAttackTrigger : MonoBehaviour
{
    public GameObject target { get; private set; } = null;
    public BasicAttack basicAttack { get; private set; }

    // Update is called once per frame
    private void Update()
    {
        // Destroy the attack object if the target dies.
        if(target != null){
            if(target.GetComponent<Unit>().isDead){
                Destroy(gameObject);
            }
        }
    }

    // Called when the GameObject collides with an another GameObject.
    private void OnTriggerEnter(Collider other){
        // If the attack has hit its target call the attack function.
        if(other.gameObject == target){
            basicAttack.AttackHit(other.gameObject);
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
    *   BasicAttack - Sets the basicAttack script reference.
    *   @param basicAttack - basicAttack script reference.
    */
    public void SetBasicAttack(BasicAttack basicAttack){
        this.basicAttack = basicAttack;
    }
}
