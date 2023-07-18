using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableObject for a monster. Extends Unit.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "New Monster", menuName = "Unit/Monster")]
public class ScriptableMonster : ScriptableUnit
{
    [field: SerializeField] public float patienceAmount { get; private set; }
    [field: SerializeField] public float patienceRange { get; private set; }
    [field: SerializeField] public bool isParent { get; private set; }
    [field: SerializeField] public float respawnTime { get; private set; }
    [field: SerializeField] public string size { get; private set; }
}
