using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements an interface for a Player, extends units.
*
* @author: Colin Keys
*/
public interface IPlayer : IUnit
{
    LevelManager levelManager { get; set; }
    Score score { get; set; }
    GameObject playerUI { get; }
    GameObject playerBar { get; }
    Vector3 MouseOnCast { get; set; }
    bool IsCasting { get; set; }
    ISpell CurrentCastedSpell { get; set; }
}
