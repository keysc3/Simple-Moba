using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flash", menuName = "Spell/Summoner Spells/Flash")]
public class FlashData : SpellData
{
    [field: SerializeField] public float maxMagnitude { get; private set; }
}
