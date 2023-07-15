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
    public float patienceAmount;
    public float patienceRange;
    public bool isParent;
    public float respawnTime;
    public string size;

}
