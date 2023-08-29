using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
* Purpose: Implements Bahri'a third spell. Bahri throws out a charm at a target direction. The first unit hit takes damage and gets charmed.
*
* @author: Colin Keys
*/
public class BahriSpell3 : Spell, IHasCast, IHasHit
{
    public SpellHitCallback spellHitCallback { get; set; }

    new private BahriSpell3Data spellData;

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BahriSpell3Data) base.spellData;
        if(SpellNum == null)
            SpellNum = spellData.defaultSpellNum;
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Vector3 targetPosition = (spellController.GetTargetDirection() - transform.position).normalized;
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
            Vector3 targetDirection = spellController.GetTargetDirection();
            // Set the target position to be in the direction of the mouse on cast and at max spell distance from the player.
            Vector3 targetPosition = (targetDirection - transform.position).normalized;
            targetPosition = transform.position + (targetPosition * spellData.magnitude);
            // Start coroutines to handle the spells cast time and animation.
            StartCoroutine(spellController.CastTime(spellData.castTime));
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
        StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));  
        // Create spell 3 GameObject and set its necessary variables.
        GameObject missile = (GameObject) Instantiate(spellData.missile, transform.position, Quaternion.identity);
        BahriSpell3Trigger spell3Trigger = missile.GetComponent<BahriSpell3Trigger>();
        spell3Trigger.hitMethod = this;
        spell3Trigger.caster = player;
        // While the spell object still exists.
        while(missile){
            // If target location has not been reached then move the object towards the target location.
            if(missile.transform.position != targetPosition){
                missile.transform.position = Vector3.MoveTowards(missile.transform.position, targetPosition, spellData.speed * Time.deltaTime);
            }
            else{
                Destroy(missile);
            }
            yield return null;
        }
    }

    /*
    *   Hit - Deals third spells damage to the enemy hit. Applies a charm effect on target hit.
    *   @param unit - IUnit of the enemy hit.
    */
    public void Hit(IUnit unit){
        spellHitCallback?.Invoke(unit, this);
        if(unit is IDamageable){
            float magicDamage = championStats.magicDamage.GetValue();
            // Add the charm effect to the hit GameObject.
            unit.statusEffects.AddEffect(spellData.charmEffect.InitializeEffect(SpellLevel, player, unit));
            ((IDamageable) unit).TakeDamage(spellData.baseDamage[SpellLevel] + magicDamage, "magic", player, false);
        }
    }
}

