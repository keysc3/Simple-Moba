using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableEffect for initializing a spell effect.
*
* @author: Colin Keys
*/
[CreateAssetMenu(menuName = "Effects/Personal Spell")]
public class ScriptablePersonalSpell : ScriptableEffect
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
        return new PersonalSpell(this, duration[spellLevel], stacks, unitCasted, unitEffected);
    }
}
