using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    ScriptableUnit SUnit { get; }
    UnitStats unitStats { get; set; }
    NewStatusEffects statusEffects { get; set; }
    NewDamageTracker damageTracker { get; set; }
    Inventory inventory { get; set; }
    //NewScore score { get; set; }
    //LevelManager levelManager { get; set; }
    //NavMeshAgent navMeshAgent { get; set; }
    Collider myCollider { get; set; }
    //GameObject playerUI { get; set; }
    //GameObject playerBar { get; set; }
    bool IsDead { get; }
}
