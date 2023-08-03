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

    public string team { get; }

    public MinionStats(ScriptableMinion minion) : base(minion){
        team = minion.team;
    }
}
