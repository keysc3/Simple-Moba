using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableEffect for initializing a Drowsy effect.
*
* @author: Colin Keys
*/
[CreateAssetMenu(menuName = "Effects/Drowsy")]
public class ScriptableDrowsy : ScriptableEffect
{
    public ScriptableSlow slow;
    public ScriptableSleep sleep;

    /*
    *   InitializeEffect - Initializes a new drowsy effect.
    *   @param unitCasted - GameObject of the unit that casted the charm.
    *   @param unitEffected - GameObject of the unit effected by the charm.
    */
    public Effect InitializeEffect(int spellLevel, GameObject unitCasted, GameObject unitEffected){
        ccValue = 0;
        return new Drowsy(this, duration[spellLevel], spellLevel, unitCasted, unitEffected);
    }
}
