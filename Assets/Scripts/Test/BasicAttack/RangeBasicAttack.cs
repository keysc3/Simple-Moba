using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeBasicAttack : BasicAttackBehaviour
{
    private float AttackProjectileSpeed { get => unitStats.attackProjectileSpeed.GetValue(); }

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
        GameObject projectile = (GameObject) GameObject.Instantiate(attackProjectile, transform.position, Quaternion.identity);
        BasicAttackTrigger newBasicAttackTrigger = projectile.gameObject.GetComponent<BasicAttackTrigger>();
        newBasicAttackTrigger.Target = target;
        newBasicAttackTrigger.nba = nba;
        // While the attack still exists animate it.
        while(projectile){
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, target.transform.position, AttackProjectileSpeed * Time.deltaTime);
            yield return null;
        }
    }

}
