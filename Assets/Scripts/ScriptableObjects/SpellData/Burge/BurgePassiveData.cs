using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BurgePassive", menuName = "Spell/Burge/Passive")]
public class BurgePassiveData : SpellData
{
    [field: SerializeField] public float magnitude { get; private set; }
}
