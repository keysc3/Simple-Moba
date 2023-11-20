using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BurgeSpell2", menuName = "Spell/Burge/Spell2")]
public class BurgeSpell2Data : SpellData
{
    [field: SerializeField] public float duration { get; private set; }
    [field: SerializeField] public float sizeMultiplier { get; private set; }
    [field: SerializeField] public GameObject prefab { get; private set; }
    [field: SerializeField] public float magnitude { get; private set; }
    [field: SerializeField] public int numberOfHits { get; private set; }
    [field: SerializeField] public ScriptableKnockup knockup { get; private set; }
}
