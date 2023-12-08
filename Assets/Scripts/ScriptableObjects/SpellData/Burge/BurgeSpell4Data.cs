using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BurgeSpell4", menuName = "Spell/Burge/Spell4")]
public class BurgeSpell4Data : SpellData
{
    [field: SerializeField] public float spellFill { get; private set; }
    [field: SerializeField] public float width { get; private set; }
    [field: SerializeField] public float length { get; private set; }
    [field: SerializeField] public float maxDuration { get; private set; }
    [field: SerializeField] public float minDuration { get; private set; }
    [field: SerializeField] public ScriptablePersonalSpell spellEffect { get; private set; }
    [field: SerializeField] public Sprite castedSprite { get; private set; }
}
