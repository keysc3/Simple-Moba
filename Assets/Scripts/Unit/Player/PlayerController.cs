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
    public void RightClick(IUnit hitUnit, Vector3 hitVec){
        if(hitUnit != null){
            // If the player clicked an enemy set the target, otherwise set the destination.
            if(hitUnit.GameObject.tag == "Enemy" && !hitUnit.IsDead){
                playerMover.TargetedEnemy = hitUnit;
                return;
            }
        }
        playerMover.TargetedEnemy = null;
        playerMover.Destination = hitVec;
    }

    /*
    *   PlayerLookDirection - Turns the players transforms forward direction in the direction they are moving or casting.
    *   @param nextTarget - Vector3 of the direction to look at.
    */
    public void PlayerLookDirection(){
        // The player should never be casting something if current spell is null but just to be safe check for null.
        if(player.IsCasting && player.CurrentCastedSpell != null && !player.CurrentCastedSpell.CanMove)
            playerMover.CurrentTarget = player.MouseOnCast;
        else{
            playerMover.CurrentTarget = playerMover.NextDestination;
        }
    }

    /*
    *   SetPlayerDestinationUsingTarget - Sets the player interfaces destination if a target exists.
    */
    public void SetPlayerDestinationUsingTarget(){
        if(playerMover.TargetedEnemy != null && !player.IsCasting){
            if(!playerMover.TargetedEnemy.IsDead){
                // Get the targets distance from the player.
                Vector3 myTarget = playerMover.TargetedEnemy.Position;
                myTarget.y = player.Position.y;
                float distToEnemy = (player.Position - myTarget).magnitude;
                // If the enemy is in auto range then start autoing.
                if(distToEnemy <= player.unitStats.autoRange.GetValue()){
                    // Stop navmesh
                    playerMover.Destination = player.Position;
                }
                else{
                    // Move the player into range of the target.
                    playerMover.Destination = playerMover.TargetedEnemy.Position;
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
