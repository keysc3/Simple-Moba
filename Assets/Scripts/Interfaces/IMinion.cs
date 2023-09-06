using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements an interface for a Minion, extends units.
*
* @author: Colin Keys
*/
public interface IMinion : IUnit, IDamageable
{
    LevelManager levelManager { get; set; }
    //NavMeshAgent navMeshAgent { get; set; }
}
