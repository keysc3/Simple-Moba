using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Slow")]
public class ScriptableSlow : ScriptableEffect
{
    [field: SerializeField] public float duration { get; private set; }
    [field: SerializeField] public float slowPercent { get; private set; }

    /*
    *   InitializeEffect - Initializes a new slow effect with the duration and slow percent.
    *   @param unitCasted - GameObject of the unit that casted the slow.
    *   @param unitEffected - GameObject of the unit effected by the slow.
    */
    public Effect InitializeEffect(GameObject unitCasted, GameObject unitEffected){
        ccValue = 0;
        return new Slow(this, duration, unitCasted, unitEffected);
    }
}
