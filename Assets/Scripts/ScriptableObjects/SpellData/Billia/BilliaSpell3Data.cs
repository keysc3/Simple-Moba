using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BilliaSpell3", menuName = "Spell/Billia/Spell3")]
public class BilliaSpell3Data : SpellData
{
    [field: SerializeField] public GameObject visualPrefab { get; private set; }
    [field: SerializeField] public ScriptableSlow slowEffect { get; private set; }
    [field: SerializeField] public float maxLobMagnitude { get; private set; }
    [field: SerializeField] public float lobTime { get; private set; }
    [field: SerializeField] public float seedSpeed { get; private set; }
    [field: SerializeField] public float seedRotation { get; private set; }
    [field: SerializeField] public float seedConeAngle { get; private set; }
    [field: SerializeField] public float seedConeRadius { get; private set; }
    [field: SerializeField] public float lobLandScale { get; private set; }
    [field: SerializeField] public float seedScale { get; private set; }
}
