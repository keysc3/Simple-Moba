using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IPlayer : IUnit
{
    NewLevelManager levelManager { get; set; }
    NewScore score { get; set; }
    //NavMeshAgent navMeshAgent { get; set; }
    GameObject playerUI { get; }
    GameObject playerBar { get; }
}
