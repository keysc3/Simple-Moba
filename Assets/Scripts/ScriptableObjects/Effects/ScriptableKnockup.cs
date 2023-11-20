using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableEffect for initializing a Knockup effect.
*
* @author: Colin Keys
*/
[CreateAssetMenu(menuName = "Effects/Knockup")]
public class ScriptableKnockup : ScriptableEffect
{
    public float knockupHeight = 4f;
    
    /*
    *   InitializeEffect - Initializes a new knockup effect with the duration.
    *   @param spellLevel - int of the  spell level.
    *   @param casted - IUnit of the unit that caused the effect.
    *   @param effected - IUnit of the unit that is being affected by the effect.
    */
    public Effect InitializeEffect(int spellLevel, IUnit casted, IUnit effected){
        ccValue = 2;
        keyword = "Knocked Up";
        return new Knockup(this, duration[spellLevel], casted, effected);
    }
}
