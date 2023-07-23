using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements the casting and animations for the champion Billia's abilities.
* @author: Colin Keys
*/
public class BilliaAbilities : ChampionAbilities
{
    [field: SerializeField] public ScriptableSleep sleep { get; private set; }
    [field: SerializeField] public ScriptableSpeedBonus spell_1_passiveSpeedBonus { get; private set; }
    [field: SerializeField] public GameObject spell1Visual { get; private set; }
    [field: SerializeField] public GameObject spell2Visual { get; private set; }
    [field: SerializeField] public GameObject drowsyVisual { get; private set; }

    [SerializeField] private int spell_1_passiveStacks;
    [SerializeField] private float p1_y_offset;
    [SerializeField] private float p2_y;
    [SerializeField] private List<GameObject> passiveApplied = new List<GameObject>();
    [SerializeField] private ScriptableDot passiveDot;
    [SerializeField] private GameObject seed;
    [SerializeField] private ScriptableDrowsy drowsy;
    private float spell1Visual_initialAlpha = 60.0f;
    private float spell1Visual_finalAlpha = 160.0f;
    //private float spell_1_lastStackTime;
    private bool spell_1_passiveRunning;
    private bool canUseSpell_4 = false;
    //private List<float> spell_1_passiveTracker = new List<float>();
    private List<Effect> spell_1_passiveEffectTracker = new List<Effect>();
    private Billia billia;
    private BilliaAbilityHit billiaAbilityHit;

    public delegate void DoubleRadiusHitboxHit(GameObject hit, string radius); 
    public DoubleRadiusHitboxHit spellHit;

    // Called when the script instance is being loaded. 
    protected override void Awake(){
        base.Awake();
        billiaAbilityHit = GetComponent<BilliaAbilityHit>();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        billia = (Billia) player.unit;
        spell_1_passiveStacks = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if(levelManager.spellLevels["Spell_4"] > 0){
            canUseSpell_4 = CanUseSpell_4();
        }
    }

    /*
    *   Passive - Passive implementation for Billia. Applies a dot to enemies hit by any of Billia's abilities and heals Billia over the duration.
    *   @param enemy - GameObject of the unit to apply the passive to.
    */
    public void Passive(GameObject enemy){
        enemy.GetComponent<Unit>().statusEffects.AddEffect(passiveDot.InitializeEffect(30f, 0, gameObject, enemy));
        if(!passiveApplied.Contains(enemy)){
            passiveApplied.Add(enemy);
            StartCoroutine(PassiveHeal(enemy));
        }
    }

    /*
    *   PassiveHeal - Heals Billia while the unit has her passive applied to them.
    *   @param enemy - GameObject the dot is applied to and the passive healing is coming from.
    */
    private IEnumerator PassiveHeal(GameObject enemy){
        // Check to make sure the dot is still on the unit.
        Unit unit = enemy.GetComponent<Unit>();
        while(unit.statusEffects.CheckForEffectWithSource(passiveDot, gameObject)){
            // Heal the champion amount if unit is a champion.
            if(unit.unit is ScriptableChampion){
                Debug.Log("Billia passive found on: " + enemy.name);
                float healAmount = (6f + ((84f / 17f) * (float)(levelManager.level - 1)))/passiveDot.duration[0];
                championStats.SetHealth(championStats.currentHealth + healAmount);
                Debug.Log("Billia passive healed " + healAmount + " health from passive tick.");
            }
            else if(unit.unit is ScriptableMonster){
                if(((ScriptableMonster) unit.unit).size == "large"){
                    Debug.Log("Billia passive found on: " + enemy.name);
                    float healAmount = (39f + ((15f / 17f) * (float)(levelManager.level - 1)))/passiveDot.duration[0];
                    championStats.SetHealth(championStats.currentHealth + healAmount);
                    Debug.Log("Billia passive healed " + healAmount + " health from passive tick.");
                }
            }
            yield return new WaitForSeconds(passiveDot.tickRate);
        }
        passiveApplied.Remove(enemy);
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
        spellHit = billiaAbilityHit.Spell_1_Hit;
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
        //spell_1_lastStackTime = Time.time;
        if(levelManager.spellLevels["Spell_1"] > 0 && spell_1_passiveStacks < billia.spell_1_passiveMaxStacks){
            // Create a new speed bonus with the 
            float bonusPercent = billia.spell_1_passiveSpeed[levelManager.spellLevels["Spell_1"]-1];
            SpeedBonus speedBonus = (SpeedBonus) spell_1_passiveSpeedBonus.InitializeEffect(levelManager.spellLevels["Spell_1"]-1, gameObject, gameObject);
            speedBonus.SetBonusPercent(bonusPercent);
            player.statusEffects.AddEffect(speedBonus);
            spell_1_passiveEffectTracker.Add(speedBonus);
            spell_1_passiveStacks += 1;
            //float amountIncrease = championStats.speed.GetValue() * bonusPercent;
            //navMeshAgent.speed = navMeshAgent.speed + amountIncrease;
            //spell_1_passiveTracker.Add(amountIncrease);
            // If the passive has started dropping stacks or has none, start running it again.
            /*if(!spell_1_passiveRunning){
                StartCoroutine(Spell_1_PassiveRunning());
            }*/
        }
        if(spell_1_passiveStacks > 1){
            ResetSpell_1_PassiveTimers();
        }
    }

