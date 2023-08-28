using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableEffect for initializing a SpeedBonus effect.
*
* @author: Colin Keys
*/
[CreateAssetMenu(menuName = "Effects/SpeedBonus")]
public class ScriptableSpeedBonus : ScriptableEffect
{
    [field: SerializeField] public List<float> bonusPercent { get; private set; } = new List<float>();
    public bool isAdditive;

    /*
    *   InitializeEffect - Initializes a new speed bonus effect with the duration and bonus percent.
    *   @param unitCasted - GameObject of the unit that casted the speed bonus.
    *   @param unitEffected - GameObject of the unit effected by the speed bonus.
    */
    public Effect InitializeEffect(int spellLevel, GameObject unitCasted, GameObject unitEffected){
        ccValue = 0;
        return new SpeedBonus(this, duration[spellLevel], bonusPercent[spellLevel], unitCasted, unitEffected);
    }
}
