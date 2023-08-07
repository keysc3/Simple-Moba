using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpeedBonus : Effect
{
    private float bonusPercent;
    public float BonusPercent { 
        get => bonusPercent;
        set {
            if(value >= 0)
                bonusPercent = value;
        }
    }

    /*
    *   Slow - Initialize a new speed bonus effect.
    *   @param slowEffect - ScriptableSpeedBonus of the speed bonus effect to apply.
    *   @param duration - float of the duration for the speed bonus to last.
    *   @param unitCasted - GameObject of the unit that casted the speed bonus.
    *   @param - unitEffected - GameObject of the unit that the speed bonus is affecting.
    */
    public SpeedBonus(ScriptableSpeedBonus speedBonusEffect, float duration, float bonusPercent, GameObject unitCasted, GameObject unitEffected) : base(speedBonusEffect, duration, unitCasted, unitEffected){
        this.bonusPercent = bonusPercent;
    }
}
