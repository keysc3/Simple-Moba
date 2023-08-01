using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BilliaPassive", menuName = "Spell/Billia/Passive")]
public class BilliaPassiveData : SpellData
{
    [field: SerializeField] public ScriptableDot passiveDot { get; private set; }
    [field: SerializeField] public List<float> baseChampionHeal { get; private set; }
    [field: SerializeField] public List<float> baseLargeMonsterHeal { get; private set; }
    
}
