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

    public void Spell_1_Hit(GameObject enemy, string radius){
        billiaAbilities.Passive(enemy);
        float magicDamage = championStats.magicDamage.GetValue();
        if(radius == "inner")
            enemy.GetComponent<UnitStats>().TakeDamage(billia.spell1BaseDamage[levelManager.spellLevels["Spell_1"]-1] + magicDamage, "magic", gameObject, false);   
        else{
            enemy.GetComponent<UnitStats>().TakeDamage(billia.spell1BaseDamage[levelManager.spellLevels["Spell_1"]-1] + magicDamage, "magic", gameObject, false);
            enemy.GetComponent<UnitStats>().TakeDamage(billia.spell1BaseDamage[levelManager.spellLevels["Spell_1"]-1] + magicDamage, "true", gameObject, false);
        }
    }

    public void Spell_2_Hit(GameObject enemy, string radius){
        billiaAbilities.Passive(enemy);
        float magicDamage = championStats.magicDamage.GetValue();
        if(radius == "inner")
            enemy.GetComponent<UnitStats>().TakeDamage((billia.spell2BaseDamage[levelManager.spellLevels["Spell_2"]-1] + magicDamage) * 2f, "magic", gameObject, false);   
        else
            enemy.GetComponent<UnitStats>().TakeDamage(billia.spell2BaseDamage[levelManager.spellLevels["Spell_2"]-1] + magicDamage, "magic", gameObject, false);
    }

    public void Spell_3_Hit(GameObject enemy){
        billiaAbilities.Passive(enemy);
        float magicDamage = championStats.magicDamage.GetValue();
        enemy.GetComponent<StatusEffectManager>().AddEffect(slowEffect.InitializeEffect(gameObject, enemy));
        enemy.GetComponent<UnitStats>().TakeDamage(billia.spell3BaseDamage[levelManager.spellLevels["Spell_3"]-1] + magicDamage, "magic", gameObject, false);   
    }

    public void Spell_4_SleepProc(GameObject enemy, bool isDot){
        if(!isDot){
            if(enemy.GetComponent<StatusEffectManager>().CheckForEffect(billiaAbilities.sleep, gameObject)){
                float magicDamage = championStats.magicDamage.GetValue();
                enemy.GetComponent<StatusEffectManager>().RemoveEffect(billiaAbilities.sleep, gameObject);
                enemy.GetComponent<UnitStats>().bonusDamage -= Spell_4_SleepProc;
                enemy.GetComponent<UnitStats>().TakeDamage(billia.spell4BaseDamage[levelManager.spellLevels["Spell_4"]-1] + magicDamage, "magic", gameObject, false);
            }
            else{
                enemy.GetComponent<UnitStats>().bonusDamage -= Spell_4_SleepProc;
            }
        }
    }

}
