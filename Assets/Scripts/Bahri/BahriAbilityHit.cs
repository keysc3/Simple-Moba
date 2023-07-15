using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Handles the effects and damage for one of Bahri's spells hitting a unit.
*
* @author: Colin Keys
*/
public class BahriAbilityHit : MonoBehaviour
{
    [field: SerializeField] public ScriptableCharm charmEffect { get; private set; }

    private List<GameObject> spell_1_enemiesHit = new List<GameObject>();
    private List<GameObject> spell_2_enemiesHit = new List<GameObject>();
    private ChampionStats championStats;
    private ChampionAbilities bahriAbilities;
    private Bahri bahri;
    private LevelManager levelManager;

    // Called when the script instance is being loaded.
    private void Awake(){
        bahriAbilities = GetComponent<BahriAbilities>();
        levelManager = GetComponent<LevelManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        championStats = (ChampionStats) GetComponent<Player>().unitStats;
        bahri = (Bahri) championStats.unit;
    }

    // Update is called once per frame
    private void Update()
    {
        // For testing charm effect.
        if(Input.GetKeyDown(KeyCode.Z) && ActiveChampion.instance.champions[ActiveChampion.instance.activeChampion] == gameObject)
            GetComponent<Unit>().statusEffects.AddEffect(charmEffect.InitializeEffect(levelManager.spellLevels["Spell_3"]-1, gameObject.transform.Find("/Champions/Red Bahri").gameObject, gameObject));
    }

    /*
    *   Spell_1_Hit - Deals first spells damage to the enemy hit. Magic damage on first part then true damage on return.
    *   @param enemy - GameObject of the enemy hit.
    *   @param isReturning - bool for if the orb is returning or not.
    */
    public void Spell_1_Hit(GameObject enemy, bool isReturning){
        float magicDamage = championStats.magicDamage.GetValue();
        float damageValue = 0;
        // Only want to hit an enemy once per way.
        if(!spell_1_enemiesHit.Contains(enemy)){
            damageValue = bahri.spell1BaseDamage[levelManager.spellLevels["Spell_1"]-1] + magicDamage;
            // Magic damage on first part then true damage on return.
            if(!isReturning){
                enemy.GetComponent<Unit>().TakeDamage(damageValue, "magic", gameObject, false);
            }
            else{
                enemy.GetComponent<Unit>().TakeDamage(damageValue, "true", gameObject, false);
            }
            spell_1_enemiesHit.Add(enemy);
        }
    }

    /*
    *   Spell_2_Hit - Deals second spells damage to the enemy hit. Reduced damage on missiles that hit the same target more than once.
    *   @param enemy - GameObject of the enemy hit.
    */
    public void Spell_2_Hit(GameObject enemy){
        float magicDamage = championStats.magicDamage.GetValue();
        float finalDamage = bahri.spell2BaseDamage[levelManager.spellLevels["Spell_2"]-1] + magicDamage;
        // Reduce damage of spell if hitting the same target more than once.
        if(spell_2_enemiesHit.Contains(enemy)){
            finalDamage = Mathf.Round(finalDamage * bahri.spell_2_multiplier);
        }
        enemy.GetComponent<Unit>().TakeDamage(finalDamage, "magic", gameObject, false);
        spell_2_enemiesHit.Add(enemy);
    }

    /*
    *   Spell_3_Hit - Deals third spells damage to the enemy hit. Applies a charm effect on target hit.
    *   @param enemy - GameObject of the enemy hit.
    */
    public void Spell_3_Hit(GameObject enemy){
        float magicDamage = championStats.magicDamage.GetValue();
        // Add the charm effect to the hit GameObject.
        enemy.GetComponent<Unit>().statusEffects.AddEffect(charmEffect.InitializeEffect(levelManager.spellLevels["Spell_3"]-1, gameObject, enemy));
        enemy.GetComponent<Unit>().TakeDamage(bahri.spell3BaseDamage[levelManager.spellLevels["Spell_3"]-1] + magicDamage, "magic", gameObject, false);
    }

    /*
    *   Spell_4_Hit - Deals fourth spells damage to the enemy hit.
    *   @param enemy - GameObject of the enemy hit.
    */
    public void Spell_4_Hit(GameObject enemy){
        float magicDamage = championStats.magicDamage.GetValue();
        enemy.GetComponent<Unit>().TakeDamage(bahri.spell4BaseDamage[levelManager.spellLevels["Spell_4"]-1] + magicDamage, "magic", gameObject, false);
    }

    /*
    *   AutoAttack - Deals auto attacks damage to the enemy hit.
    *   @param enemy - GameObject of the enemy hit.
    */
    public void AutoAttack(GameObject enemy){
        float physicalDamage = championStats.physicalDamage.GetValue();
        enemy.GetComponent<Unit>().TakeDamage(physicalDamage, "physical", gameObject, false);
    }

    /*
    *   SpellResetEnemiesHit - Resets a spells enemies hit list.
    *   @param spell - String of which spells list needs to be reset.
    */
    public void SpellResetEnemiesHit(string spell){
        if(spell == "1")
            spell_1_enemiesHit.Clear();
        else
            spell_2_enemiesHit.Clear();
    }
}
