using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableEffect for initializing a Sleep effect.
*
* @author: Colin Keys
*/
[CreateAssetMenu(menuName = "Effects/Sleep")]
public class ScriptableSleep : ScriptableEffect
{
    //[SerializeField] private List<float> duration = new List<float>();

    /*
    *   InitializeEffect - Initializes a new sleep effect with the objects duration and total amount of damage to deal.
    *   @param spellLevel - int of the casting spells level.
    *   @param casted - IUnit of the unit that caused the effect.
    *   @param effected - IUnit of the unit that is being affected by the effect.
    */
    public Effect InitializeEffect(int spellLevel, IUnit casted, IUnit effected){
        ccValue = 2;
        keyword = "Asleep";
        return new Sleep(this, duration[spellLevel], casted, effected);
    }
}
