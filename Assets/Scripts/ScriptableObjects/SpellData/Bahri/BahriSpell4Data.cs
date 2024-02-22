using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BahriSpell4", menuName = "Spell/Bahri/Spell4")]
public class BahriSpell4Data : SpellData
{
    [field: SerializeField] public GameObject missile { get; private set; }
    [field: SerializeField] public ScriptablePersonalSpell spell4 { get; private set; }
    [field: SerializeField] public float maxMagnitude { get; private set; }
    [field: SerializeField] public float speed { get; private set; }
    [field: SerializeField] public float missileSpeed { get; private set; }
    [field: SerializeField] public float radius { get; private set; }
    [field: SerializeField] public int charges { get; private set; }
    [field: SerializeField] public float duration { get; private set; }
    [field: SerializeField] public float takedownDuration { get; private set; }
    [field: SerializeField] public float projectileScale { get; private set; }
}
