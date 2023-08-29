using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Handles a basic attack GameObjects actions and collision.
*
* @author: Colin Keys
*/
public class BasicAttackTrigger : MonoBehaviour
{
    public BasicAttackController basicAttackController;
    public IUnit Target { get; set; }

    // Update is called once per frame
    private void Update()
    {
        // Destroy the attack object if the target dies or is destroyed.
        if(Target.IsDead){
            Destroy(gameObject);
        }
    }

    // Called when the GameObject collides with an another GameObject.
    private void OnTriggerEnter(Collider other){
        // If the attack has hit its target call the attack function.
        if(other.gameObject == Target.GameObject){
            basicAttackController.AttackHit(other.GetComponent<IUnit>());
            Destroy(gameObject);
        }
    }
}
