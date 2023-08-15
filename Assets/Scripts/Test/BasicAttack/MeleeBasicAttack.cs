using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBasicAttack : BasicAttackBehaviour
{
    /*
    *   MeleeAttack - Melee basic attack method.
    *   @param target - GameObject of the target to attack.
    */
    public override void Attack(GameObject target){
        nba.AttackHit(target);
    }
}
