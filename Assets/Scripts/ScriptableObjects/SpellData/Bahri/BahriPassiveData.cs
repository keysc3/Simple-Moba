using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BahriPassive", menuName = "Spell/Bahri/Passive")]
public class BahriPassiveData : SpellData
{
    [field: SerializeField] public ScriptablePersonalSpell passivePreset { get; private set; }
}
