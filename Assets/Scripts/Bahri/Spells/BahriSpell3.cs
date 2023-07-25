using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BahriSpell3 : DamageSpell
{
    private BahriSpell3Data spellData;

    public BahriSpell3(ChampionSpells championSpells, string spellNum, SpellData spellData) : base(championSpells, spellNum){
        this.spellData = (BahriSpell3Data) spellData;
    }

    /*
    *   Spell_3 - Sets up and creates the players third spell GameObject. The spell casts a GameObject in the target direction and 'Charms' the first enemy hit,
    *   disabling their actions and moving them towards Bahri at a decreased move speed.
    */
    public override void Cast(){
        if(!onCd && !player.isCasting && championStats.currentMana >= spellData.baseMana[levelManager.spellLevels[spellNum]-1]){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = GetTargetDirection();
            // Set the target position to be in the direction of the mouse on cast and at max spell distance from the player.
            Vector3 targetPosition = (targetDirection - gameObject.transform.position).normalized;
            targetPosition = gameObject.transform.position + (targetPosition * spellData.magnitude);
            // Start coroutines to handle the spells cast time and animation.
            championSpells.StartCoroutine(CastTime(spellData.castTime, canMove));
            championSpells.StartCoroutine(Spell_3_Move(targetPosition));
            // Use mana and set the spell to be on cooldown.
            championStats.UseMana(spellData.baseMana[levelManager.spellLevels[spellNum]-1]);
            onCd = true;
        }
    }

    /*
    *   Spell_3_Move - Animates the spell in the direction of the target position.
    *   @param targetPosition - Vector3 of the target position to cast the spell towards.
    */
    private IEnumerator Spell_3_Move(Vector3 targetPosition){
        // Wait for cast time.
        while(player.isCasting)
            yield return null;
        // Cooldown stats on cast.
        championSpells.StartCoroutine(Spell_Cd_Timer(spellData.baseCd[levelManager.spellLevels[spellNum]-1], spellNum));  
        // Create spell 3 GameObject and set its necessary variables.
        GameObject missile = (GameObject) GameObject.Instantiate(spellData.missile, gameObject.transform.position, Quaternion.identity);
        //SpellObjectCreated(spell_3_object);
        Spell3Trigger spell3Trigger = missile.GetComponent<Spell3Trigger>();
        spell3Trigger.SetBahriSpell3(this);
        spell3Trigger.SetBahri(gameObject);
        // While the spell object still exists.
        while(missile){
            // If target location has not been reached then move the object towards the target location.
            if(missile.transform.position != targetPosition){
                missile.transform.position = Vector3.MoveTowards(missile.transform.position, targetPosition, spellData.speed * Time.deltaTime);
            }
            else{
                GameObject.Destroy(missile);
            }
            yield return null;
        }
    }

    /*
    *   Spell_3_Hit - Deals third spells damage to the enemy hit. Applies a charm effect on target hit.
    *   @param enemy - GameObject of the enemy hit.
    */
    public override void Hit(GameObject enemy){
        float magicDamage = championStats.magicDamage.GetValue();
        // Add the charm effect to the hit GameObject.
        Unit enemyUnit = enemy.GetComponent<Unit>();
        enemyUnit.statusEffects.AddEffect(spellData.charmEffect.InitializeEffect(levelManager.spellLevels[spellNum]-1, gameObject, enemy));
        enemyUnit.TakeDamage(spellData.baseDamage[levelManager.spellLevels[spellNum]-1] + magicDamage, "magic", gameObject, false);
    }
}
