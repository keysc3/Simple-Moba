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
    *   SetBonusPercent - Sets the bonus percent of the speed bonus.
    *   @param bonusPercent - float of percent to set the bonus to.
    */
    public void SetBonusPercent(float bonusPercent){
        this.bonusPercent = bonusPercent;
    }
}
