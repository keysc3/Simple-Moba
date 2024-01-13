using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements an interface for a Units basic attack.
*
* @author: Colin Keys
*/

public delegate void BasicAttackHitCallback(IUnit hit, IUnit from);

public interface IBasicAttack
{
    BasicAttackHitCallback basicAttackHitCallback { get; set; }
    bool WindingUp { get; set; }
    float NextAuto { get; set; }
    float AttackSpeed { get; }
    float AutoWindUp { get; }
    float PhysicalDamage { get; }
    //string RangeType { get; }

    void Attack(IUnit target);
}
