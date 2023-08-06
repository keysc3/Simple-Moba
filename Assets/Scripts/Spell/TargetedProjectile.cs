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
    private GameObject target = null;
    public GameObject Target {
        get => target;
        set {
            if((value.GetComponent<Unit>() as Unit) != null){
                target = value;
                targetSet = true;
                targetUnit = target.GetComponent<Unit>();
            }
        }
    }
    private Unit targetUnit = null;
    private bool targetSet = false;

    public delegate void HitMethod(GameObject hit);
    public HitMethod hit;

    // Update is called once per frame
    private void Update()
    {
        // Destroy GameObject if target dies.
        if((targetUnit == null || targetUnit.isDead) && targetSet == true){
            Destroy(gameObject);
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
}
