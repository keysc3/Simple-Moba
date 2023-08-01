using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BahriPassive", menuName = "Spell/Bahri/Passive")]
public class BahriPassiveData : SpellData
{
    [field: SerializeField] public ScriptablePersonalSpell passivePreset { get; private set; }
}
