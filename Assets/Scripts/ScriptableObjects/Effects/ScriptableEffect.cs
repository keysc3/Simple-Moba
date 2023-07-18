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
    [field: SerializeField] public List<float> duration { get; private set; } = new List<float>();
    [field: SerializeField] public bool isStackable { get; private set; } = false;
    [field: SerializeField] public int ccValue { get; protected set; }
    [field: SerializeField] public bool isBuff { get; private set; }
    [field: SerializeField] public Sprite sprite { get; private set; }

}
