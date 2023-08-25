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
    GameObject TargetedEnemy { get; set; }
    //private float attackTime;
    //private Camera mainCamera;
    //private RaycastHit hitInfo;
    Vector3 Destination { get; set; }
    Vector3 CurrentTarget { get; set; }
    Vector3 Position { get; set; }
    float Range { get; }
    //Player player;
    //private ChampionStats championStats;
}
