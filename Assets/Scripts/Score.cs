using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score
{
    public int kills;
    public int assists;
    public int deaths;
    public int cs;

    public Score(){
        kills = 0;
        assists = 0;
        deaths = 0;
        cs = 0;
    }

    public void ChampionKill(){
        kills += 1;
    }

    public void CreepKill(){
        cs += 1;
    }

    public void Death(){
        deaths +=1;
    }

    public void Assist(){
        assists += 1;
    }

}
