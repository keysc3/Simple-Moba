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
    *   @param target - GameObject of the target to attack.
    */
    public override void Attack(GameObject target){
        basicAttackController.AttackHit(target);
    }
}
