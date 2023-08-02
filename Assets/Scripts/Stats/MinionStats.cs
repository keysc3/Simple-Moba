using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements minion stats. Extends UnitStats.
*
* @author: Colin Keys
*/
public class MinionStats : UnitStats
{

    [field: SerializeField] public string team { get; private set; }

    public MinionStats(ScriptableMinion minion) : base(minion){
        team = minion.team;
    }
}
