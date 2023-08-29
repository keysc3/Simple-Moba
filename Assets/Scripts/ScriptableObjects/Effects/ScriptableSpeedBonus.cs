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
    *   @param casted - IUnit of the unit that caused the effect.
    *   @param effected - IUnit of the unit that is being affected by the effect.
    */
    public Effect InitializeEffect(int spellLevel, IUnit casted, IUnit effected){
        ccValue = 0;
        return new SpeedBonus(this, duration[spellLevel], bonusPercent[spellLevel], casted, effected);
    }
}
