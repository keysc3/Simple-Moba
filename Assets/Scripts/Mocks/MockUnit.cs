using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements an mock unit for testing.
*
* @author: Colin Keys
*/
public class MockUnit : IUnit
{
    public ScriptableUnit SUnit { get; set; } = ScriptableObject.CreateInstance<ScriptableUnit>();
    public UnitStats unitStats { get; set; }
    public StatusEffects statusEffects { get; set; }
    public DamageTracker damageTracker { get; set; }
    public Inventory inventory { get; set; }
    public Collider myCollider { get; set; } = null;
    public bool IsDead { get; } = false;
    public BonusDamage bonusDamage { get; set; }
}
