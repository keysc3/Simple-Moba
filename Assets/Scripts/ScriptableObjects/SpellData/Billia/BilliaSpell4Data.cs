using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BilliaSpell4", menuName = "Spell/Billia/Spell4")]
public class BilliaSpell4Data : SpellData
{
    [field: SerializeField] public Sprite sleepSprite { get; private set; }
    [field: SerializeField] public ScriptableDrowsy drowsy { get; private set; }
    [field: SerializeField] public GameObject drowsyVisual { get; private set; }
    [field: SerializeField] public ScriptableDot passiveDot { get; private set; }
    [field: SerializeField] public float travelTime { get; private set; }
}
