using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements basic attacks methods for a unit.
*
* @author: Colin Keys
*/
public class BasicAttackController
{

    private IBasicAttack basicAttack;
    private IPlayerMover playerMover;

    public BasicAttackController(IBasicAttack basicAttack, IPlayerMover playerMover){
        this.basicAttack = basicAttack;
        this.playerMover = playerMover;
    }

    public void CheckForTargets(){
        // Check for anything in range if no target.
    }

    /*
    *   CanAuto - Determines if an auto can be cast.
    *   @param currentTime - float of the current game time.
    */
    public bool CanAuto(float currentTime){
        if(playerMover.TargetedEnemy != null){
            //Vector3 myTarget = target.transform.position;
            //myTarget.y = 0.0f;
            float distToEnemy = (playerMover.Position - playerMover.TargetedEnemy.transform.position).magnitude;
            // If the enemy is in auto range then start autoing.
            if(distToEnemy < playerMover.Range){
                if(currentTime >  basicAttack.NextAuto && !basicAttack.WindingUp){
                    return true;
                }
            }
        }
        return false;
    }

    /*
    *   AutoAttackWindUp - Winds up the players basic attack. Attacks can be canceled if an action is input before the windup finishes.
    */
    public IEnumerator BasicAttackWindUp(){
        float timer = 0.0f;
        // Wind up time is the time it takes for the player to attack * the percentage of 
        float windUpTime = (1.0f/basicAttack.AttackSpeed) * basicAttack.AutoWindUp;
        while(playerMover.TargetedEnemy != null && basicAttack.WindingUp && timer <= windUpTime){
            // TODO: Animate windup
            timer += Time.deltaTime;
            yield return null;
        }
        basicAttack.Attack(playerMover.TargetedEnemy);
        basicAttack.NextAuto = Time.time + (1.0f/basicAttack.AttackSpeed);
        //Debug.Log("Next auto in: " + 1.0f/player.unitStats.attackSpeed.GetValue());
        basicAttack.WindingUp = false;
    }

    /*
    *   AttackHit - Apply basic attack damage.
    *   @param target - GameObject of the target to attack.
    */
    public void AttackHit(GameObject target){
        float physicalDamage = basicAttack.PhysicalDamage;
        target.GetComponent<IDamageable>().TakeDamage(physicalDamage, "physical", target.GetComponent<IUnit>(), false);
    }
}
