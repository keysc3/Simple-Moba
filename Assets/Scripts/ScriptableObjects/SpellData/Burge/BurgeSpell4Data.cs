using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BurgeSpell3", menuName = "Spell/Burge/Spell4")]
public class BurgeSpell4Data : SpellData
{
    [field: SerializeField] public float spellFill { get; private set; }
}
