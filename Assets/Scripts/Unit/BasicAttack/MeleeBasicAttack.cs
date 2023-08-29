using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a melee basic attack being cast.
*
* @author: Colin Keys
*/
public class MeleeBasicAttack : BasicAttackBehaviour
{
    /*
    *   Attack - Melee basic attack method.
    *   @param target - IUnit of the target to attack.
    */
    public override void Attack(IUnit target){
        basicAttackController.AttackHit(target);
    }
}
