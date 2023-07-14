using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableEffect for intializing a spell effect.
*
* @author: Colin Keys
*/
[CreateAssetMenu(menuName = "Effects/Spell")]
public class ScriptableSpell : ScriptableEffect
{
    //[field: SerializeField] public float duration { get; private set; }
    [field: SerializeField] public int stacks { get; private set; }
    
    /*
    *   InitializeEffect - Initializes a new spell effect with the duration and bonus percent.
    *   @param unitCasted - GameObject of the unit that casted the spell.
    *   @param unitEffected - GameObject of the unit effected by the spell.
    */
    public Effect InitializeEffect(int spellLevel, GameObject unitCasted, GameObject unitEffected){
        ccValue = 0;
        return new Spell(this, duration[spellLevel], stacks, unitCasted, unitEffected);
    }
}
