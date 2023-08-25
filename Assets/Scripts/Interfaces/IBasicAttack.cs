using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements an interface for a Units basic attack.
*
* @author: Colin Keys
*/
public interface IBasicAttack
{
    bool WindingUp { get; set; }
    float NextAuto { get; set; }
    float AttackSpeed { get; }
    float AutoWindUp { get; }
    float PhysicalDamage { get; }
    //string RangeType { get; }

    void Attack(GameObject target);
}
