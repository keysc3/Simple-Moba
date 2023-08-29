using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableEffect for initializing a Dot effect.
*
* @author: Colin Keys
*/
[CreateAssetMenu(menuName = "Effects/Dot")]
public class ScriptableDot : ScriptableEffect
{
    //[field: SerializeField] public float duration { get; private set; }
    [field: SerializeField] public string damageType { get; private set; }
    [field: SerializeField] public float tickRate { get; private set; }

    /*
    *   InitializeEffect - Initializes a new dot effect with the objects duration and total amount of damage to deal.
    *   @param totalDamage - float of the total damage to deal.
    *   @param unitCasted - GameObject of the unit that casted the charm.
    *   @param unitEffected - GameObject of the unit effected by the charm.
    */
    public Effect InitializeEffect(float totalDamage, int spellLevel, IUnit casted, IUnit effected){
        ccValue = 0;
        return new Dot(this, totalDamage, duration[spellLevel], casted, effected);
    }
}
