using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController
{
    private IPlayerMover _pm;

    public NewPlayerController(IPlayerMover pm){
        _pm = pm;
    }
    
    /*
    *   RightClick - Moves the player to the click position if no enemy is targeted, otherwise sets the target.
    */
    public void RightClick(RaycastHit hitInfo, GameObject gameObject){
        Vector3 dest = hitInfo.point;
        //dest.y = player.myCollider.bounds.center.y;
        // If the player clicked an enemy set the target, otherwise set the destination.
        if(hitInfo.collider.tag == "Enemy" && hitInfo.collider.gameObject != gameObject && hitInfo.collider.enabled){
            _pm.TargetedEnemy = hitInfo.collider.gameObject;
        }
        else{
            _pm.TargetedEnemy = null;
            _pm.Destination = dest;
        }
    }

    /*
    *   PlayerLookDirection - Turns the players transforms forward direction in the direction they are moving.
    *   @param nextTarget - Vector3 of the direction to look at.
    */
    public void PlayerLookDirection(Vector3 nextTarget){
        if(_pm.CurrentTarget != nextTarget){
            //nextTarget.y = player.myCollider.bounds.center.y;
            _pm.CurrentTarget = nextTarget;
        }
    }

    /*
    *   MovePlayerToEnemy - Moves the player into range of their targeted enemy whenever they have one.
    */
    public void MovePlayerToEnemy(){
        if(!_pm.TargetedEnemy.GetComponent<Unit>().isDead){}
        // Get the targets distance from the player.
        Vector3 myTarget = _pm.TargetedEnemy.transform.position;
        myTarget.y = 0.0f;
        float distToEnemy = (_pm.Position - myTarget).magnitude;
        // If the enemy is in auto range then start autoing.
        if(distToEnemy < _pm.Range){
            // Stop navmesh
            _pm.Destination = _pm.Position;
            // If the time since last auto is greater than the next time the player is allowed to auto.
            // Make sure player isn't already winding up an auto.
            /*if(Time.time > player.basicAttack.nextAuto && !player.basicAttack.windingUp){
                player.basicAttack.windingUp = true;
                StartCoroutine(player.basicAttack.BasicAttackWindUp());
            }*/
        }
        else{
            // Stop the auto wind up since the enemy is no longer in range.
            //StopCoroutine(player.basicAttack.BasicAttackWindUp());
            //player.basicAttack.windingUp = false;
            // Move the player into range of the target.
            Vector3 enemyDest = _pm.TargetedEnemy.transform.position;
            //enemyDest.y = player.myCollider.bounds.center.y;
            _pm.Destination = enemyDest;
        }
    }
}
