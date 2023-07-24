using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BahriPassive", menuName = "Spell/Bahri/Passive")]
public class BahriPassiveData : SpellData
{
    [field: SerializeField] public ScriptablePersonalSpell passivePreset { get; private set; }
    [field: SerializeField] public float magnitude { get; private set; }
    [field: SerializeField] public float speed { get; private set; }
    [field: SerializeField] public float minSpeed { get; private set; }
    [field: SerializeField] public float maxSpeed { get; private set; }
    [field: SerializeField] public float accel { get; private set; }

}
