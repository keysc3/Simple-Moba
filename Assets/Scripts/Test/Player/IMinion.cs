using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMinion : IUnit
{
    NewLevelManager levelManager { get; set; }
    NavMeshAgent navMeshAgent { get; set; }
}
