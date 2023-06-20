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
    //public Minion minion;

    public string team;

    protected override void Awake(){
        base.Awake();
        team = ((Minion) unit).team;
    }

    // Update is called once per frame
    void Update()
    {
        displayCurrentHealth = currentHealth;
    }
}
