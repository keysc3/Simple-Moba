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
    *   @param casted - IUnit of the unit that caused the effect.
    *   @param effected - IUnit of the unit that is being affected by the effect.
    */
    public Effect InitializeEffect(int spellLevel, IUnit casted, IUnit effected){
        ccValue = 0;
        return new PersonalSpell(this, duration[spellLevel], stacks, casted, effected);
    }
}
