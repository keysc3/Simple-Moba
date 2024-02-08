using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BurgeSpell4", menuName = "Spell/Burge/Spell4")]
public class BurgeSpell4Data : SpellData
{
    [field: SerializeField] public float width { get; private set; }
    [field: SerializeField] public float length { get; private set; }
    [field: SerializeField] public float maxCastedHits { get; private set; }
    [field: SerializeField] public float maxDuration { get; private set; }
    [field: SerializeField] public float minDuration { get; private set; }
    [field: SerializeField] public ScriptablePersonalSpell spellEffect { get; private set; }
    [field: SerializeField] public Sprite castedSprite { get; private set; }
    [field: SerializeField] public Dictionary<string, float> fillPerSpellHit { get; private set; } = new Dictionary<string, float>(){
        {"Burge1", 5f},
        {"Burge2", 5f},
        {"Burge3", 20f}
    };
    [field: SerializeField] public float fadeTime { get; private set; }
    [field: SerializeField] public GameObject visualHitbox { get; private set; }
    [field: SerializeField] public string secondName { get; private set; }
    [field: SerializeField] public float recastCastTime { get; private set; }
}
