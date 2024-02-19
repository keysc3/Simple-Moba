using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BahriSpell3", menuName = "Spell/Bahri/Spell3")]
public class BahriSpell3Data : SpellData
{
    [field: SerializeField] public GameObject missile { get; private set; }
    [field: SerializeField] public ScriptableCharm charmEffect { get; private set; }
    [field: SerializeField] public float magnitude { get; private set; }
    [field: SerializeField] public float speed { get; private set; }
    [field: SerializeField] public float charmScale { get; private set; }
}
