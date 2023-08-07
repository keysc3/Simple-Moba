using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableEffect for intializing a Slow effect.
*
* @author: Colin Keys
*/
[CreateAssetMenu(menuName = "Effects/Slow")]
public class ScriptableSlow : ScriptableEffect
{
    [field: SerializeField] public List<float> slowPercent { get; private set; } = new List<float>();
    [field: SerializeField] public bool isChild { get; private set; }

    /*
    *   InitializeEffect - Initializes a new slow effect with the duration and slow percent.
    *   @param unitCasted - GameObject of the unit that casted the slow.
    *   @param unitEffected - GameObject of the unit effected by the slow.
    */
    public Effect InitializeEffect(int spellLevel, GameObject unitCasted, GameObject unitEffected){
        ccValue = 0;
        return new Slow(this, duration[spellLevel], slowPercent[spellLevel], unitCasted, unitEffected);
    }
}
