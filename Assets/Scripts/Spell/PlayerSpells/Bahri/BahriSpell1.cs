using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements Bahri'a first spell. Bahri throws a damage dealing orb at a target direction that returns after reaching its maximum magnitude.
* Any unit hit on the initial cast takes magic damage and any unit hit by the return takes true damage.
* The orb accelerates upon starting its return until reaching Bahri.
*
* @author: Colin Keys
*/
public class BahriSpell1 : Spell, IHasCast, IHasHit
{
    public bool returning { get; private set; } = false;
    public SpellHitCallback spellHitCallback { get; set; }

    new private BahriSpell1Data spellData;
    private List<IUnit> enemiesHit = new List<IUnit>();

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BahriSpell1Data) base.spellData;
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Vector2 size = new Vector2(spellData.orbScale, spellData.magnitude + spellData.orbScale/2f);
        DrawSpellUIHitbox(0, spellData.magnitude/2f + spellData.orbScale/4f, size, true);
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = spellController.GetTargetDirection();
            player.MouseOnCast = targetDirection;
            // Set the target position to be in the direction of the mouse on cast and at max spell distance from the player.
            Vector3 targetPosition = (targetDirection - transform.position).normalized;
            targetPosition = transform.position + (targetPosition * spellData.magnitude);
            // Start coroutines to handle the spells cast time and animation.
            StartCoroutine(spellController.CastTime(spellData.castTime, spellData.name));
            StartCoroutine(Spell_1_Move(targetPosition));
            // Use mana and set spell on cooldown to true.
            championStats.UseMana(spellData.baseMana[SpellLevel]);
            OnCd = true;
        }
    }

    /*
    *   Spell_1_Move - Animates the players first spell.
    *   @param targetPosition - Vector3 representing the orbs return point.
    */
    private IEnumerator Spell_1_Move(Vector3 targetPosition){
        // Wait for the spells cast time.
        while(player.IsCasting)
            yield return null;
        // Cooldown starts on cast.
        StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
        // Create the spells object and set necessary values.
        GameObject orb = (GameObject) Instantiate(spellData.orb, transform.position, Quaternion.identity);
        orb.transform.localScale = Vector3.one * spellData.orbScale;
        BahriSpell1Trigger spell1Trigger = orb.GetComponentInChildren<BahriSpell1Trigger>();
        spell1Trigger.bahriSpell1 = this;
        spell1Trigger.unit = player; 
        // Set initial return values.
        returning = false;
        float returnSpeed = spellData.minSpeed;
        // While the spell is active.
        while(orb){
            // If the spell hasn't started returning.
            if(!returning){
                // If target location has not been reached then move the orb towards the target location.
                if(orb.transform.position != targetPosition){
                    orb.transform.position = Vector3.MoveTowards(orb.transform.position, targetPosition, spellData.speed * Time.deltaTime);
                }
                else{
                    // Set return bools.
                    returning = true;
                    enemiesHit.Clear();
                }
            }
            else{
                // The orb is returning, move it towards the player.
                orb.transform.position = Vector3.MoveTowards(orb.transform.position, transform.position, returnSpeed * Time.deltaTime);
                // Speed up the orb as it returns until the max speed is reached.
                returnSpeed += spellData.accel * Time.deltaTime;
                if(returnSpeed > spellData.maxSpeed)
                    returnSpeed = spellData.maxSpeed;
            }
            yield return null;
        }
        enemiesHit.Clear();
    }

    /*
    *   Hit - Deals first spells damage to the enemy hit. Magic damage on first part then true damage on return.
    *   @param unit - IUnit of the enemy hit.
    */
    public void Hit(IUnit unit){
        spellHitCallback?.Invoke(unit, this);
        if(unit is IDamageable){
            // Only want to hit an enemy once per way.
            if(!enemiesHit.Contains(unit)){
                float damageValue = spellData.baseDamage[SpellLevel] + (0.45f * championStats.magicDamage.GetValue());
                // Magic damage on first part then true damage on return.
                if(!returning){
                    ((IDamageable) unit).TakeDamage(damageValue, DamageType.Magic, player, false);
                }
                else{
                    ((IDamageable) unit).TakeDamage(damageValue, DamageType.True, player, false);
                }
                enemiesHit.Add(unit);
            }
        }
    }
}
