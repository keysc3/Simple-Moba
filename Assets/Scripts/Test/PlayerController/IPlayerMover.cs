using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
