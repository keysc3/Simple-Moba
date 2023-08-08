using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements basic attacks class for a player.
*
* @author: Colin Keys
*/
public class BasicAttack
{
    public bool windingUp = false;
    public float nextAuto { get; private set; } = 0.0f;

    private GameObject attackProjectile;
    private Player player;

    public BasicAttack(Player player, GameObject attackProjectile){
        this.player = player;
        this.attackProjectile = attackProjectile;
    }

    /*
    *   Attack - Units basic attack method.
    *   @param target - GameObject of the enemy to attack.
    */
    public void Attack(GameObject target){
        if(player.SUnit.rangeType == "melee")
            MeleeAttack(target);
        else
            RangeAttack(target);
    }

    /*
    *   MeleeAttack - Melee basic attack method.
    *   @param target - GameObject of the target to attack.
    */
    private void MeleeAttack(GameObject target){
        AttackHit(target);
    }

    /*
    *   RangeAttack - Ranged basic attack method.
    *   @param target - GameObject of the target to attack.
    */
    private void RangeAttack(GameObject target){
        if(attackProjectile != null)
            player.StartCoroutine(AttackProjectile(target));
    }
    
    /*
    *   AttackHit - Apply basic attack damage.
    *   @param target - GameObject of the target to attack.
    */
    public void AttackHit(GameObject target){
        float physicalDamage = player.unitStats.physicalDamage.GetValue();
        target.GetComponent<Unit>().TakeDamage(physicalDamage, "physical", player.gameObject, false);
    }

    /*
    *   AttackProjectile - Animates the basic attack GameObject towards its target.
    *   @param target - GameObject of the target to attack.
    */
    private IEnumerator AttackProjectile(GameObject target){
        // Create attack GameObject and set necessary variables.
        GameObject projectile = (GameObject) GameObject.Instantiate(attackProjectile, player.transform.position, Quaternion.identity);
        BasicAttackTrigger basicAttackTrigger = projectile.gameObject.GetComponent<BasicAttackTrigger>();
        basicAttackTrigger.Target = target;
        basicAttackTrigger.basicAttack = this;
        // While the attack still exists animate it.
        while(projectile){
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, target.transform.position, player.unitStats.attackProjectileSpeed.GetValue() * Time.deltaTime);
            yield return null;
        }
    }

    /*
    *   AutoAttackWindUp - Winds up the players basic attack. Attacks can be canceled if an action is input before the windup finishes.
    */
    public IEnumerator BasicAttackWindUp(){
        float timer = 0.0f;
        player.unitStats.UpdateAttackSpeed();
        // Wind up time is the time it takes for the player to attack * the percentage of 
        float windUpTime = ((1.0f/player.unitStats.attackSpeed.GetValue()) * player.unitStats.autoWindUp.GetValue());
        while(player.playerController.targetedEnemy != null && windingUp){
            if(timer <= windUpTime){
                // TODO: Animate windup
            }
            else{
                Attack(player.playerController.targetedEnemy);
                nextAuto = Time.time + 1.0f/player.unitStats.attackSpeed.GetValue();
                Debug.Log("Next auto in: " + 1.0f/player.unitStats.attackSpeed.GetValue());
                windingUp = false;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        windingUp = false;
    }
}
