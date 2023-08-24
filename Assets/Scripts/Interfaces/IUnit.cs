using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void BonusDamage(IUnit toDamage, bool isDot);

public interface IUnit
{
    ScriptableUnit SUnit { get; }
    UnitStats unitStats { get; set; }
    StatusEffects statusEffects { get; set; }
    DamageTracker damageTracker { get; set; }
    Inventory inventory { get; set; }
    //NewScore score { get; set; }
    //LevelManager levelManager { get; set; }
    //NavMeshAgent navMeshAgent { get; set; }
    Collider myCollider { get; set; }
    //GameObject playerUI { get; set; }
    //GameObject playerBar { get; set; }
    bool IsDead { get; }
    BonusDamage bonusDamage { get; set; }
}
