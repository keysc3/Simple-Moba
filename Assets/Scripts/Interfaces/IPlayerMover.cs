using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements an interface for a player/unit movement.
*
* @author: Colin Keys
*/
public interface IPlayerMover
{
    IUnit TargetedEnemy { get; set; }
    Vector3 Destination { get; set; }
    Vector3 NextDestination { get; }
    Vector3 CurrentTarget { get; set; }
    bool IsMoving { get; }

}
