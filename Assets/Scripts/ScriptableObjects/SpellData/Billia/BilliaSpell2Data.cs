using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BilliaSpell2", menuName = "Spell/Billia/Spell2")]
public class BilliaSpell2Data : SpellData
{
    [field: SerializeField] public GameObject visualPrefab { get; private set; }
    [field: SerializeField] public float innerRadius { get; private set; }
    [field: SerializeField] public float outerRadius { get; private set; }
    [field: SerializeField] public float maxMagnitude { get; private set; }
    [field: SerializeField] public float dashOffset { get; private set; }
    [field: SerializeField] public float dashTime { get; private set; }
    [field: SerializeField] public List<float> minionDamage { get; private set; }
}
