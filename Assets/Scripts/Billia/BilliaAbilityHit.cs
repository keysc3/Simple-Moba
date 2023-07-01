using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Use damage values in scriptiable billia.
public class BilliaAbilityHit : MonoBehaviour
{
    [field: SerializeField] public ScriptableSlow slowEffect { get; private set; }

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
    public void Spell_3_Hit(GameObject enemy){
        billiaAbilities.Passive(enemy);
        float magicDamage = championStats.magicDamage.GetValue();
        enemy.GetComponent<StatusEffectManager>().AddEffect(slowEffect.InitializeEffect(gameObject, enemy));
        enemy.GetComponent<UnitStats>().TakeDamage(billia.spell3BaseDamage[levelManager.spellLevels["Spell_3"]-1] + magicDamage, "magic", gameObject, false);   
    }

    public void Spell_4_SleepProc(GameObject enemy, bool isDot){
        // TODO: Calculate damage value.
        if(!isDot){
            if(enemy.GetComponent<StatusEffectManager>().CheckForEffect(billiaAbilities.sleep, gameObject)){
                enemy.GetComponent<StatusEffectManager>().RemoveEffect(billiaAbilities.sleep, gameObject);
                enemy.GetComponent<UnitStats>().bonusDamage -= Spell_4_SleepProc;
                enemy.GetComponent<UnitStats>().TakeDamage(20f, "magic", gameObject, false);
            }
            else{
                enemy.GetComponent<UnitStats>().bonusDamage -= Spell_4_SleepProc;
            }
        }
    }

}
