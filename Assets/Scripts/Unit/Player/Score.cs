using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
* Purpose: Implements a score object for a unit.
*
* @author: Colin Keys
*/
public class Score
{
    private TMP_Text killsText = null;
    private TMP_Text assistsText = null;
    private TMP_Text deathsText = null;
    private TMP_Text csText = null;

    private int kills = 0;
    public int Kills { 
        get => kills;
        set {
            kills = value;
            if(killsText != null)
                killsText.SetText(value.ToString());
        }
    }
    private int assists = 0;
    public int Assists { 
        get => assists;
        set {
            assists = value;
            if(assistsText != null)
                assistsText.SetText(value.ToString());
        }
    }
    private int deaths = 0;
    public int Deaths { 
        get => deaths;
        set {
            deaths = value;
            if(deathsText != null)
                deathsText.SetText(value.ToString());
        }
    }
    private int cs = 0;
    public int CS { 
        get => cs;
        set {
            cs = value;
            if(csText != null)
                csText.SetText(value.ToString());
        }
    }

    public delegate void Takedown(IUnit killed);
    public event Takedown takedownCallback;

    /*
    *   Score - Creates a new Score object.
    *   @param scoreUI - Transform of the scores UI parent.
    */
    public Score(Transform scoreUI){
        if(scoreUI != null){
            killsText = scoreUI.Find("Kills/Value").GetComponent<TMP_Text>();
            assistsText = scoreUI.Find("Assists/Value").GetComponent<TMP_Text>();
            deathsText = scoreUI.Find("Deaths/Value").GetComponent<TMP_Text>();
            csText = scoreUI.Find("CS/Value").GetComponent<TMP_Text>();
        }
    }

    /*
    *   ChampionKill - Adds a champion kill.
    *   @param killed - IUnit of the killed unit.
    */
    public void ChampionKill(IUnit killed){
        Kills += 1;
        takedownCallback?.Invoke(killed);
    }

    /*
    *   CreepKill - Adds a creep kill.
    *   @param killed - IUnit of the killed unit.
    */
    public void CreepKill(IUnit killed){
        CS += 1;
        takedownCallback?.Invoke(killed);
    }

    /*
    *   Assist - Adds an assist.
    *   @param killed - IUnit of the killed unit.
    */
    public void Assist(IUnit killed){
        Assists += 1;
        takedownCallback?.Invoke(killed);
    }

    /*
    *   Death - Adds a death.
    */
    public void Death(){
        Deaths += 1;
    }
}
