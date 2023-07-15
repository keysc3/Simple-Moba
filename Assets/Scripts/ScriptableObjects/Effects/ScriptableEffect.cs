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
    public bool isStackable = false;
    public int ccValue;
    public bool isBuff;
    public Sprite sprite;

}
