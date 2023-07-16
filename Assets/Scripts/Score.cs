using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a score object for a unit.
*
* @author: Colin Keys
*/
public class Score
{
    public int kills;
    public int assists;
    public int deaths;
    public int cs;

    public delegate void Takedown(GameObject killed);
    public event Takedown takedownCallback;

    public Score(){
        kills = 0;
        assists = 0;
        deaths = 0;
        cs = 0;
    }

    public void ChampionKill(GameObject killed){
        kills += 1;
        takedownCallback?.Invoke(killed);
    }

    public void CreepKill(GameObject killed){
        cs += 1;
        takedownCallback?.Invoke(killed);
    }

    public void Death(){
        deaths +=1;
    }

    public void Assist(){
        assists += 1;
    }

}
