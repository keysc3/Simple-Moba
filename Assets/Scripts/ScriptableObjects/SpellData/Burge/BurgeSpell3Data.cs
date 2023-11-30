using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Extends SpellData for spell specific constant data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BurgeSpell3", menuName = "Spell/Burge/Spell3")]
public class BurgeSpell3Data : SpellData
{
    [field: SerializeField] public float hitboxLength { get; private set; }
    [field: SerializeField] public float hitboxWidth { get; private set; }
    [field: SerializeField] public float chargedHitboxLength { get; private set; }
    [field: SerializeField] public float chargedHitboxWidth { get; private set; }
    [field: SerializeField] public float maxChargeDuration { get; private set; }
    [field: SerializeField] public float holdDuration { get; private set; }
    [field: SerializeField] public float cdRefundPercent { get; private set; }
    [field: SerializeField] public float timeBetweenDash { get; private set; }
    [field: SerializeField] public float dashMagnitude { get; private set; }
    [field: SerializeField] public float dashTime { get; private set; }
    [field: SerializeField] public GameObject visualHitbox { get; private set; }
    [field: SerializeField] public GameObject secondCastVisual { get; private set; }
}
