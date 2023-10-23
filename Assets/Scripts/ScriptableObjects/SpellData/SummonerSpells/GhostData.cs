using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ghost", menuName = "Spell/Summoner Spells/Ghost")]
public class GhostData : SpellData
{
    [field: SerializeField] public ScriptableSpeedBonus speedBonus { get; private set; }
}
