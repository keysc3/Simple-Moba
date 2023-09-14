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
    public Collider myCollider { get; set; }
    public bool IsDead { get; set; }
    public BonusDamage bonusDamage { get; set; }
    public GameObject GameObject { get; set; } = new GameObject();
    public Vector3 Position { get; set; }

    public void TakeDamage(float damageAmount, string damageType, IUnit damager, bool isDot){
        
    }
}
