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
            if(value > 0)
                kills = value;
        }
    }
    private int assists = 0;
    public int Assists { 
        get => assists;
        set {
            if(value > 0)
                assists = value;
        }
    }
    private int deaths = 0;
    public int Deaths { 
        get => deaths;
        set {
            if(value > 0)
                deaths = value;
        }
    }
    private int cs = 0;
    public int CS { 
        get => cs;
        set {
            if(value > 0)
                cs = value;
        }
    }

    public delegate void Takedown(IUnit killed);
    public event Takedown takedownCallback;

    public delegate void UpdateScoreUI(Score score, string toUpdate);
    public event UpdateScoreUI UpdateScoreCallback;

    /*
    *   ChampionKill - Adds a champion kill.
    *   @param killed - IUnit of the killed unit.
    */
    public void ChampionKill(IUnit killed){
        Kills += 1;
        takedownCallback?.Invoke(killed);
        UpdateScoreCallback?.Invoke(this, "kill");
    }

    /*
    *   CreepKill - Adds a creep kill.
    *   @param killed - IUnit of the killed unit.
    */
    public void CreepKill(IUnit killed){
        CS += 1;
        takedownCallback?.Invoke(killed);
        UpdateScoreCallback?.Invoke(this, "cs");
    }

    /*
    *   Assist - Adds an assist.
    *   @param killed - IUnit of the killed unit.
    */
    public void Assist(IUnit killed){
        Assists += 1;
        takedownCallback?.Invoke(killed);
        UpdateScoreCallback?.Invoke(this, "assist");
    }

    /*
    *   Death - Adds a death.
    */
    public void Death(){
        Deaths += 1;
        UpdateScoreCallback?.Invoke(this, "death");
    }
}
