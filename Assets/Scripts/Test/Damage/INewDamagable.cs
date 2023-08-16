using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INewDamagable
{
    void TakeDamage(float incomingDamage, string damageType, IUnit damager, bool isDot);
}
