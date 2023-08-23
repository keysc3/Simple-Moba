using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void TakeDamage(float incomingDamage, string damageType, IUnit damager, bool isDot);
}
