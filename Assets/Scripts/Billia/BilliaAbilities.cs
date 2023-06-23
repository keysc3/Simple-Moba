using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BilliaAbilities : ChampionAbilities
{
    [SerializeField] private int spell_1_passiveStacks;
    private float spell_1_lastStackTime;
    private bool spell_1_passiveRunning;

    private Billia billia;

    public GameObject spell1Visual;
    public float spell1Visual_initialAlpha = 60.0f;
    public float spell1Visual_finalAlpha = 160.0f;
    public GameObject spell2Visual;

    public delegate void DoubleRadiusHitboxHit(GameObject hit, string radius); 
    public DoubleRadiusHitboxHit spellHit;
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
            StartCoroutine(Spell_1_Cast(Spell_1_Visual()));
            // Use mana.
            championStats.UseMana(billia.spell1BaseMana[levelManager.spellLevels["Spell_1"]-1]);
            spell_1_onCd = true;
        }        
    }

    /*
    *   Spell_1_Cast - Casts Billia's first spell.
    */
    private IEnumerator Spell_1_Cast(GameObject spell1VisualHitbox){
        // Animate the beginning of the spell.
        StartCoroutine(Spell_1_Animation(spell1VisualHitbox, spell1Visual_initialAlpha, spell1Visual_finalAlpha));
        while(isCasting){
            yield return null;
        }
        StartCoroutine(Spell_Cd_Timer(billia.spell1BaseCd[levelManager.spellLevels["Spell_1"]-1], (myBool => spell_1_onCd = myBool), "Spell_1"));
        // Set method to call if a hit.
        spellHit = Spell_1_Hit_Placeholder;
        // Hitbox starts from center of Billia.
        DoubleRadiusHitboxCheck(transform.position, billia.spell_1_outerRadius, "Spell_1", spellHit);
        // Animate the ending of the spell.
        StartCoroutine(Spell_1_Animation(spell1VisualHitbox, spell1Visual_finalAlpha, spell1Visual_initialAlpha));
    }

    /*
    *   Spell_1_Animation - Animates the wind up or wind down of Billia's first spell.
    *   @param spell1VisualHitBox - GameObject of the hitbox visual.
    *   @param initialAlpha - float of the starting alpha.
    *   @param finalAlpha - float of the final alpha to reach.
    */
    private IEnumerator Spell_1_Animation(GameObject spell1VisualHitbox, float initialAlpha, float finalAlpha){
        // Get the outer radius's renderer and color.
        Renderer outerRenderer = spell1VisualHitbox.transform.GetChild(1).gameObject.GetComponent<Renderer>();
        Color newColor = outerRenderer.material.color;
        // Set up.
        float startTime = Time.time;
        float timer = 0.0f;
        // Animation time is spell cast time.
        while(timer < billia.spell_1_castTime){
            // Animate the spell cast.
            float step = (Time.time - startTime)/billia.spell_1_castTime;
            newColor.a = Mathf.Lerp(initialAlpha, finalAlpha, step)/255f;
            outerRenderer.material.color = newColor;
            timer += Time.deltaTime;
            yield return null;
        }
        // Last tick.
        newColor.a = finalAlpha/255f;
        // Destroy the hitbox visual is this was the ending animation.
        if(finalAlpha < initialAlpha)
            Destroy(spell1VisualHitbox);
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
    *   Spell_1_Visual - Visual hitbox indicator for Billia's first spell.
    *   @return GameObject - Created visual hitbox GameObject.
    */
    private GameObject Spell_1_Visual(){
        // Create the spells visual hitbox and set necessary values.
        GameObject spell1VisualHitbox = (GameObject)Instantiate(spell1Visual, transform.position, Quaternion.identity);
        spell1VisualHitbox.name = "BilliaSpell_1";
        float yScale = spell1VisualHitbox.transform.GetChild(0).localScale.y;
        spell1VisualHitbox.transform.GetChild(0).localScale = new Vector3(billia.spell_1_innerRadius * 2f, yScale, billia.spell_1_innerRadius * 2f);
        spell1VisualHitbox.transform.GetChild(1).localScale = new Vector3(billia.spell_1_outerRadius * 2f, yScale, billia.spell_1_outerRadius * 2f);
        return spell1VisualHitbox;
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
            // Show the spells hitbox.
            Spell_2_Visual(targetPosition);
            // Apply the dash.
            StartCoroutine(Spell_2_Dash(billiaTargetPosition, targetPosition));
            // Use mana.
            championStats.UseMana(billia.spell1BaseMana[levelManager.spellLevels["Spell_2"]-1]);
            spell_2_onCd = true;
        }
    }

    /*
    *   Spell_2_Visual - Visual hitbox indicator for Billia's second spell.
    */
    private void Spell_2_Visual(Vector3 targetPosition){
        // Create the spells visual hitbox and set necessary values.
        GameObject spell2VisualHitbox = (GameObject)Instantiate(spell2Visual, targetPosition, Quaternion.identity);
        spell2VisualHitbox.name = "BilliaSpell_2";
        spell2VisualHitbox.transform.position = new Vector3(spell2VisualHitbox.transform.position.x, 0.5f, spell2VisualHitbox.transform.position.z);
        float yScale = spell2VisualHitbox.transform.GetChild(0).localScale.y;
        spell2VisualHitbox.transform.GetChild(0).localScale = new Vector3(billia.spell_2_innerRadius * 2f, yScale, billia.spell_2_innerRadius * 2f);
        spell2VisualHitbox.transform.GetChild(1).localScale = new Vector3(billia.spell_2_outerRadius * 2f, yScale, billia.spell_2_outerRadius * 2f);
    }

    /*
    *   Spell_2_Dash - Moves Billia to the target offset position from the spell casts position.
    *   @param targetPosition - Vector3 of the position to move Billia to.
    */
    private IEnumerator Spell_2_Dash(Vector3 targetPosition, Vector3 spellTargetPosition){
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
            Spell_2_Cast(spellTargetPosition);
    }

    /*
    *   Spell_2_Cast - Casts Billia's second spell.
    */
    private void Spell_2_Cast(Vector3 targetPosition){
        StartCoroutine(Spell_Cd_Timer(billia.spell2BaseCd[levelManager.spellLevels["Spell_2"]-1], (myBool => spell_2_onCd = myBool), "Spell_2"));
        // Set method to use if a hit.
        spellHit = Spell_2_Hit_Placeholder;
        // Hitbox starts from center of calculated target position.
        DoubleRadiusHitboxCheck(targetPosition, billia.spell_2_outerRadius, "Spell_2", spellHit);
        Destroy(GameObject.Find("/BilliaSpell_2"));
    }

    /*
    *   DoubleRadiusHitboxCheck - Checks an outer radius for any collider hits then checks if those hits are part of the inner radius damage.
    *   The appropriate spells damage method and radius is used based on the results.
    *   @param hitboxCenter - Vector3 of the position of the center of the radius' hitbox.
    *   @param outerRadius - float of the outer radius value to be used.
    *   @param spell - string of the spell that has been casted.
    *   @param DoubleRadiusHitboxHit - delegate containing the method to call if a spell hit is found.
    */
    private void DoubleRadiusHitboxCheck(Vector3 hitboxCenter, float outerRadius, string spell, DoubleRadiusHitboxHit hitMethod){
        bool passiveStack = false;
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        List<Collider> outerHit = new List<Collider>(Physics.OverlapSphere(hitboxCenter, outerRadius, enemyMask));
        foreach(Collider collider in outerHit){
            // Check if the unit was hit by the specified spells inner damage.
            if(CheckTwoRadiusInnerHit(collider, hitboxCenter, spell)){
                hitMethod(collider.gameObject, "inner");
                // TODO: Add passive dot.
            }
            // Unit hit by outer portion.
            else{
                hitMethod(collider.gameObject, "outer");
                // TODO: Add passive dot.
            }
            passiveStack = true;
        }
        // If a unit was hit proc the spells passive.
        if(passiveStack)
            Spell_1_PassiveProc();
    }

    /*
    *   CheckTwoRadiusInnerHit - Checks if a collider that was within an outer radius is fully in or partially in an inner radius.
    *   @param collider - Collider to check the bounds of.
    *   @param hitboxCenter - Vector3 of the center of the inner radius.
    *   @param spell - String of which spells hitbox is being checked.
    */
    private bool CheckTwoRadiusInnerHit(Collider collider, Vector3 hitboxCenter, string spell){
        // Get the min and max bounds of the collider and set their y to the same as Billia.
        Vector3 min = collider.bounds.min;
        Vector3 max = collider.bounds.max;
        max.y = transform.position.y;
        min.y = transform.position.y;
        // Get the distances from inner radius center.
        float minMag = (hitboxCenter - min).magnitude;
        float maxMag = (hitboxCenter - max).magnitude;
        // Spell 1 inner hit needs the entire collider inside the inner radius.
        if(spell == "Spell_1")
            return minMag <= billia.spell_1_innerRadius && maxMag <= billia.spell_1_innerRadius;
        // Spell 2 inner hit needs any part of the collider in the inner radius.
        else
            return minMag <= billia.spell_2_innerRadius || maxMag <= billia.spell_2_innerRadius;
    }

    // TODO: Move to ability hit script.
    private void Spell_2_Hit_Placeholder(GameObject hit, string radius){
        Debug.Log("Spell 2 Hit on " + hit.name + " w/ " + radius + " radius.");
    }
    // TODO: Move to ability hit script.
    private void Spell_1_Hit_Placeholder(GameObject hit, string radius){
        Debug.Log("Spell 1 Hit on " + hit.name + " w/ " + radius + " radius.");
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
