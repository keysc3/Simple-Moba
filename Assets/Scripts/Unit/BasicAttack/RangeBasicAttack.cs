using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a range basic attack being cast.
*
* @author: Colin Keys
*/
public class RangeBasicAttack : BasicAttackBehaviour
{
    private float AttackProjectileSpeed { get => unitStats.attackProjectileSpeed.GetValue(); }

    /*
    *   Attack - Range basic attack method.
    *   @param target - GameObject of the target to attack.
    */
    public override void Attack(GameObject target){
        if(attackProjectile != null)
            StartCoroutine(AttackProjectile(target));
    }

    /*
    *   AttackProjectile - Animates the basic attack GameObject towards its target.
    *   @param target - GameObject of the target to attack.
    */
    private IEnumerator AttackProjectile(GameObject target){
        // Create attack GameObject and set necessary variables.
        GameObject projectile = (GameObject) Instantiate(attackProjectile, transform.position, Quaternion.identity);
        BasicAttackTrigger basicAttackTrigger = projectile.gameObject.GetComponent<BasicAttackTrigger>();
        basicAttackTrigger.Target = target;
        basicAttackTrigger.basicAttackController = basicAttackController;
        // While the attack still exists animate it.
        while(projectile){
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, target.transform.position, AttackProjectileSpeed * Time.deltaTime);
            yield return null;
        }
    }

}
