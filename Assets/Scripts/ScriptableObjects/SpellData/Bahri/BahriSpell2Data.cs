using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BahriSpell2", menuName = "Spell/Bahri/Spell2")]
public class BahriSpell2Data : SpellData
{
    [field: SerializeField] public GameObject missile { get; private set; }
    [field: SerializeField] public ScriptableSpeedBonus speedBonus { get; private set; }
    [field: SerializeField] public ScriptableCharm charm { get; private set; }
    [field: SerializeField] public float magnitude { get; private set; }
    [field: SerializeField] public float rotationSpeed { get; private set; }
    [field: SerializeField] public float heightOffset { get; private set; }
    [field: SerializeField] public float duration { get; private set; }
    [field: SerializeField] public float radius{ get; private set; }
    [field: SerializeField] public float speed { get; private set; }
    [field: SerializeField] public float multiplier { get; private set; }
    [field: SerializeField] public float bonusSpeedPercent { get; private set; }
}
