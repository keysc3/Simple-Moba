using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BurgeSpell1", menuName = "Spell/Burge/Spell1")]
public class BurgeSpell1Data : SpellData
{
    [field: SerializeField] public float magnitude { get; private set; }
    [field: SerializeField] public float jumpTime { get; private set; }
    [field: SerializeField] public float hitboxLength { get; private set; }
    [field: SerializeField] public float hitboxWidth { get; private set; }
    [field: SerializeField] public float numberOfHits { get; private set; }
}
