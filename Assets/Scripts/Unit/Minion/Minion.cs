using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a minion unit.
*
* @author: Colin Keys
*/
public class Minion : Unit
{
    /*
    *   DeathActions - Handles any game state/other Unit actions upon this Minions death.
    *   @param fromUnit - Unit that killed this Minion.
    */
    protected override void DeathActions(Unit fromUnit){
        base.DeathActions(fromUnit);
        if(fromUnit is Player){
            Player killer = (Player) fromUnit;
            killer.score.CreepKill(gameObject);
            UIManager.instance.UpdateCS(killer.score.cs.ToString(), killer.playerUI);
        }
    }
}
