using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : Unit
{
    public Inventory inventory;
    public Score score;

    protected override void Init(){
        unitStats = new ChampionStats((ScriptableChampion)unit);
        inventory = new Inventory(this);
        score = new Score();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /*
    *   TakeDamage - Damages the unit.
    *   @param incomingDamage - float of the incoming damage amount.
    *   @param damageType - string of the type of damage that is being inflicted.
    *   @param from - GameObject of the damage source.
    */
    public override void TakeDamage(float incomingDamage, string damageType, GameObject from, bool isDot){
        Unit fromUnit = from.GetComponent<Unit>();
        float damageToTake = DamageCalculator.CalculateDamage(incomingDamage, damageType, fromUnit.unitStats, unitStats);
        unitStats.SetHealth(unitStats.currentHealth - damageToTake);
        Debug.Log(transform.name + " took " + damageToTake + " " + damageType + " damage from " + from.transform.name);
        // If dead then award a kill and start the death method.
        if(unitStats.currentHealth <= 0f){
            isDead = true;
            Debug.Log(transform.name + " killed by " + from.transform.name);
            GetComponent<RespawnDeath>().Death();
            if(fromUnit is Player){
                Player killer = (Player) fromUnit;
                killer.score.ChampionKill(gameObject);
                killer.uiManager.UpdateKills(killer.score.kills.ToString());
                // Grant any assists if the unit is a champion.
                foreach(GameObject assist in GetComponent<DamageTracker>().CheckForAssists()){
                    if(assist != from)
                        assist.GetComponent<Player>().score.Assist();
                }
            }
            score.Death();
            uiManager.UpdateDeaths(score.deaths.ToString());
        }
        // Apply any damage that procs after recieving damage.
        else{
            bonusDamage?.Invoke(gameObject, isDot);
        }
        GetComponent<DamageTracker>().DamageTaken(from, incomingDamage, damageType);
        GetComponent<UIManager>().UpdateHealthBar();
    }
}
