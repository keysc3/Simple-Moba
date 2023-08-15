using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBasicAttackTrigger : MonoBehaviour
{
    public NewBasicAttack nba;
    private Unit targetUnit;
    private GameObject target = null;
    public GameObject Target {
        get => target;
        set {
            if((value.GetComponent<Unit>() as Unit) != null){
                target = value;
                targetUnit = value.GetComponent<Unit>();
            }
        }
    }

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
            nba.AttackHit(other.gameObject);
            Destroy(gameObject);
        }
    }
}
