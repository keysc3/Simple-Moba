using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableEffect for initializing a Charm effect.
*
* @author: Colin Keys
*/
[CreateAssetMenu(menuName = "Effects/Charm")]
public class ScriptableCharm : ScriptableEffect
{
    public ScriptableSlow slow;

    /*
    *   InitializeEffect - Initializes a new charm effect with the objects duration and slow percent.
    *   @param spellLevel - int of the charms spell level.
    *   @param casted - IUnit of the unit that caused the effect.
    *   @param effected - IUnit of the unit that is being affected by the effect.
    */
    public Effect InitializeEffect(int spellLevel, IUnit casted, IUnit effected){
        ccValue = 1;
        return new Charm(this, duration[spellLevel], spellLevel, casted, effected);
    }
}
