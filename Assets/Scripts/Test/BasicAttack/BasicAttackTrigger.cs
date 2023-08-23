using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackTrigger : MonoBehaviour
{
    public BasicAttack nba;
    private IUnit targetUnit;
    private GameObject target = null;
    public GameObject Target {
        get => target;
        set {
            if((value.GetComponent<IUnit>() as IUnit) != null){
                target = value;
                targetUnit = value.GetComponent<IUnit>();
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Destroy the attack object if the target dies or is destroyed.
        if(targetUnit == null || targetUnit.IsDead){
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
