using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockPlayerMover : IPlayerMover
{
    public IUnit TargetedEnemy { get; set; }
    public Vector3 Destination { get; set; }
    public Vector3 NextDestination { get; set; }
    public Vector3 CurrentTarget { get; set; }
    public bool IsMoving { get; set; }
}
