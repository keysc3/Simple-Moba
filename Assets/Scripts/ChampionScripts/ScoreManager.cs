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
    
    private int kills;
    private int assists;
    private int deaths;
    private int cs;
    private UIManager uiManager;

    // Called when the script instance is being loaded.
    private void Awake(){
        uiManager = GetComponent<UIManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        kills = 0;
        assists = 0;
        deaths = 0;
        cs = 0;
    }

    /*
    *   Kill - Updates the champion kills or creep score depending on what unit was killed.
    *   @param killed - GameObject of the unit killed.
    */
    public void Kill(GameObject killed){
        // If the unit is a champion add a kill and update the kill UI.
        if(killed.GetComponent<UnitStats>().unit is Champion){
            kills += 1;
            uiManager.UpdateKills(kills.ToString());
        }
        // Update the creeps score and creep score UI.
        else{
            cs += 1;
            uiManager.UpdateCS(cs.ToString());
        }
        // Invoke the takedown (kill) callback.
        takedownCallback?.Invoke(killed);
    }

    /*
    *   Death - Updates the players deaths and deaths UI.
    */
    public void Death(){
        deaths += 1;
        uiManager.UpdateDeaths(deaths.ToString());
    }

    /*
    *   Assist - Updates the players assists and assist UI.
    */
    public void Assist(){
        Debug.Log("Assist for " + gameObject.name + ".");
        assists += 1;
        uiManager.UpdateAssists(assists.ToString());
    }
}
