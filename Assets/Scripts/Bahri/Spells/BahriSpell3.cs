using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements Bahri'a third spell. Bahri throws out a charm at a target direction. The first unit hit takes damage and gets charmed.
*
* @author: Colin Keys
*/
public class BahriSpell3 : DamageSpell, ICastable
{
    new private BahriSpell3Data spellData;

    /*
    *   BahriSpell3 - Initialize Bahri's third spell.
    *   @param championSpells - ChampionSpells instance this spell is a part of.
    *   @param spellNum - string of the spell number this spell is.
    *   @param spellData - SpellData to use.
    */
    public BahriSpell3(ChampionSpells championSpells, SpellData spellData) : base(championSpells, spellData){
        this.spellData = (BahriSpell3Data) spellData;
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Vector3 targetPosition = (GetTargetDirection() - player.gameObject.transform.position).normalized;
        targetPosition = player.gameObject.transform.position + (targetPosition * spellData.magnitude);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(player.gameObject.transform.position, targetPosition);
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        if(!player.isCasting && championStats.CurrentMana >= spellData.baseMana[player.levelManager.spellLevels[spellNum]-1]){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = GetTargetDirection();
            // Set the target position to be in the direction of the mouse on cast and at max spell distance from the player.
            Vector3 targetPosition = (targetDirection - player.gameObject.transform.position).normalized;
            targetPosition = player.gameObject.transform.position + (targetPosition * spellData.magnitude);
            // Start coroutines to handle the spells cast time and animation.
            championSpells.StartCoroutine(CastTime(spellData.castTime, canMove));
            championSpells.StartCoroutine(Spell_3_Move(targetPosition));
            // Use mana and set the spell to be on cooldown.
            championStats.UseMana(spellData.baseMana[player.levelManager.spellLevels[spellNum]-1]);
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
        championSpells.StartCoroutine(Spell_Cd_Timer(spellData.baseCd[player.levelManager.spellLevels[spellNum]-1], spellNum));  
        // Create spell 3 GameObject and set its necessary variables.
        GameObject missile = (GameObject) GameObject.Instantiate(spellData.missile, player.gameObject.transform.position, Quaternion.identity);
        Spell3Trigger spell3Trigger = missile.GetComponent<Spell3Trigger>();
        spell3Trigger.bahriSpell3 = this;
        spell3Trigger.bahri = player.gameObject;
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
    *   Hit - Deals third spells damage to the enemy hit. Applies a charm effect on target hit.
    *   @param enemy - GameObject of the enemy hit.
    */
    public override void Hit(GameObject enemy){
        float magicDamage = championStats.magicDamage.GetValue();
        // Add the charm effect to the hit GameObject.
        Unit enemyUnit = enemy.GetComponent<Unit>();
        enemyUnit.statusEffects.AddEffect(spellData.charmEffect.InitializeEffect(player.levelManager.spellLevels[spellNum]-1, player.gameObject, enemy));
        enemyUnit.TakeDamage(spellData.baseDamage[player.levelManager.spellLevels[spellNum]-1] + magicDamage, "magic", player.gameObject, false);
    }
}
