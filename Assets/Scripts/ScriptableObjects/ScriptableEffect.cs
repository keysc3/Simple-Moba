using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableObject for a scriptable status effect.
*
* @author: Colin Keys
*/
public abstract class ScriptableEffect : ScriptableObject
{
    public bool isEffectStackable;

    public abstract Effect InitializeEffect(float duration, GameObject unitCasted, GameObject unitEffected, float speedMultiplier);

}