    /*
    *   ResetSpell_1_PassiveTimers - Changes the duration of Billia's passive stacks. 
    *   This is for when a passive proc happens and the timers need to be updated.
    */
    private void ResetSpell_1_PassiveTimers(){
        int multiplier = spell_1_passiveStacks - 1;
        // If at max stacks then reset the newest stack as well.
        if(spell_1_passiveStacks == billia.spell_1_passiveMaxStacks){
            int last = spell_1_passiveEffectTracker.Count - 1;
            ChangeSpell_1_PassiveEffect(last, 0f);
        }
        // Reset each passive stacks timer to the base plus an increase based on where it is in the stack list.
        for(int i = 0; i < spell_1_passiveEffectTracker.Count - 1; i++){
            ChangeSpell_1_PassiveEffect(i, billia.spell_1_passiveExpireDuration * multiplier);
            multiplier -= 1;
        }
    }

    /*
    *   ChangeSpell_1_PassiveEffect - Resets the timer and changes the duration of a spell 1 passive effect.
    *   @param index - int of the stacks index being changed.
    *   @param baseIncrease - float of the increase from the base duration for the stack.
    */
    private void ChangeSpell_1_PassiveEffect(int index, float baseIncrease){
        spell_1_passiveEffectTracker[index].ResetTimer();
        float newDuration = billia.spell_1_passiveSpeedDuration + baseIncrease;
        spell_1_passiveEffectTracker[index].SetDuration(newDuration);
    }

