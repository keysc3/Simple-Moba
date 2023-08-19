using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IMinion : IUnit, INewDamagable
{
    NewLevelManager levelManager { get; set; }
    NavMeshAgent navMeshAgent { get; set; }
}
