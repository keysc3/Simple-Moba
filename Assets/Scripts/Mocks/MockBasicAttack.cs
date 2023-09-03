using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockBasicAttack : IBasicAttack
{
    public bool WindingUp { get; set; }
    public float NextAuto { get; set; }
    public float AttackSpeed { get; }
    public float AutoWindUp { get; }
    public float PhysicalDamage { get; }

    public void Attack(IUnit target){}
}
