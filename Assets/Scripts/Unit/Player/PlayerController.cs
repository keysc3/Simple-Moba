using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*   Purpose: Handles a players movement and non-spell methods.
*
*   @author: Colin Keys
*/
public class PlayerController
{
    private IPlayerMover playerMover;
    private IPlayer player;

    public PlayerController(IPlayerMover playerMover, IPlayer player){
        this.playerMover = playerMover;
        this.player = player;
    }
    
    /*
    *   RightClick - Moves the player to the click position if no enemy is targeted, otherwise sets the target.
    */
    public void RightClick(IUnit hit, Vector3 targetDest){
        if(hit != null){
            // If the player clicked an enemy set the target, otherwise set the destination.
            if(hit.GameObject.tag == "Enemy" && hit != player && !hit.IsDead){
                playerMover.TargetedEnemy = hit;
            }
        }
        else{
            playerMover.TargetedEnemy = null;
            playerMover.Destination = targetDest;
        }
    }

    /*
    *   PlayerLookDirection - Turns the players transforms forward direction in the direction they are moving or casting.
    *   @param nextTarget - Vector3 of the direction to look at.
    */
    public void PlayerLookDirection(){
        // The player should never be casting something if current spell is null but just to be safe check for null.
        if(player.IsCasting && player.CurrentCastedSpell != null && !player.CurrentCastedSpell.CanMove)
            playerMover.CurrentTarget = player.MouseOnCast;
        else
            playerMover.CurrentTarget = playerMover.NextDestination;
    }

    /*
    *   MovePlayerToEnemy - Moves the player into range of their targeted enemy whenever they have one.
    */
    public void MovePlayerToEnemy(){
        if(playerMover.TargetedEnemy != null && !player.IsCasting){
            if(!playerMover.TargetedEnemy.IsDead){
                // Get the targets distance from the player.
                Vector3 myTarget = playerMover.TargetedEnemy.Position;
                myTarget.y = 0.0f;
                float distToEnemy = (player.Position - myTarget).magnitude;
                // If the enemy is in auto range then start autoing.
                if(distToEnemy < player.unitStats.autoRange.GetValue()){
                    // Stop navmesh
                    playerMover.Destination = player.Position;
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
                    Vector3 enemyDest = playerMover.TargetedEnemy.Position;
                    //enemyDest.y = player.myCollider.bounds.center.y;
                    playerMover.Destination = enemyDest;
                }
            }
            else{
                playerMover.TargetedEnemy = null;
                playerMover.Destination = player.Position;
            }
        }
    }

    public void StopPlayer(){
        playerMover.TargetedEnemy = null;
        playerMover.Destination = player.Position;
    }
}
