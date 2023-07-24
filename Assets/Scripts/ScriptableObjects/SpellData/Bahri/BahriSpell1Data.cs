using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BahriSpell1", menuName = "Spell/Bahri/Spell1")]
public class BahriSpell1Data : SpellData
{
    [field: SerializeField] public GameObject orb { get; private set; }
    [field: SerializeField] public float magnitude { get; private set; }
    [field: SerializeField] public float speed { get; private set; }
    [field: SerializeField] public float minSpeed { get; private set; }
    [field: SerializeField] public float maxSpeed { get; private set; }
    [field: SerializeField] public float accel { get; private set; }
}
