using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockUnit : IUnit
{
    public ScriptableUnit SUnit { get; set; } = ScriptableObject.CreateInstance<ScriptableUnit>();
    public UnitStats unitStats { get; set; }
    public StatusEffects statusEffects { get; set; }
    public DamageTracker damageTracker { get; set; }
    public Inventory inventory { get; set; }
    //NewScore score { get; set; }
    //LevelManager levelManager { get; set; }
    //NavMeshAgent navMeshAgent { get; set; }
    public Collider myCollider { get; set; } = null;
    //GameObject playerUI { get; set; }
    //GameObject playerBar { get; set; }
    public bool IsDead { get; } = false;
    public BonusDamage bonusDamage { get; set; }
}
