using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Handles the collision checking and GameObject for Bahri's second and fourth spell.
*
* @author: Colin Keys
*/
public class TargetedProjectile : MonoBehaviour
{
    public GameObject target { get; private set; } = null;

    private Unit targetUnit = null;

    public delegate void HitMethod(GameObject hit);
    public HitMethod hit;

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
            hit?.Invoke(other.gameObject);
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
}
