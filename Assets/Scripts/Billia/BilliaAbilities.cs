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
        // TODO: Animate.
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        List<Collider> outerHit = new List<Collider>(Physics.OverlapSphere(transform.position, billia.spell_1_outerRadius, enemyMask));
        foreach(Collider collider in outerHit){
            // Check if the unit was hit only by the inner portion.
            if(CheckSpell_1_OnlyInnerHit(collider)){
                Debug.Log("inner radius hit");
                //billiaAbilityHit.Spell_1_Hit(collider.gameObject, "inner");
                // TODO: Add passive dot.
            }
            // Unit hit by outer portion.
            else{
                Debug.Log("outer radius hit");
                //billiaAbilityHit.Spell_1_Hit(collider.gameObject, "outer");
                // TODO: Add passive dot.
            }
            passiveStack = true;
        }
        // If a unit was hit proc the spells passive.
        if(passiveStack)
            Spell_1_PassiveProc();
    }

    /*
    *   CheckSpell_1_OnlyInnerHit - Checks if a colliders min and max bounds are both within the inner radius of spell 1.
    *   @param collider - Collider to check the bounds of.
    */
    private bool CheckSpell_1_OnlyInnerHit(Collider collider){
        // Get the min and max bounds of the collider and set their y to the same as Billia.
        Vector3 min = collider.bounds.min;
        Vector3 max = collider.bounds.max;
        max.y = transform.position.y;
        min.y = transform.position.y;
        // Get the distances from Billia and return whether or not both are within spell 1's inner radius.
        float minMag = (transform.position - min).magnitude;
        float maxMag = (transform.position - max).magnitude;
        return minMag <= billia.spell_1_innerRadius && maxMag <= billia.spell_1_innerRadius;
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
    *   Spell_1_PassiveRunning - Controls logic for whether the passive is still running or not.
    */
    private IEnumerator Spell_1_PassiveRunning(){
        spell_1_passiveRunning = true;
        // While the time since last stack hasn't reached the time until stack dropping.
        while(Time.time - spell_1_lastStackTime < billia.spell_1_passiveSpeedDuration){
            yield return null;
        }
        spell_1_passiveRunning = false;
        // Initiate stack fall off coroutine.
        StartCoroutine(Spell_1_PassiveDropping());
    }

    /*
    *   Spell_1_PassiveDropping - Drops spell 1 passive stacks one at a time.
    */
    private IEnumerator Spell_1_PassiveDropping(){
        // While spell 1 passive has stopped running drop a stack every iteration.
        while(!spell_1_passiveRunning && spell_1_passiveStacks > 0){
            //TODO: Take away speed boost.
            spell_1_passiveStacks -= 1;
            yield return new WaitForSeconds(billia.spell_1_passiveExpireDuration);
        }
    }

    /*
    *   Spell_2 - Set up Billia's second spell. She dashed an offset distance towards her target location then deals damage in two radius'.
    *   The inner radius deals bonus damage.
    */
    public override void Spell_2(){
        // If the spell is off cd, Billia is not casting, and has enough mana.
        if(!spell_1_onCd && !isCasting && championStats.currentMana >= billia.spell2BaseMana[levelManager.spellLevels["Spell_2"]-1]){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = GetTargetDirection();
            // Set the target position to be in the direction of the mouse on cast.
            Vector3 targetPosition = (targetDirection - transform.position);
            // Set the spell cast position to max range if casted past that value.
            if(targetPosition.magnitude > billia.spell_2_maxMagnitude)
                targetPosition = transform.position + (targetPosition.normalized * billia.spell_2_maxMagnitude);
            // Set the spell cast position to the dashOffset if target positions magnitude is less than it.
            else if(targetPosition.magnitude < billia.spell_2_dashOffset)
                targetPosition = transform.position + (targetPosition.normalized * billia.spell_2_dashOffset);
            // Set target position to calculated mouse position.
            else
                targetPosition = transform.position + targetPosition;
            // TODO: Handle terrain checking.
            // TODO: Add spell hitbox on the ground.
            // Get the direction the final calculated spell cast is in.
            Vector3 directionToMove = (new Vector3(targetPosition.x, targetDirection.y, targetPosition.z) - transform.position).normalized;
            // Get the position offset to place Billia from the spell cast position.
            Vector3 billiaTargetPosition = targetPosition - (directionToMove * billia.spell_2_dashOffset);
            // Apply the dash.
            StartCoroutine(Spell_2_Dash(billiaTargetPosition));
            // Use mana.
            championStats.UseMana(billia.spell1BaseMana[levelManager.spellLevels["Spell_2"]-1]);
        }
    }

    /*
    *   Spell_2_Dash - Moves Billia to the target offset position from the spell casts position.
    *   @param targetPosition - Vector3 of the position to move Billia to.
    */
    private IEnumerator Spell_2_Dash(Vector3 targetPosition){
            // Disable pathing.
            navMeshAgent.ResetPath();
            navMeshAgent.enabled = false;
            // Get dash speed since dash duration is a fixed time.
            float dashSpeed = (targetPosition - transform.position).magnitude/billia.spell_2_dashTime; 
            float timer = 0.0f;
            // While still dashing.
            while(timer < billia.spell_2_dashTime){
                // Move towards target position.
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }
            // Apply last tick dash and enable pathing.
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
            navMeshAgent.enabled = true;
            //TODO: Call method to check targets hit.
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
