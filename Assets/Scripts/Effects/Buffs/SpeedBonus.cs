using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpeedBonus : Effect
{
    private float bonusPercent;
    #region "Bonus Property"
    public float BonusPercent { 
        get {
            return bonusPercent;
        }
        set {
            if(value >= 0)
                bonusPercent = value;
        }
    }
    #endregion

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

    /*
    *   StartEffect - Start the speed bonus effect.
    */
    public override void StartEffect(){
    }

    /*
    *   EndEffect - End the slow effect.
    */
    public override void EndEffect(){
    }
}
