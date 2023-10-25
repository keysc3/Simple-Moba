using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockUnitBehaviour : MonoBehaviour, IUnit
{
    public ScriptableUnit SUnit { get; set; }
    public UnitStats unitStats { get; set; }
    public StatusEffects statusEffects { get; set; }
    public DamageTracker damageTracker { get; set; }
    public Inventory inventory { get; set; }
    public Collider myCollider { get; set; }
    public bool IsDead { get; set; }
    public BonusDamage bonusDamage { get; set; }
    public GameObject GameObject { get; set; }
    public Vector3 Position { get; set; }

    public void TakeDamage(float damageAmount, DamageType damageType, IUnit damager, bool isDot){
        
    }
}
