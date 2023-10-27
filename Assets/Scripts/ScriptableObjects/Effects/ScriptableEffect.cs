using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableObject for a scriptable status effect.
*
* @author: Colin Keys
*/
public class ScriptableEffect : ScriptableObject
{
    [field: SerializeField] public List<float> duration { get; private set; } = new List<float>();
    public bool isStackable = false;
    [field: SerializeField] public int ccValue { get; protected set; }
    [field: SerializeField] public bool isBuff { get; private set; }
    [field: SerializeField] public Sprite sprite { get; private set; }
    public string keyword { get; protected set; } = "Default";

    public static ScriptableEffect CreateInstance(bool isBuff){
        ScriptableEffect effect = CreateInstance<ScriptableEffect>();
        effect.isBuff = isBuff;
        return effect;
    }
}
