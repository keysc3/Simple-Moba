using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockPlayerBehaviour : MonoBehaviour, IPlayer
{
    public ScriptableUnit SUnit { get; set; }
    public UnitStats unitStats { get; set; }
    public StatusEffects statusEffects { get; set; }
    public DamageTracker damageTracker { get; set; }
    public Inventory inventory { get; set; }
    public Collider hitbox { get; set; }
    public bool IsDead { get; set; }
    public BonusDamage bonusDamage { get; set; }
    public LevelManager levelManager { get; set; }
    public Score score { get; set; }
    public GameObject playerUI { get; set; }
    public GameObject playerBar { get; set; }
    public Vector3 MouseOnCast { get; set; }
    public bool IsCasting { get; set; }
    public ISpell CurrentCastedSpell { get; set; }
    public GameObject GameObject { get; set; }
    public Vector3 Position { get; set; }

    public void TakeDamage(float damageAmount, DamageType damageType, IUnit damager, bool isDot){
        
    }
}
