using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewScore
{
    private TMP_Text killsText = null;
    private TMP_Text assistsText = null;
    private TMP_Text deathsText = null;
    private TMP_Text csText = null;

    private int kills = 0;
    public int Kills { 
        get => kills;
        set {
            if(killsText != null)
                killsText.SetText(kills.ToString());
        }
    }
    private int assists = 0;
    public int Assists { 
        get => assists;
        set {
            if(assistsText != null)
                assistsText.SetText(assists.ToString());
        }
    }
    private int deaths = 0;
    public int Deaths { 
        get => deaths;
        set {
            if(deathsText != null)
                deathsText.SetText(deaths.ToString());
        }
    }
    private int cs = 0;
    public int CS { 
        get => cs;
        set {
            if(csText != null)
                csText.SetText(cs.ToString());
        }
    }

    public delegate void Takedown(IUnit killed);
    public event Takedown takedownCallback;

    public NewScore(Transform scoreUI){
        if(scoreUI != null){
            killsText = scoreUI.Find("Kills/Value").GetComponent<TMP_Text>();
            assistsText = scoreUI.Find("Assists/Value").GetComponent<TMP_Text>();
            deathsText = scoreUI.Find("Deaths/Value").GetComponent<TMP_Text>();
            csText = scoreUI.Find("CS/Value").GetComponent<TMP_Text>();
        }
    }
    /*
    *   ChampionKill - Adds a champion kill.
    *   @param killed - GameObject of the killed unit.
    */
    public void ChampionKill(IUnit killed){
        Kills += 1;
        takedownCallback?.Invoke(killed);
    }

    /*
    *   CreepKill - Adds a creep kill.
    *   @param killed - GameObject of the killed unit.
    */
    public void CreepKill(IUnit killed){
        CS += 1;
        takedownCallback?.Invoke(killed);
    }

    /*
    *   Assist - Adds an assist.
    */
    public void Assist(IUnit killed){
        Assists += 1;
        takedownCallback?.Invoke(killed);
    }

    /*
    *   Assist - Adds a death.
    */
    public void Death(){
        Deaths += 1;
    }
}
