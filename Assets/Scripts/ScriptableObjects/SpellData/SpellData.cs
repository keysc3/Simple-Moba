using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellData : ScriptableObject
{
    [field: SerializeField] public Sprite sprite { get; private set; }
    [field: SerializeField] public float castTime { get; private set; }
    [field: SerializeField] public List<float> baseDamage { get; private set; }
    [field: SerializeField] public List<float> baseCd { get; private set; }
    [field: SerializeField] public List<float> baseMana { get; private set; }
}
