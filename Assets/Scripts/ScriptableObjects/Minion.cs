using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableObject for a minion. Extends Unit.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "New Minion", menuName = "Unit/Minion")]
public class Minion : Unit
{
    public string team;
}
