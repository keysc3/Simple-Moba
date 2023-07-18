using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Handles one of Billia's spells hitting a unit.
* @author: Colin Keys
*/
public class BilliaAbilityHit : MonoBehaviour
{
    [field: SerializeField] public ScriptableSlow slowEffect { get; private set; }

    private ChampionStats championStats;
    private BilliaAbilities billiaAbilities;
    private Billia billia;
    private Player player;
    private LevelManager levelManager;

    // Called when the script instance is being loaded.
    private void Awake(){
        player = GetComponent<Player>();
        billiaAbilities = GetComponent<BilliaAbilities>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        levelManager = player.levelManager;
        championStats = (ChampionStats) player.unitStats;
        billia = (Billia) player.unit;
    }

    /*
    *   Spell_1_Hit - Deals first spells damage to the enemy hit. Magic damage with additional true damage on outer hit.
    *   @param enemy - GameObject of the enemy hit.
    *   @param radius - string of which radius was hit.
    */
    public void Spell_1_Hit(GameObject enemy, string radius){
        billiaAbilities.Passive(enemy);
        float magicDamage = championStats.magicDamage.GetValue();
        Unit enemyUnit = enemy.GetComponent<Unit>();
        if(radius == "inner")
            enemyUnit.TakeDamage(billia.spell1BaseDamage[levelManager.spellLevels["Spell_1"]-1] + magicDamage, "magic", gameObject, false);   
        else{
            enemyUnit.TakeDamage(billia.spell1BaseDamage[levelManager.spellLevels["Spell_1"]-1] + magicDamage, "magic", gameObject, false);
            enemyUnit.TakeDamage(billia.spell1BaseDamage[levelManager.spellLevels["Spell_1"]-1] + magicDamage, "true", gameObject, false);
        }
    }

    /*
    *   Spell_2_Hit - Deals second spells damage to the enemy hit. Magic damage with inner hit dealing increased magic damage.
    *   @param enemy - GameObject of the enemy hit.
    *   @param radius - string of which radius was hit.
    */
    public void Spell_2_Hit(GameObject enemy, string radius){
        billiaAbilities.Passive(enemy);
        float magicDamage = championStats.magicDamage.GetValue();
        if(radius == "inner")
            enemy.GetComponent<Unit>().TakeDamage((billia.spell2BaseDamage[levelManager.spellLevels["Spell_2"]-1] + magicDamage) * 2f, "magic", gameObject, false);   
        else
            enemy.GetComponent<Unit>().TakeDamage(billia.spell2BaseDamage[levelManager.spellLevels["Spell_2"]-1] + magicDamage, "magic", gameObject, false);
    }

    /*
    *   Spell_3_Hit - Deals third spells damage to the enemy hit. Magic damage with a slow on hit.
    *   @param enemy - GameObject of the enemy hit.
    */
    public void Spell_3_Hit(GameObject enemy){
        billiaAbilities.Passive(enemy);
        float magicDamage = championStats.magicDamage.GetValue();
        Unit enemyUnit = enemy.GetComponent<Unit>();
        enemyUnit.statusEffects.AddEffect(slowEffect.InitializeEffect(levelManager.spellLevels["Spell_3"]-1, gameObject, enemy));
        enemyUnit.TakeDamage(billia.spell3BaseDamage[levelManager.spellLevels["Spell_3"]-1] + magicDamage, "magic", gameObject, false);   
    }

    /*
    *   Spell_4_SleepProc - Deals fourth spells damage to the enemy hit. Magic damage if target has sleep effect.
    *   @param enemy - GameObject of the enemy hit.
    *   @param isDot - bool of whether or not the damage taken was from a dot.
    */
    public void Spell_4_SleepProc(GameObject enemy, bool isDot){
        // Dots do not proc the sleep.
        if(!isDot){
            Unit enemyUnit = enemy.GetComponent<Unit>();
            if(enemyUnit.statusEffects.CheckForEffectWithSource(billiaAbilities.sleep, gameObject)){
                float magicDamage = championStats.magicDamage.GetValue();
                // Remove sleep, deal damage and remove function from delegate.
                enemyUnit.statusEffects.RemoveEffect(billiaAbilities.sleep, gameObject);
                enemyUnit.bonusDamage -= Spell_4_SleepProc;
                enemyUnit.TakeDamage(billia.spell4BaseDamage[levelManager.spellLevels["Spell_4"]-1] + magicDamage, "magic", gameObject, false);
            }
            // If effect fell off before damage was dealt, remove the bonus damage method.
            else{
                enemyUnit.bonusDamage -= Spell_4_SleepProc;
            }
        }
    }

}
