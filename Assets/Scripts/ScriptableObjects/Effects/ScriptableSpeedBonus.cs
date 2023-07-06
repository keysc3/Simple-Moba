using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableEffect for intializing a SpeedBonus effect.
*
* @author: Colin Keys
*/
[CreateAssetMenu(menuName = "Effects/SpeedBonus")]
public class ScriptableSpeedBonus : ScriptableEffect
{
    [field: SerializeField] public float duration { get; private set; }
    [field: SerializeField] public float bonusPercent { get; private set; }
    [field: SerializeField] public bool isAdditive { get; private set; }

    /*
    *   InitializeEffect - Initializes a new speed bonus effect with the duration and bonus percent.
    *   @param unitCasted - GameObject of the unit that casted the speed bonus.
    *   @param unitEffected - GameObject of the unit effected by the speed bonus.
    */
    public Effect InitializeEffect(GameObject unitCasted, GameObject unitEffected){
        ccValue = 0;
        return new SpeedBonus(this, duration, unitCasted, unitEffected);
    }

    /*
    *   SetDuration - Sets the slows duration.
    *   @param duration - float of duration to set slow to.
    */
    public void SetDuration(float duration){
        this.duration = duration;
    }
}
