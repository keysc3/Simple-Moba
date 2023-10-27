using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableEffect for initializing a Drowsy effect.
*
* @author: Colin Keys
*/
[CreateAssetMenu(menuName = "Effects/Drowsy")]
public class ScriptableDrowsy : ScriptableEffect
{
    public ScriptableSlow slow;
    public ScriptableSleep sleep;

    /*
    *   InitializeEffect - Initializes a new drowsy effect.
    *   @param casted - IUnit of the unit that caused the effect.
    *   @param effected - IUnit of the unit that is being affected by the effect.
    */
    public Effect InitializeEffect(int spellLevel, IUnit casted, IUnit effected){
        ccValue = 0;
        keyword = "Drowsy";
        return new Drowsy(this, duration[spellLevel], spellLevel, casted, effected);
    }
}
