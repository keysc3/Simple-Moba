using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableEffect for intializing a Drowsy effect.
*
* @author: Colin Keys
*/
[CreateAssetMenu(menuName = "Effects/Drowsy")]
public class ScriptableDrowsy : ScriptableEffect
{
    //[field: SerializeField] public float duration { get; private set; }
    [field: SerializeField] public ScriptableSlow slow { get; private set; }
    [field: SerializeField] public ScriptableSleep sleep { get; private set; }

    /*
    *   InitializeEffect - Initializes a new drowsy effect.
    *   @param unitCasted - GameObject of the unit that casted the charm.
    *   @param unitEffected - GameObject of the unit effected by the charm.
    */
    public Effect InitializeEffect(int spellLevel, GameObject unitCasted, GameObject unitEffected){
        ccValue = 0;
        //this.sleep = sleep;
        //slow.SetDuration(duration);
        return new Drowsy(this, duration[spellLevel], spellLevel, unitCasted, unitEffected);
    }
}
