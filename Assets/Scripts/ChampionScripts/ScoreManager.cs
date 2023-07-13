using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Handles updating the players score (kills, deaths, creep score).
*
* @author: Colin Keys
*/
public class ScoreManager : MonoBehaviour
{
    
    public delegate void Takedown(GameObject killed);
    public event Takedown takedownCallback;
    
    private Score score;
    private UIManager uiManager;

    // Called when the script instance is being loaded.
    private void Awake(){
        score = new Score();
        uiManager = GetComponent<UIManager>();
    }

    /*
    *   Kill - Updates the champion kills or creep score depending on what unit was killed.
    *   @param killed - GameObject of the unit killed.
    */
    public void Kill(GameObject killed){
        // If the unit is a champion add a kill and update the kill UI.
        if(killed.GetComponent<UnitStats>().unit is Champion){
            score.ChampionKill();
            uiManager.UpdateKills(score.kills.ToString());
        }
        // Update the creeps score and creep score UI.
        else{
            score.CreepKill();
            uiManager.UpdateCS(score.cs.ToString());
        }
        // Invoke the takedown (kill) callback.
        takedownCallback?.Invoke(killed);
    }

    /*
    *   Death - Updates the players deaths and deaths UI.
    */
    public void Death(){
        score.Death();
        uiManager.UpdateDeaths(score.deaths.ToString());
    }

    /*
    *   Assist - Updates the players assists and assist UI.
    */
    public void Assist(){
        Debug.Log("Assist for " + gameObject.name + ".");
        score.Assist();
        uiManager.UpdateAssists(score.assists.ToString());
    }
}
