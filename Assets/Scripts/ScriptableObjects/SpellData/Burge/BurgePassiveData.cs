using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BurgePassive", menuName = "Spell/Burge/Passive")]
public class BurgePassiveData : SpellData
{
    [field: SerializeField] public ScriptablePersonalSpell passiveEffect { get; private set; }
    [field: SerializeField] public float timeAfterProc { get; private set; }
}
