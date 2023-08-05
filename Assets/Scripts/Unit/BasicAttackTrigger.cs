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
    public BasicAttack basicAttack;
    private Unit targetUnit;
    private GameObject target = null;
    #region "Target property"
    public GameObject Target {
        get { 
            return target;
        }
        set {
            if((value.GetComponent<Unit>() as Unit) != null){
                target = value;
                targetUnit = value.GetComponent<Unit>();
            }
        }
    }
    #endregion

    // Update is called once per frame
    private void Update()
    {
        // Destroy the attack object if the target dies or is destroyed.
        if(targetUnit == null || targetUnit.isDead){
            Destroy(gameObject);
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
}
