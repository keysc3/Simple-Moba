using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements an interface for a damageable object.
*
* @author: Colin Keys
*/
public interface IDamageable
{
    void TakeDamage(float incomingDamage, DamageType damageType, IUnit damager, bool isDot);
}
