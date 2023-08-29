using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockPlayer : IPlayer
{
    public ScriptableUnit SUnit { get; set; } = ScriptableObject.CreateInstance<ScriptableUnit>();
    public UnitStats unitStats { get; set; }
    public StatusEffects statusEffects { get; set; }
    public DamageTracker damageTracker { get; set; }
    public Inventory inventory { get; set; }
    public Collider myCollider { get; set; }
    public bool IsDead { get; }
    public BonusDamage bonusDamage { get; set; }
    public LevelManager levelManager { get; set; }
    public Score score { get; set; }
    public GameObject playerUI { get; }
    public GameObject playerBar { get; }
    public Vector3 MouseOnCast { get; set; }
    public bool IsCasting { get; set; }
    public ISpell CurrentCastedSpell { get; set; }
    public GameObject GameObject { get; set; } = new GameObject();
    public Vector3 Position { get; set; }
}
