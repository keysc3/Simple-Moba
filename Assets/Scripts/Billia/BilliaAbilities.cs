using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaAbilities : ChampionAbilities
{
    private int spell_1_passiveStacks;
    private float spell_1_lastStackTime;
    private bool spell_1_passiveRunning;

    private Billia billia;

    // Start is called before the first frame update
    void Start()
    {
       billia = (Billia) championStats.unit;
       spell_1_passiveStacks = 0;
    }

    void Update(){
        Debug.Log("Passive stacks: " + spell_1_passiveStacks);
    }

    /*
    *   Spell_1 - Sets up Billia's first spell. She swirls her weapon in a radius around her. Players hit by the outer portion take bonus damage.
    *   Passive: Gain a stacking speed bonus whenever a unit is hit with any spell, up to 4 stacks.
    */
    public override void Spell_1(){
        // If the spell is off cd, Billia is not casting, and has enough mana.
        if(!spell_1_onCd && !isCasting && championStats.currentMana >= billia.spell1BaseMana[levelManager.spellLevels["Spell_1"]-1]){
            // Start cast time then cast the spell.
            StartCoroutine(CastTime(billia.spell_1_castTime));
            StartCoroutine(Spell_1_Cast());
            // Use mana.
            championStats.UseMana(billia.spell1BaseMana[levelManager.spellLevels["Spell_1"]-1]);
        }        
    }

    /*
    *   Spell_1_Cast - Casts Billia's first spell.
    */
    private IEnumerator Spell_1_Cast(){
        bool passiveStack = false;
        while(isCasting)
            yield return null;
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        //TODO: Fix outerHit to not include inner radius.
        List<Collider> outerHit = new List<Collider>(Physics.OverlapSphere(transform.position, billia.spell_1_outerRadius, enemyMask));
        List<Collider> innerHit = new List<Collider>(Physics.OverlapSphere(transform.position, billia.spell_1_innerRadius, enemyMask));
        foreach(Collider collider in outerHit){
            //billiaAbilityHit.Spell_1_Hit(collider.gameObject, "outer");
            //TODO: Add passive dot.
            passiveStack = true;
        }
        foreach(Collider collider in innerHit){
            if(!outerHit.Contains(collider)){
                //billiaAbilityHit.Spell_1_Hit(collider.gameObject, "inner");
                //TODO: Add passive dot.
                passiveStack = true;
            }
        }
        // If a unit was hit proc the spells passive.
        if(passiveStack)
            Spell_1_PassiveProc();
    }

    /*
    *   Spell_1_PassiveProc - Handles spell 1's passive being activated or refreshed.
    */
    private void Spell_1_PassiveProc(){
        spell_1_lastStackTime = Time.time;
        if(spell_1_passiveStacks < billia.spell_1_passiveMaxStacks){
            spell_1_passiveStacks += 1;
            //TODO: Grant speed boost.
            // If the passive has started dropping stacks or has none, start running it again.
            if(!spell_1_passiveRunning){
                StartCoroutine(Spell_1_PassiveRunning());
            }
        }
    }

    /*
    *   Spell_1_PassiveRunning - Controls logic for whether the passive is still in the running of not.
    */
    private IEnumerator Spell_1_PassiveRunning(){
        spell_1_passiveRunning = true;
        // While the time since last stack hasn't reached the time until stack dropping.
        while(Time.time - spell_1_lastStackTime < billia.spell_1_passiveSpeedDuration){
            yield return null;
        }
        spell_1_passiveRunning = false;
        StartCoroutine(Spell_1_PassiveDropping());
    }

    /*
    *   Spell_1_PassiveDropping - Drops spell 1 passive stacks one at a time.
    */
    private IEnumerator Spell_1_PassiveDropping(){
        // While spell 1 passive has stopped running drop a stack every iteration.
        while(!spell_1_passiveRunning && spell_1_passiveStacks > 0){
            Debug.Log("Passive stacks dropping");
            //TODO: Take away speed boost.
            spell_1_passiveStacks -= 1;
            yield return new WaitForSeconds(billia.spell_1_passiveExpireDuration);
        }
    }

    /*
    *   Spell_2 - Champions second ability method.
    */
    public override void Spell_2(){

    }

    /*
    *   Spell_3 - Champions third ability method.
    */
    public override void Spell_3(){

    }

    /*
    *   Spell_4 - Champions fourth ability method.
    */
    public override void Spell_4(){

    }

    /*
    *   Attack - Champions auto attack method.
    *   @param targetedEnemy - GameObject of the enemy to attack.
    */
    public override void Attack(GameObject targetedEnemy){

    }

    /*
    *   OnDeathCleanUp - Method for any necessary spell cleanup on death.
    */
    public override void OnDeathCleanUp(){

    }

    /*
    *   OnRespawnCleanUp - Method for any necessary spell cleanup on respawn.
    */
    public override void OnRespawnCleanUp(){

    }

}
