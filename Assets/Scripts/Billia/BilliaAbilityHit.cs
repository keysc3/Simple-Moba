using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaAbilityHit : MonoBehaviour
{
    private ChampionStats championStats;
    private BilliaAbilities billiaAbilities;
    private Billia billia;
    private LevelManager levelManager;

    // Called when the script instance is being loaded.
    private void Awake(){
        championStats = GetComponent<ChampionStats>();
        billiaAbilities = GetComponent<BilliaAbilities>();
        levelManager = GetComponent<LevelManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        billia = (Billia) championStats.unit;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spell_4_SleepProc(GameObject enemy){
        // TODO: Calculate damage value.
        if(enemy.GetComponent<StatusEffectManager>().CheckForEffect(billiaAbilities.sleep, gameObject)){
            enemy.GetComponent<StatusEffectManager>().RemoveEffect(billiaAbilities.sleep, gameObject);
            enemy.GetComponent<UnitStats>().bonusDamage -= Spell_4_SleepProc;
            enemy.GetComponent<UnitStats>().TakeDamage(20f, "magic", gameObject);
        }
    }

}
