using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IPlayer : IUnit
{
    LevelManager levelManager { get; set; }
    Score score { get; set; }
    //NavMeshAgent navMeshAgent { get; set; }
    GameObject playerUI { get; }
    GameObject playerBar { get; }
    Vector3 MouseOnCast { get; set; }
    bool IsCasting { get; set; }
    ISpell CurrentCastedSpell { get; set; }
}