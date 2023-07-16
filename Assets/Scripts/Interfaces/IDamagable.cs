using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Interface for a damagable GameObject.
*
* @author: Colin Keys
*/
public interface IDamagable
{
    void TakeDamage(float incomingDamage, string damageType, GameObject from, bool isDot);
}
