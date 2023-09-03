using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockBasicAttack : IBasicAttack
{
    public bool WindingUp { get; set; }
    public float NextAuto { get; set; }
    public float AttackSpeed { get; set; }
    public float AutoWindUp { get; set; }
    public float PhysicalDamage { get; set; }

    public void Attack(IUnit target){}
}
