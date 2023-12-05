using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*   Purpose: Implements a targeted GameObject.
*
*   @author: Colin Keys
*/
public class TargetedProjectile : MonoBehaviour
{
    private IUnit targetUnit = null;
    public IUnit TargetUnit {
        get => targetUnit;
        set {
            targetUnit = value;
            targetSet = true;
        }
    }
    private bool targetSet = false;

    public delegate void HitMethod(IUnit hit);
    public HitMethod hit;

    // Update is called once per frame
    private void Update()
    {
        // Destroy GameObject if target dies.
        if((targetUnit == null || targetUnit.IsDead) && targetSet == true){
            Destroy(transform.parent.gameObject);
        }
    }

    // Called when the GameObject collides with an another GameObject.
    private void OnTriggerEnter(Collider other){
        IUnit otherUnit = other.gameObject.GetComponentInParent<IUnit>();
        // Call collision handler for whichever spell the GameObject was for if the target was hit.
        if(otherUnit == targetUnit){
            hit?.Invoke(otherUnit);
            Destroy(transform.parent.gameObject);
        }
    }
}
