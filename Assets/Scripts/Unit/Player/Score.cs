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
    public int kills { get; private set; } = 0;
    public int assists { get; private set; } = 0;
    public int deaths { get; private set; } = 0;
    public int cs { get; private set; } = 0;

    public delegate void Takedown(GameObject killed);
    public event Takedown takedownCallback;

    /*
    *   ChampionKill - Adds a champion kill.
    *   @param killed - GameObject of the killed unit.
    */
    public void ChampionKill(GameObject killed){
        kills += 1;
        takedownCallback?.Invoke(killed);
    }

    /*
    *   CreepKill - Adds a creep kill.
    *   @param killed - GameObject of the killed unit.
    */
    public void CreepKill(GameObject killed){
        cs += 1;
        takedownCallback?.Invoke(killed);
    }

    /*
    *   Assist - Adds an assist.
    */
    public void Assist(){
        assists += 1;
    }

    /*
    *   Assist - Adds a death.
    */
    public void Death(){
        deaths +=1;
    }

}
