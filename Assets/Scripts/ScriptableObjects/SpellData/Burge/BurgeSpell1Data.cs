using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements Bahri'a first spell. Bahri throws a damage dealing orb at a target direction that returns after reaching its maximum magnitude.
* Any unit hit on the initial cast takes magic damage and any unit hit by the return takes true damage.
* The orb accelerates upon starting its return until reaching Bahri.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "BurgeSpell1", menuName = "Spell/Burge/Spell1")]
public class BurgeSpell1Data : SpellData
{
    [field: SerializeField] public float magnitude { get; private set; }
    [field: SerializeField] public float jumpTime { get; private set; }
    [field: SerializeField] public float hitboxLength { get; private set; }
    [field: SerializeField] public float hitboxWidth { get; private set; }
    [field: SerializeField] public int numberOfHits { get; private set; }
}
