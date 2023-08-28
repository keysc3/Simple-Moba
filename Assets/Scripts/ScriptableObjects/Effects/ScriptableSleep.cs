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
    *   @param unitCasted - GameObject of the unit that casted the sleep.
    *   @param unitEffected - GameObject of the unit effected by the sleep.
    */
    public Effect InitializeEffect(int spellLevel, GameObject unitCasted, GameObject unitEffected){
        ccValue = 2;
        return new Sleep(this, duration[spellLevel], unitCasted, unitEffected);
    }
}
