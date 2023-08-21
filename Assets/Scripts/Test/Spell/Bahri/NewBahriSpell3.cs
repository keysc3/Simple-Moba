using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
* Purpose: Implements Bahri'a third spell. Bahri throws out a charm at a target direction. The first unit hit takes damage and gets charmed.
*
* @author: Colin Keys
*/
public class NewBahriSpell3 : InterSpell, IHasCast, IHasHit
{
    new private BahriSpell3Data spellData;
    public SpellHitCallback spellHitCallback { get; set; }

    /*
    *   BahriSpell3 - Initialize Bahri's third spell.
    *   @param championSpells - ChampionSpells instance this spell is a part of.
    *   @param spellNum - string of the spell number this spell is.
    *   @param spellData - SpellData to use.
    */
    /*public BahriSpell3(ChampionSpells championSpells, SpellData spellData) : base(championSpells, spellData){
        this.spellData = (BahriSpell3Data) spellData;
    }*/

    void Start(){
        if(SpellNum == null)
            SpellNum = "Spell_3";
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Vector3 targetPosition = (sc.GetTargetDirection() - transform.position).normalized;
        targetPosition = transform.position + (targetPosition * spellData.magnitude);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, targetPosition);
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = sc.GetTargetDirection();
            // Set the target position to be in the direction of the mouse on cast and at max spell distance from the player.
            Vector3 targetPosition = (targetDirection - transform.position).normalized;
            targetPosition = transform.position + (targetPosition * spellData.magnitude);
            // Start coroutines to handle the spells cast time and animation.
            StartCoroutine(sc.CastTime(spellData.castTime));
            StartCoroutine(Spell_3_Move(targetPosition));
            // Use mana and set the spell to be on cooldown.
            championStats.UseMana(spellData.baseMana[SpellLevel]);
            OnCd = true;
        }
    }

    /*
    *   Spell_3_Move - Animates the spell in the direction of the target position.
    *   @param targetPosition - Vector3 of the target position to cast the spell towards.
    */
    private IEnumerator Spell_3_Move(Vector3 targetPosition){
        // Wait for cast time.
        while(player.IsCasting)
            yield return null;
        // Cooldown stats on cast.
        StartCoroutine(sc.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));  
        // Create spell 3 GameObject and set its necessary variables.
        GameObject missile = (GameObject) GameObject.Instantiate(spellData.missile, transform.position, Quaternion.identity);
        NewBahriSpell3Trigger spell3Trigger = missile.GetComponent<NewBahriSpell3Trigger>();
        spell3Trigger.hitMethod = this;
        spell3Trigger.caster = player;
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
    public void Hit(IUnit unit){
        spellHitCallback?.Invoke(unit, this);
        if(unit is INewDamagable){
            float magicDamage = championStats.magicDamage.GetValue();
            // Add the charm effect to the hit GameObject.
            //unit.statusEffects.AddEffect(spellData.charmEffect.InitializeEffect(SpellLevel, gameObject, unit));
            ((INewDamagable) unit).TakeDamage(spellData.baseDamage[SpellLevel] + magicDamage, "magic", player, false);
        }
    }
}

