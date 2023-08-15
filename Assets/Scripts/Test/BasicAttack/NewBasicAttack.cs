using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBasicAttack
{

    private IBasicAttack _ba;
    private IPlayerMover _pm;

    public NewBasicAttack(IBasicAttack ba, IPlayerMover pm){
        _ba = ba;
        _pm = pm;
    }

    public void CheckForTargets(){
        // Check for anything in range if no target.
    }

    public bool CanAuto(float currentTime){
        if(_pm.TargetedEnemy != null){
            //Vector3 myTarget = target.transform.position;
            //myTarget.y = 0.0f;
            float distToEnemy = (_pm.Position - _pm.TargetedEnemy.transform.position).magnitude;
            // If the enemy is in auto range then start autoing.
            if(distToEnemy < _pm.Range){
                if(currentTime >  _ba.NextAuto && !_ba.WindingUp){
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
        float windUpTime = (1.0f/_ba.AttackSpeed) * _ba.AutoWindUp;
        while(_pm.TargetedEnemy != null && _ba.WindingUp && timer <= windUpTime){
            // TODO: Animate windup
            timer += Time.deltaTime;
            yield return null;
        }
        _ba.Attack(_pm.TargetedEnemy);
        _ba.NextAuto = Time.time + (1.0f/_ba.AttackSpeed);
        //Debug.Log("Next auto in: " + 1.0f/player.unitStats.attackSpeed.GetValue());
        _ba.WindingUp = false;
    }

    //TODO FIX TAKEDAMAGE FROM PARAMETER
    /*
    *   AttackHit - Apply basic attack damage.
    *   @param target - GameObject of the target to attack.
    */
    public void AttackHit(GameObject target){
        float physicalDamage = _ba.PhysicalDamage;
        target.GetComponent<Unit>().TakeDamage(physicalDamage, "physical", target, false);
    }
}
