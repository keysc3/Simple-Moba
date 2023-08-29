using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
* Purpose: Implements an interface for a Unit.
*
* @author: Colin Keys
*/
public delegate void BonusDamage(IUnit toDamage, bool isDot);

public interface IUnit
{
    ScriptableUnit SUnit { get; }
    UnitStats unitStats { get; set; }
    StatusEffects statusEffects { get; set; }
    DamageTracker damageTracker { get; set; }
    Inventory inventory { get; set; }
    bool IsDead { get; }
    BonusDamage bonusDamage { get; set; }
    GameObject GameObject { get; }
    Vector3 Position { get; set; }
}
