using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BahriSpell4", menuName = "Spell/Bahri/Spell4")]
public class BahriSpell4Data : SpellData
{
    [field: SerializeField] public GameObject missile { get; private set; }
    [field: SerializeField] public ScriptablePersonalSpell spell4 { get; private set; }
    [field: SerializeField] public float maxMagnitude { get; private set; }
    [field: SerializeField] public float speed { get; private set; }
    [field: SerializeField] public float radius { get; private set; }
    [field: SerializeField] public float charges { get; private set; }
    [field: SerializeField] public float duration { get; private set; }
    [field: SerializeField] public float takedownDuration { get; private set; }
}
