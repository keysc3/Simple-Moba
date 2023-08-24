using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BahriSpell3Trigger : MonoBehaviour
{
    public IHasHit hitMethod;
    public IUnit caster;

    private bool hit = false;

    // Called when the GameObject collides with an another GameObject.
    private void OnTriggerEnter(Collider other){
        // If the GameObject hits an enemy call the spell collision handler.
        // hit variable is to avoid two units getting charmed if the charm hits two objects on the same physics update.
        if(other.gameObject.tag == "Enemy" &&  other.gameObject.GetComponent<IUnit>() != null &&  other.gameObject.GetComponent<IUnit>() != caster && !hit){
            hitMethod.Hit(other.gameObject.GetComponent<IUnit>());
            hit = true;
            Destroy(gameObject);
        }
    }
}