    /*
    *   RemoveSpell_1_PassiveStack - Removes any stacks from the passive list if they have finished.
    */
    private void RemoveSpell_1_PassiveStack(){
        for(int i = spell_1_passiveEffectTracker.Count - 1; i >=0; i--){
            if(spell_1_passiveEffectTracker[i].isFinished){
                spell_1_passiveEffectTracker.RemoveAt(i);
                spell_1_passiveStacks -= 1;
            }
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
        if(!spell_2_onCd && !isCasting && championStats.currentMana >= billia.spell2BaseMana[levelManager.spellLevels["Spell_2"]-1]){
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

            Vector3 initialTarget = targetPosition;
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
            // Get the direction to move Billia in using initial target.
            Vector3 directionToMove = (new Vector3(initialTarget.x, targetDirection.y,initialTarget.z) - transform.position).normalized;
            // Get the position offset to place Billia from the spell cast position.
            Vector3 billiaTargetPosition = targetPosition - (directionToMove * billia.spell_2_dashOffset);
            StartCoroutine(CastTime(billia.spell_2_castTime, false));
            // Show the spells hitbox.
            Spell_2_Visual(targetPosition);
            StartCoroutine(Spell_2_Cast(billiaTargetPosition, targetPosition));
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
    *   Spell_2_Cast - Handles cast time and dash initialization of Spell 2.
    *   @param billiaTargetPosition - Vector3 of the position to move Billia to.
    *   @param targetPosition - Vecto3 of the center of the spell.
    */
    private IEnumerator Spell_2_Cast(Vector3 billiaTargetPosition, Vector3 targetPosition){
        while(isCasting)
            yield return null;
        // Apply the dash.
        StartCoroutine(Spell_2_Dash(billiaTargetPosition, targetPosition));
    }

    /*
    *   Spell_2_Dash - Moves Billia to the target offset position from the spell casts position.
    *   @param targetPosition - Vector3 of the position to move Billia to.
    *   @param spellTargetPosition - Vector3 of the center of the spell.
    */
    private IEnumerator Spell_2_Dash(Vector3 targetPosition, Vector3 spellTargetPosition){
        isCasting = true;
        // Disable pathing.
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;
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
        navMeshAgent.isStopped = false;
        isCasting = false;
        Spell_2_Finished(spellTargetPosition);
    }

    /*
    *   Spell_2_Finished - Handles spell cd and hitbox checking when the spell 2 is finished animating.
    *   @param targetPosition - Vector3 of the center of the spell.
    */
    private void Spell_2_Finished(Vector3 targetPosition){
        StartCoroutine(Spell_Cd_Timer(billia.spell2BaseCd[levelManager.spellLevels["Spell_2"]-1], (myBool => spell_2_onCd = myBool), "Spell_2"));
        // Set method to use if a hit.
        spellHit = billiaAbilityHit.Spell_2_Hit;
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
                }
                // Unit hit by outer portion.
                else{
                    hitMethod(collider.gameObject, "outer");
                }
                passiveStack = true;
            }
        }
        // If a unit was hit proc the spells passive.
        if(passiveStack)
            Spell_1_PassiveProc();
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
            StartCoroutine(Spell_3_Cast(targetPosition, targetPosition - transform.position));
            // Use mana.
            championStats.UseMana(billia.spell3BaseMana[levelManager.spellLevels["Spell_3"]-1]);
            spell_3_onCd = true;
        }
    }

    /*
    *   Spell_3_Cast - Casts Billia's third ability after the cast time.
    *   @param targetPosition - Vector3 of where the seed is to be lobbed.
    */
    private IEnumerator Spell_3_Cast(Vector3 targetPosition, Vector3 targetDirection){
        // Wait for cast time.
        while(isCasting)
            yield return null;
        StartCoroutine(Spell_Cd_Timer(billia.spell3BaseCd[levelManager.spellLevels["Spell_3"]-1], (myBool => spell_3_onCd = myBool), "Spell_3"));
        StartCoroutine(Spell_3_Lob(targetPosition, targetDirection));
    }
    
    /*
    *   Spell_3_Lob - Lobs the seed at the target location over a set time using a Quadratic Bezier Curve.
    *   @param targetPosition - Vector3 of the target position for the seed to land.
    */
    private IEnumerator Spell_3_Lob(Vector3 targetPosition, Vector3 targetDirection){
        // Create spell object.
        GameObject spell_3_seed = (GameObject)Instantiate(seed, transform.position, Quaternion.identity);
        BilliaSpell3Trigger billiaSpell3Trigger = spell_3_seed.GetComponent<BilliaSpell3Trigger>();
        //billiaSpell3Trigger.SetBbilliaSpell3Script(this);
        billiaSpell3Trigger.SetCaster(gameObject);
        // Set p0.
        Vector3 p0 = transform.position;
        // Set p1. X and Z of p1 are halfway between Billia and target position. Y of p1 is an offset value.
        Vector3 p1 = transform.position;
        p1.y = p1.y + p1_y_offset;
        Vector3 dir = targetDirection.normalized;
        float mag = targetDirection.magnitude;
        //float mag = targetDirection.normalized;
        p1.x = p1.x + (dir.x * (mag/2f));
        p1.z = p1.z + (dir.z * (mag/2f));
        // Set p2. p2 y is a value directly above the ground.
        Vector3 p2 = targetPosition;
        p2.y = p2_y;
        // While lob time has not finished.
        float timer = 0.0f;
        while(timer < billia.spell_3_lobTime){
            // Get t value, a value between 0 and 1.
            float t = Mathf.Clamp01(timer/billia.spell_3_lobTime);
            // Get the next position on the Quadratic Bezier curve.
            Vector3 point = QuadraticBezierCurvePoint(t, p0, p1, p2);
            // Set the seeds new position.
            spell_3_seed.transform.position = point;
            timer += Time.deltaTime;
            yield return null;
        }
        // Set the seeds final point.
        Vector3 lastPoint = QuadraticBezierCurvePoint(1, p0, p1, p2);
        spell_3_seed.transform.position = lastPoint;
        //billiaSpell3Trigger.SetHasLanded(true);
        // Start the seeds rolling.
        StartCoroutine(Spell_3_Move(targetDirection.normalized, spell_3_seed, billiaSpell3Trigger));
    }

    /*
    *   Spell_3_Move - Instantiates the seed and checks for collision on lob landing. If no landing collision the seed rolls in the 
    *   target forward direction until a collision.
    *   @param targetDirection - Vector3 of the lobbed seeds direction to roll.
    */
    private IEnumerator Spell_3_Move(Vector3 targetDirection, GameObject spell_3_seed, BilliaSpell3Trigger billiaSpell3Trigger){
        billiaSpell3Trigger.SetForwardDirection(targetDirection);
        // Set inital seed position.
        //spell_3_seed.transform.position = new Vector3(spell_3_seed.transform.position.x, 0.9f, spell_3_seed.transform.position.z);
        // Look at roll direction.
        spell_3_seed.transform.LookAt(spell_3_seed.transform.position + targetDirection);
        LayerMask groundMask = LayerMask.GetMask("Ground", "Projectile");
        // Check for lob landing hits.
        List<Collider> lobHit = new List<Collider>(Physics.OverlapSphere(spell_3_seed.transform.position, 
        spell_3_seed.GetComponent<SphereCollider>().radius * billia.spell_3_lobLandHitbox, ~groundMask));
        // If a hit then apply damage in a cone in the roll direction.
        if(lobHit.Count > 0){
            if(lobHit[0].gameObject != gameObject){
                Debug.Log("Hit on lob land: " + lobHit[0].gameObject.name);
                Spell_3_ConeHitbox(spell_3_seed, lobHit[0].gameObject, targetDirection);
                Destroy(spell_3_seed);
            }
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
        bool passiveStack = false;
        if(initialHit.tag == "Enemy"){
            billiaAbilityHit.Spell_3_Hit(initialHit);
            passiveStack = true;
        }
        // Check for hits in a sphere with radius of the cone to be checked.
        LayerMask groundMask = LayerMask.GetMask("Ground", "Projectile");
        Collider [] seedConeHits = Physics.OverlapSphere(spell_3_seed.transform.position, billia.spell_3_seedConeRadius, ~groundMask);
        foreach (Collider collider in seedConeHits){
            if(collider.tag == "Enemy" && collider.gameObject != initialHit){
                // Get the direction to the hit collider.
                Vector3 colliderPos = collider.transform.position;
                Vector3 directionToHit = (new Vector3(colliderPos.x, spell_3_seed.transform.position.y, colliderPos.z) - spell_3_seed.transform.position).normalized;
                // If the angle between the roll direction and hit collider direction is within the cone then apply damage.
                if(Vector3.Angle(forwardDirection, directionToHit) < billia.spell_3_seedConeAngle/2){
                    billiaAbilityHit.Spell_3_Hit(collider.gameObject);
                    passiveStack = true;
                }
            }
        }
        if(passiveStack)
            Spell_1_PassiveProc();
    }

    /*
    *   QuadraticBezierCurvePoint - Calculates a point on a quadratic Bezier curve based on the t value.
    *   It is a linear interpolation of two points obtained from linear Bezier curves from p0 to p1 and p1 to p2.
    *   @param t - float of a time value between 0 and 1 for the progress on the curve.
    *   @param p0 - Vector3 of the first control point (starting point).
    *   @param p1 - Vector3 of the second control point (connecting point).
    *   @param p2 - Vector 3 of the third control point (end point).
    */
    private Vector3 QuadraticBezierCurvePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2){
        // p = ((1-t)^2 * P0) + (2(1-t)t * P1) + (t^2 * P2)
        float coefficient = 1 - t;
        float alpha = Mathf.Pow(coefficient, 2f);
        float beta = 2 * coefficient * t;
        float phi = Mathf.Pow(t, 2f);

        float x = (alpha * p0.x) + (beta * p1.x) + (phi * p2.x);
        float y = (alpha * p0.y) + (beta * p1.y) + (phi * p2.y);
        float z = (alpha * p0.z) + (beta * p1.z) + (phi * p2.z);
        return new Vector3(x, y, z);
    }


    /*
    *   Spell_4 - Champions fourth ability method.
    */
    public override void Spell_4(){
        // Only allow cast if a champion has passive on them.
        if(canUseSpell_4){
            if(!spell_4_onCd && !isCasting && championStats.currentMana >= billia.spell1BaseMana[levelManager.spellLevels["Spell_4"]-1]){
                // Start cast time then cast the spell.
                StartCoroutine(CastTime(billia.spell_4_castTime, true));
                StartCoroutine(Spell_4_Cast(GetChampionsWithPassive()));
                // Use mana.
                championStats.UseMana(billia.spell4BaseMana[levelManager.spellLevels["Spell_4"]-1]);
                spell_4_onCd = true;
            }
        }
    }

    /*
    *   Spell_4_Cast - Casts and starts the cd timer for spell 4. Would have to be refactored to implement projectile destruction/blocking.
    *   @param List<GameObject> - List of GameObjects to apply the drowsy to.
    */
    private IEnumerator Spell_4_Cast(List<GameObject> applyDrowsy){
        while(isCasting)
            yield return null;
        StartCoroutine(Spell_Cd_Timer(billia.spell4BaseCd[levelManager.spellLevels["Spell_4"]-1], (myBool => spell_4_onCd = myBool), "Spell_4"));
        StartCoroutine(Spell_4_Projectile(applyDrowsy));
    }

    /*
    *   Spell_4_Projectile - Handle the travel time of spell 4.
    *   @param List<GameObject> - List of GameObjects to apply the drowsy to.
    */
    private IEnumerator Spell_4_Projectile(List<GameObject> applyDrowsy){
        float travelTime = billia.spell_4_travelTime;
        float startTime = Time.time;
        while(Time.time - startTime < travelTime){
            // Move projectile.
            yield return null;
        }
        // Apply drowsy debuff.
        Spell_4_Drowsy(applyDrowsy);
    }

    /*
    *   Spell_4_Drowsy - Applies the drowsy debuff from spell 4 to any champions applied with the passive dot.
    *   @param List<GameObject> - List of GameObjects to apply the drowsy to.
    */
    private void Spell_4_Drowsy(List<GameObject> applyDrowsy){
        foreach(GameObject enemy in applyDrowsy){
            Unit enemyUnit = enemy.GetComponent<Unit>();
            if(enemyUnit is Player){
                // Add drowsy to enemy player and update the bonus damage delegate.
                Drowsy newDrowsy = (Drowsy) drowsy.InitializeEffect(levelManager.spellLevels["Spell_4"]-1, gameObject, enemy);
                enemyUnit.statusEffects.AddEffect(newDrowsy);
                enemyUnit.bonusDamage += billiaAbilityHit.Spell_4_SleepProc;
                // Animate the drowsy effect.
                GameObject drowsyObject = (GameObject)Instantiate(drowsyVisual, enemy.transform.position, Quaternion.identity);
                drowsyObject.transform.SetParent(enemy.transform);
                BilliaDrowsyVisual visualScript = drowsyObject.GetComponent<BilliaDrowsyVisual>();
                visualScript.SetDrowsyDuration(newDrowsy.effectDuration);
                visualScript.SetDrowsy(drowsy);
                visualScript.SetSource(gameObject);
            }
        }
    }

    /*
    *   CanUseSpell_4 - Checks if any champion has Billia's passive on them, which allows the use of spell 4.
    */
    private bool CanUseSpell_4(){
        List<GameObject> passiveAppliedChamps = GetChampionsWithPassive();
        if(passiveAppliedChamps.Count > 0){
            return true;
        }
        return false;
    }

    /*
    *   GetChampionsWithPassive - Get all champions with a Billia passive on them.
    *   @return List<GameObject> - List of champion GameObjects with Billia passive dot on them.
    */
    private List<GameObject> GetChampionsWithPassive(){
        List<GameObject> passiveAppliedChamps = new List<GameObject>();
        // Get all StatusEffectManagers.
        Player[] playerScripts = FindObjectsOfType<Player>();
        for (int i = 0; i < playerScripts.Length; i++){
            // If it is a champion and has the Billia dot then add it to the list.
            if(playerScripts[i].statusEffects.CheckForEffectByName(passiveDot, passiveDot.name)){
                passiveAppliedChamps.Add(playerScripts[i].gameObject);
            }
        }
        return passiveAppliedChamps;
    }

    // LateUpdate is called after all Update functions have been called
    private void LateUpdate(){
        if(canUseSpell_4 && !spell_4_onCd)
            UIManager.instance.SetSpellCoverActive(4, false, player.playerUI);
        else
            UIManager.instance.SetSpellCoverActive(4, true, player.playerUI);
        RemoveSpell_1_PassiveStack();
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
