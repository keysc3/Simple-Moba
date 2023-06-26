using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class BilliaAbilities : ChampionAbilities
{
    [SerializeField] private ScriptableDot passiveDot;
    [SerializeField] private GameObject seed;
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
    *   Passive - Passive implementation for Billia. Applies a dot to enemies hit by any of Billia's abilities and heals Billia over the duration.
    *   @param enemy - GameObject of the unit to apply the passive to.
    */
    public void Passive(GameObject enemy){
        // TODO: Handle resetting dot timer on new ability hit.
        enemy.GetComponent<StatusEffectManager>().AddEffect(passiveDot.InitializeEffect(100f, gameObject, enemy));
        StartCoroutine(PassiveHeal(enemy));
    }

    /*
    *   PassiveHeal - Heals Billia while the unit has her passive applied to them.
    *   @param enemy - GameObject the dot is applied to and the passive healing is coming from.
    */
    private IEnumerator PassiveHeal(GameObject enemy){
        // Check to make sure the dot is still on the unit.
        StatusEffectManager statusEffectManager = enemy.GetComponent<StatusEffectManager>();
        UnitStats unitStats = enemy.GetComponent<UnitStats>();
        while(statusEffectManager.CheckForEffect(passiveDot, gameObject)){
            // Heal the champion amount if unit is a champion.
            if(unitStats.unit is Champion){
                Debug.Log("Billia passive found on: " + enemy.name);
                float healAmount = (6f + ((84f / 17f) * (float)(levelManager.level - 1)))/passiveDot.duration;
                championStats.SetHealth(championStats.currentHealth + healAmount);
                Debug.Log("Billia passive healed " + healAmount + " health from passive tick.");
            }
            else if(unitStats.unit is Monster){
                if(((Monster) unitStats.unit).size == "large"){
                    Debug.Log("Billia passive found on: " + enemy.name);
                    float healAmount = (39f + ((15f / 17f) * (float)(levelManager.level - 1)))/passiveDot.duration;
                    championStats.SetHealth(championStats.currentHealth + healAmount);
                    Debug.Log("Billia passive healed " + healAmount + " health from passive tick.");
                }
            }
            yield return new WaitForSeconds(passiveDot.tickRate);
        }
    }

    /*
    *   Spell_1 - Sets up Billia's first spell. She swirls her weapon in a radius around her. Players hit by the outer portion take bonus damage.
    *   Passive: Gain a stacking speed bonus whenever a unit is hit with any spell, up to 4 stacks.
    */
    public override void Spell_1(){
        // If the spell is off cd, Billia is not casting, and has enough mana.
        if(!spell_1_onCd && !isCasting && championStats.currentMana >= billia.spell1BaseMana[levelManager.spellLevels["Spell_1"]-1]){
            // Start cast time then cast the spell.
            StartCoroutine(CastTime(billia.spell_1_castTime, true));
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
        DoubleRadiusHitboxCheck(transform.position, billia.spell_1_outerRadius, billia.spell_1_innerRadius, "Spell_1", spellHit);
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
        spell1VisualHitbox.transform.SetParent(transform);
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
            // Initalize variables 
            NavMeshHit meshHit;
            int walkableMask = 1 << UnityEngine.AI.NavMesh.GetAreaFromName("Walkable");
            // Check if there is terrain between the target location and billia.
            if(NavMesh.Raycast(transform.position, targetPosition, out meshHit, walkableMask)){
                // Use the value returned in meshHit to set a new target position.
                Vector3 temp = targetPosition;
                targetPosition = meshHit.position;
                targetPosition.y = temp.y;
            }
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
        DoubleRadiusHitboxCheck(targetPosition, billia.spell_2_outerRadius, billia.spell_2_innerRadius, "Spell_2", spellHit);
        Destroy(GameObject.Find("/BilliaSpell_2"));
    }

    /*
    *   DoubleRadiusHitboxCheck - Checks an outer radius for any collider hits then checks if those hits are part of the inner radius damage.
    *   The appropriate spells damage method and radius is used based on the results.
    *   @param hitboxCenter - Vector3 of the position of the center of the radius' hitbox.
    *   @param outerRadius - float of the outer radius value to be used.
    *   @param innerRadius - float of the inner radius value to be used.
    *   @param spell - string of the spell that has been casted.
    *   @param DoubleRadiusHitboxHit - delegate containing the method to call if a spell hit is found.
    */
    private void DoubleRadiusHitboxCheck(Vector3 hitboxCenter, float outerRadius, float innerRadius, string spell, DoubleRadiusHitboxHit hitMethod){
        bool passiveStack = false;
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        List<Collider> outerHit = new List<Collider>(Physics.OverlapSphere(hitboxCenter, outerRadius, enemyMask));
        foreach(Collider collider in outerHit){
            // Check if the center of the hit collider is within the spell hitbox.
            Vector3 colliderHitCenter = collider.bounds.center;
            float distToHitboxCenter = (colliderHitCenter - hitboxCenter).magnitude;
            if(distToHitboxCenter < outerRadius){
                // If the casted spell is spell 2 then use edge range for hitbox check.
                if(spell == "Spell_2"){
                    Vector3 closestPoint = collider.ClosestPoint(hitboxCenter);
                    closestPoint.y = collider.bounds.center.y;
                    distToHitboxCenter = (closestPoint - hitboxCenter).magnitude;
                }
                // Check if the unit was hit by the specified spells inner damage.
                if(distToHitboxCenter < innerRadius){
                    hitMethod(collider.gameObject, "inner");
                    Passive(collider.gameObject);
                    //collider.gameObject.GetComponent<StatusEffectManager>().AddEffect(passiveDot.InitializeEffect(20f, gameObject, collider.gameObject));
                    // TODO: Add passive dot.
                }
                // Unit hit by outer portion.
                else{
                    hitMethod(collider.gameObject, "outer");
                    // TODO: Add passive dot.
                }
                passiveStack = true;
            }
        }
        // If a unit was hit proc the spells passive.
        if(passiveStack)
            Spell_1_PassiveProc();
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
    *   Spell_3 - Champions third ability method. Lobs a seed at a target location. If no initial terrain or unit collision the seed rolls 
    *   until one occurs. Explodes on collision and applies its damage to units within a cone forward of the collision.
    */
    public override void Spell_3(){
        if(!spell_3_onCd && !isCasting && championStats.currentMana >= billia.spell3BaseMana[levelManager.spellLevels["Spell_3"]-1]){
            // Start cast time then cast the spell.
            StartCoroutine(CastTime(billia.spell_3_castTime, false));
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = GetTargetDirection();
            // Set the target position to be in the direction of the mouse on cast.
            Vector3 targetPosition = (targetDirection - transform.position);
            // Set target to lob seed to to max lob distance if casted at a greater distance.
            if(targetPosition.magnitude > billia.spell_3_maxLobMagnitude)
                targetPosition = transform.position + (targetPosition.normalized * billia.spell_3_maxLobMagnitude);
            else
                targetPosition = transform.position + (targetDirection - transform.position);
            StartCoroutine(Spell_3_Cast(targetPosition));
            // Use mana.
            championStats.UseMana(billia.spell3BaseMana[levelManager.spellLevels["Spell_3"]-1]);
            spell_3_onCd = true;
        }
    }

    /*
    *   Spell_3_Cast - Casts Billia's third ability after the cast time.
    *   @param targetPosition - Vector3 of where the seed is to be lobbed.
    */
    private IEnumerator Spell_3_Cast(Vector3 targetPosition){
        // Wait for cast time.
        while(isCasting)
            yield return null;
        StartCoroutine(Spell_Cd_Timer(billia.spell3BaseCd[levelManager.spellLevels["Spell_3"]-1], (myBool => spell_3_onCd = myBool), "Spell_3"));
        StartCoroutine(Spell_3_Lob(targetPosition));
    }

    /*
    *   Spell_3_Lob - Lobs the seed at the target location over a set time.
    *   @param targetPosition - Vector3 of the target position for the seed to land.
    */
    private IEnumerator Spell_3_Lob(Vector3 targetPosition){
        float timer = 0.0f;
        while (timer < billia.spell_3_lobTime){
            // TODO: Add lob animation to target position.
            timer += Time.deltaTime;
            yield return null;
        }
        // Roll after landing.
        StartCoroutine(Spell_3_Move(targetPosition));
    }

    /*
    *   Spell_3_Move - Instantiates the seed and checks for collision on lob landing. If no landing collision the seed rolls in the 
    *   target forward direction until a collision.
    *   @param targetPosition - Vector3 of the lobbed seeds landing position.
    */

    private IEnumerator Spell_3_Move(Vector3 targetPosition){
        // Instantiate seed and setup rolling collision trigger variables.
        // TODO: Instantiate and initial hit check in lob method once animation is made.
        GameObject spell_3_seed = (GameObject)Instantiate(seed, targetPosition, Quaternion.identity);
        spell_3_seed.GetComponent<BilliaSpell3Trigger>().billiaAbilities = this;
        // Direction to roll.
        Vector3 targetDirection =  (targetPosition - transform.position).normalized;
        spell_3_seed.GetComponent<BilliaSpell3Trigger>().forwardDirection = targetDirection;
        // Set inital seed position.
        spell_3_seed.transform.position = new Vector3(spell_3_seed.transform.position.x, 0.9f, spell_3_seed.transform.position.z);
        // Look at roll direction.
        spell_3_seed.transform.LookAt(spell_3_seed.transform.position + targetDirection);
        LayerMask groundMask = LayerMask.GetMask("Ground", "Projectile");
        // Check for lob landing hits.
        List<Collider> lobHit = new List<Collider>(Physics.OverlapSphere(spell_3_seed.transform.position, 
        spell_3_seed.GetComponent<SphereCollider>().radius * billia.spell_3_lobLandHitbox, ~groundMask));
        // If a hit then apply damage in a cone in the roll direction.
        if(lobHit.Count > 0){
            Debug.Log("Hit on lob land: " + lobHit[0].gameObject.name);
            Spell_3_ConeHitbox(spell_3_seed, lobHit[0].gameObject, targetDirection);
            Destroy(spell_3_seed);
        }
        // While seed hasn't been destroyed, no collision.
        while(spell_3_seed){
            // Move the seed in the target direction and rotate it to animate rolling.
            float step = billia.spell_3_seedSpeed * Time.deltaTime;
            spell_3_seed.transform.position = Vector3.MoveTowards(spell_3_seed.transform.position, spell_3_seed.transform.position + targetDirection, step);
            spell_3_seed.transform.RotateAround(spell_3_seed.transform.position, spell_3_seed.transform.right, billia.spell_3_seedRotation * Time.deltaTime);
            yield return null;
        }
    }

    /*
    *   Spell_3_ConeHitBox - Checks the seeds post collision cone hitbox for any units to apply the damage to.
    *   @param spell_3_seed - GameObject of the seed.
    *   @param forwardDirection - Vector3 of the roll direction.
    */

    public void Spell_3_ConeHitbox(GameObject spell_3_seed, GameObject initialHit, Vector3 forwardDirection){
        // Check for hits in a sphere with radius of the cone to be checked.
        LayerMask groundMask = LayerMask.GetMask("Ground", "Projectile");
        Collider [] seedConeHits = Physics.OverlapSphere(spell_3_seed.transform.position, billia.spell_3_seedConeRadius, ~groundMask);
        foreach (Collider collider in seedConeHits){
            if(collider.tag == "Enemy" && collider.gameObject != initialHit){
                // Get the direction to the hit collider.
                Vector3 directionToHit = (collider.transform.position - spell_3_seed.transform.position).normalized;
                // If the angle between the roll direction and hit collider direction is within the cone then apply damage.
                if(Vector3.Angle(forwardDirection, directionToHit) < billia.spell_3_seedConeAngle/2){
                    Debug.Log("Cone hit: " + collider.transform.name);
                    // TODO: Apply damage.
                }
            }
        }
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
