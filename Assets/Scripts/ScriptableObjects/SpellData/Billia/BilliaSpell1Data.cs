using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BilliaSpell1", menuName = "Spell/Billia/Spell1")]
public class BilliaSpell1Data : SpellData
{
    [field: SerializeField] public Sprite passiveSprite { get; private set; }
    [field: SerializeField] public ScriptableSpeedBonus passiveSpeedBonus { get; private set; }
    [field: SerializeField] public GameObject visualPrefab { get; private set; }
    [field: SerializeField] public float initialAlpha { get; private set; } = 60.0f;
    [field: SerializeField] public float finalAlpha { get; private set; } = 160.0f;
    [field: SerializeField] public List<float> passiveSpeed { get; private set; }
    [field: SerializeField] public float innerRadius { get; private set; }
    [field: SerializeField] public float outerRadius { get; private set; }
    [field: SerializeField] public float passiveSpeedDuration { get; private set; }
    [field: SerializeField] public float passiveExpireDuration { get; private set; }
    [field: SerializeField] public float passiveMaxStacks { get; private set; }
}
