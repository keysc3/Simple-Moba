using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BahriSpell1 : DamageSpell, IHasCast
{

    private BahriSpell1Data spellData;
    private List<GameObject> enemiesHit = new List<GameObject>();
    public bool returning { get; private set; } = false;

    public BahriSpell1(ChampionSpells championSpells, string spellNum, SpellData spellData) : base(championSpells, spellNum){
        this.spellData = (BahriSpell1Data) spellData;
    }

    /*
    *   Spell_1 - Sets up and creates Bahri's first spell GameObject. The spell moves from Bahri to the target position at a constant speed, then returns upon reaching
    *   the target location. The return starts slow and speeds up until reaching Bahri and being destroyed.
    */
    public void Cast(){
        if(!onCd && !player.isCasting && championStats.currentMana >= spellData.baseMana[levelManager.spellLevels[spellNum]-1]){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = GetTargetDirection();
            // Set the target position to be in the direction of the mouse on cast and at max spell distance from the player.
            Vector3 targetPosition = (targetDirection - gameObject.transform.position).normalized;
            targetPosition = gameObject.transform.position + (targetPosition * spellData.magnitude);
            // Start coroutines to handle the spells cast time and animation.
            championSpells.StartCoroutine(CastTime(spellData.castTime, canMove));
            championSpells.StartCoroutine(Spell_1_Move(targetPosition));
            // Use mana and set spell on cooldown to true.
            championStats.UseMana(spellData.baseMana[levelManager.spellLevels[spellNum]-1]);
            onCd = true;
        }
    }

    /*
    *   Spell_1_Move - Animates the players first spell.
    *   @param targetPosition - Vector3 representing the orbs return point.
    */
    private IEnumerator Spell_1_Move(Vector3 targetPosition){
        // Wait for the spells cast time.
        while(player.isCasting)
            yield return null;
        // Cooldown starts on cast.
        championSpells.StartCoroutine(Spell_Cd_Timer(spellData.baseCd[levelManager.spellLevels[spellNum]-1], spellNum));
        // Create the spells object and set necessary values.
        GameObject orb = (GameObject) GameObject.Instantiate(spellData.orb, gameObject.transform.position, Quaternion.identity);
        //SpellObjectCreated(orb);
        Spell1Trigger spell1Trigger = orb.GetComponent<Spell1Trigger>();
        spell1Trigger.SetBahriSpell1(this);
        spell1Trigger.SetBahri(gameObject); 
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
                orb.transform.position = Vector3.MoveTowards(orb.transform.position, gameObject.transform.position, returnSpeed * Time.deltaTime);
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
    *   Spell_1_Hit - Deals first spells damage to the enemy hit. Magic damage on first part then true damage on return.
    *   @param enemy - GameObject of the enemy hit.
    *   @param isReturning - bool for if the orb is returning or not.
    */
    public override void Hit(GameObject enemy){
        float magicDamage = championStats.magicDamage.GetValue();
        float damageValue = 0;
        // Only want to hit an enemy once per way.
        if(!enemiesHit.Contains(enemy)){
            damageValue = spellData.baseDamage[levelManager.spellLevels[spellNum]-1] + magicDamage;
            // Magic damage on first part then true damage on return.
            if(!returning){
                enemy.GetComponent<Unit>().TakeDamage(damageValue, "magic", gameObject, false);
            }
            else{
                enemy.GetComponent<Unit>().TakeDamage(damageValue, "true", gameObject, false);
            }
            enemiesHit.Add(enemy);
        }
    }
}
