using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

/*
* Purpose: Implements the casting and animations for the champion Bahri's abilities.
* @author: Colin Keys
*/
public class BahriAbilities : ChampionAbilities
{
    [SerializeField] private GameObject Orb;
    [SerializeField] private GameObject Spell_2_object;
    [SerializeField] private GameObject Spell_3_object;
    [SerializeField] private GameObject attack;
    [field: SerializeField] public ScriptableSpeedBonus spell_2_SpeedBonus { get; private set; }
    [field: SerializeField] public ScriptableSpell spell4 { get; private set; }

    private float spell_4_timer;
    private float spell_4_duration;
    private float spell_4_chargesLeft;
    private bool spell4Casting;
    private int passiveStacks;
    private BahriAbilityHit bahriAbilityHit;
    private Bahri bahri;
    private ScoreManager scoreManager;
    private Spell spell4Effect = null;

    // Called when the script instance is being loaded.
    protected override void Awake(){
        base.Awake();
        bahriAbilityHit = GetComponent<BahriAbilityHit>();
        scoreManager = GetComponent<ScoreManager>();
    }

    // Start is called before the first frame update
    private void Start(){
        bahri = (Bahri) championStats.unit;
        scoreManager.takedownCallback += Passive;
        scoreManager.takedownCallback += Spell_4_Takedown;
        spell4Casting = false;
        passiveStacks = 0;
    }

    // Update is called once per frame
    private void Update(){
        // Remove any destroyed objects from the active spell objects list.
        RemoveDestroyedObjects();
    }

    /*
    *   Passive - Handles the passive implementation for Bahri.
    *   @param killed - GameObject of the unit that was killed.
    */
    public void Passive(GameObject killed){
        Debug.Log("Takedown; use passive");
        float healAmount;
        // Heal off champion kill
        if(killed.GetComponent<UnitStats>().unit is Champion){
            healAmount = ((90f / 17f) * (float)(levelManager.level - 1)) + 75f;
            championStats.SetHealth(championStats.currentHealth + healAmount + championStats.magicDamage.GetValue());
            Debug.Log("Healed " + healAmount + " health from champion kill.");
        }
        // Heal off minion/monster kills if at 9 stacks.
        else{
            passiveStacks += 1;
            if(passiveStacks == 9){
                healAmount = ((60f / 17f) * (float)(levelManager.level - 1)) + 35f;
                championStats.SetHealth(championStats.currentHealth + healAmount + championStats.magicDamage.GetValue());
                passiveStacks = 0;
                Debug.Log("Healed " + healAmount + " health from minion/monster kill.");
            }
        }
    }

    /*
    *   Spell_1 - Sets up and creates Bahri's first spell GameObject. The spell moves from Bahri to the target position at a constant speed, then returns upon reaching
    *   the target location. The return starts slow and speeds up until reaching Bahri and being destroyed.
    */
    public override void Spell_1(){
        if(!spell_1_onCd && !isCasting && championStats.currentMana >= bahri.spell1BaseMana[levelManager.spellLevels["Spell_1"]-1]){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = GetTargetDirection();
            // Set the target position to be in the direction of the mouse on cast and at max spell distance from the player.
            Vector3 targetPosition = (targetDirection - transform.position).normalized;
            targetPosition = transform.position + (targetPosition * bahri.spell_1_magnitude);
            // Start coroutines to handle the spells cast time and animation.
            StartCoroutine(CastTime(bahri.spell_1_castTime, false));
            StartCoroutine(Spell_1_Move(targetPosition));
            // Use mana and set spell on cooldown to true.
            championStats.UseMana(bahri.spell1BaseMana[levelManager.spellLevels["Spell_1"]-1]);
            spell_1_onCd = true;
        }
    }

    /*
    *   Spell_1_Move - Animates the players first spell.
    *   @param targetPosition - Vector3 representing the orbs return point.
    */
    private IEnumerator Spell_1_Move(Vector3 targetPosition){
        // Wait for the spells cast time.
        while(isCasting)
            yield return null;
        // Cooldown starts on cast.
        StartCoroutine(Spell_Cd_Timer(bahri.spell1BaseCd[levelManager.spellLevels["Spell_1"]-1], (myBool => spell_1_onCd = myBool), "Spell_1"));
        // Create the spells object and set necessary values.
        GameObject orb = (GameObject)Instantiate(Orb, transform.position, Quaternion.identity);
        SpellObjectCreated(orb);
        orb.GetComponent<Spell1Trigger>().bahriAbilityHit = bahriAbilityHit;
        orb.GetComponent<Spell1Trigger>().bahri = gameObject;
        Spell1Trigger spell1Trigger = orb.GetComponent<Spell1Trigger>();
        // Set initial return values.
        bool returning = false;
        float returnSpeed = bahri.spell_1_minSpeed;
        // While the spell is active.
        while(orb){
            // If the spell hasn't started returning.
            if(!returning){
                // If target location has not been reached then move the orb towards the target location.
                if(orb.transform.position != targetPosition){
                    orb.transform.position = Vector3.MoveTowards(orb.transform.position, targetPosition, bahri.spell_1_speed * Time.deltaTime);
                }
                else{
                    // Set return bools.
                    returning = true;
                    spell1Trigger.isReturning = true;
                    bahriAbilityHit.SpellResetEnemiesHit("1");
                }
            }
            else{
                // The orb is returning, move it towards the player.
                orb.transform.position = Vector3.MoveTowards(orb.transform.position, transform.position, returnSpeed * Time.deltaTime);
                // Speed up the orb as it returns until the max speed is reached.
                returnSpeed += bahri.spell_1_accel * Time.deltaTime;
                if(returnSpeed > bahri.spell_1_maxSpeed)
                    returnSpeed = bahri.spell_1_maxSpeed;
            }
            yield return null;
        }
        // Start the spells cooldown timer.
        bahriAbilityHit.SpellResetEnemiesHit("1");
    }

    /*
    *   Spell_2 - Sets up and creates Bahri's second spell GameObjects. The spell spawns three GameObjects that rotate around Bahri and 
    *   gives a decaying speed boost. Once a spell GameObject has a target it leaves Bahri and chases its target until they die or it collides with them.
    */
    public override void Spell_2(){
        if(!spell_2_onCd && championStats.currentMana >= bahri.spell2BaseMana[levelManager.spellLevels["Spell_2"]-1]){
            // Create a parent for the spells GameObjects.
            GameObject spell_2_parent = new GameObject("Spell_2_base");
            spell_2_parent.tag = "DestroyOnDeath";
            SpellObjectCreated(spell_2_parent);
            float angle = 0.0f;
            // Create 3 GameObjects and set their position at a set magnitude from the players center and 120 degrees apart from each other.
            for(int i = 0; i < 3; i++){
                GameObject spell = (GameObject)Instantiate(Spell_2_object, spell_2_parent.transform.position, Quaternion.identity);
                spell.GetComponent<Spell2Trigger>().bahriAbilityHit = bahriAbilityHit;
                spell.transform.SetParent(spell_2_parent.transform);
                spell.transform.localPosition = new Vector3(1,0,1).normalized * bahri.spell_2_magnitude;
                spell.transform.RotateAround(spell_2_parent.transform.position, Vector3.up, angle);
                angle += 120.0f;
                SpellObjectCreated(spell);
            }
            // Start the coroutines for animating the spell and dealing with its effects.
            StartCoroutine(Spell_2_Move(spell_2_parent));
            StartCoroutine(Spell_2_Cast(spell_2_parent));
            StartCoroutine(Spell_2_Speed());
            championStats.UseMana(bahri.spell2BaseMana[levelManager.spellLevels["Spell_2"]-1]);
            spell_2_onCd = true;
        }
    }

    /*
    *   Spell_2_Move - Rotates the second spells parent GameObject to animate the spell spinning around the player.
    */
    private IEnumerator Spell_2_Move(GameObject spell_2_parent){
        // While the spell is active.
        while(spell_2_parent){
            // Keep the spells parent at the same position as the player and rotate it.
            spell_2_parent.transform.position = new Vector3(transform.position.x, transform.position.y - bahri.spell_2_heightOffset, transform.position.z);
            spell_2_parent.transform.Rotate(Vector3.up, bahri.spell_2_rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    /*
    *   Spell_2_Cast - Checks if any of the spells GameObjects have detected an enemy target in their range and starts the target found functions.
    *   @param spell_2_parent - The parent object containing the spells missiles.
    */
    private IEnumerator Spell_2_Cast(GameObject spell_2_parent){
        float timer = 0.0f;
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        // While spells still active.
        while(spell_2_parent && timer < bahri.spell_2_duration && spell_2_parent.transform.childCount > 0){
            // Wait for 0.25s wind up on cast.
            if(timer > 0.25f){
                // Check each childs proximity for an enemy.
                foreach(Transform child in spell_2_parent.transform){
                    GameObject target = null;
                    float closestDist = 0f;
                    Collider [] hitColliders = Physics.OverlapSphere(child.position, bahri.spell_2_radius, enemyMask);
                    // If an enemy is in the childs proximity find the closest one.
                    if(hitColliders.Length > 0){
                        // Check each enemy hit for closest to the GameObject.
                        foreach(Collider enemy in hitColliders){
                            // Only want to target alive units.
                            if(!enemy.gameObject.GetComponent<UnitStats>().isDead && enemy.gameObject != gameObject){
                                // If a player is currently under spell 3 effects, prioritize that player.
                                if(enemy.gameObject.GetComponent<StatusEffectManager>().CheckForEffectWithSource(bahriAbilityHit.charmEffect, gameObject)){
                                    target = enemy.gameObject;
                                    break;
                                }
                                // Set closest enemy if there isn't one yet.
                                if(target == null){
                                    target = enemy.gameObject;
                                    closestDist = (child.position - enemy.gameObject.transform.position).magnitude;
                                }
                                else{
                                    float distToEnemy = (child.position - enemy.gameObject.transform.position).magnitude;
                                    // Set closest enemy if closer one found.
                                    if (distToEnemy < closestDist){
                                        target = enemy.gameObject;
                                        closestDist = distToEnemy;
                                    }
                                }
                            }
                        }
                    }
                    // If a target has been found start the target found animation.
                    if(target != null){
                        StartCoroutine(Spell_2_Target(child.gameObject, target));
                        Spell2Trigger spell2Trigger = child.gameObject.GetComponent<Spell2Trigger>();
                        spell2Trigger.target = target;
                        spell2Trigger.spellCast = 2;
                        child.parent = null;
                    }
                }
            }
            uiManager.SetSpellActiveDuration(2, bahri.spell_2_duration, timer);
            timer += Time.deltaTime;
            yield return null;
        }
        // Once spell is complete, destroy the parent and start the cooldown timer.
        if(spell_2_parent)
            Destroy(spell_2_parent);
        bahriAbilityHit.SpellResetEnemiesHit("2");
        uiManager.SetSpellDurationOver(2);
        StartCoroutine(Spell_Cd_Timer(bahri.spell2BaseCd[levelManager.spellLevels["Spell_2"]-1], (myBool => spell_2_onCd = myBool), "Spell_2"));
    }

    /*
    *   Spell_2_Target - Move the object towards the enemy it has targeted.
    *   @param spell_2_child - GameObject to move towards the target.
    *   @param target - Target GameObject to move the spell towards.
    */
    private IEnumerator Spell_2_Target(GameObject spell_2_child, GameObject target){
        // While the GameObject still exists move it towards the target.
        while(spell_2_child){
            spell_2_child.transform.position = Vector3.MoveTowards(spell_2_child.transform.position, target.transform.position, bahri.spell_2_speed * Time.deltaTime);
            yield return null;
        }
    }

    /*
    *   Spell_2_Speed - Apply the spell casts decaying speed boost to the player.
    */
    private IEnumerator Spell_2_Speed(){
        float timer = 0.0f;
        int spellLevel = levelManager.spellLevels["Spell_2"]-1;
        // Players speed with boost applied.
        //float newSpeed = navMeshAgent.speed + (championStats.speed.GetValue() * bahri.spell_2_msBoost);
        //navMeshAgent.speed = newSpeed;
        // Create and add a new speed bonus effect.
        SpeedBonus speedBonus = (SpeedBonus) spell_2_SpeedBonus.InitializeEffect(spellLevel, gameObject, gameObject);
        speedBonus.SetBonusPercent(bahri.spell_2_msBoost);
        GetComponent<StatusEffectManager>().AddEffect(speedBonus);
        // While speed boost is still active.
        while (timer < spell_2_SpeedBonus.duration[spellLevel]){
            // Calculate the fraction of the speed boosts duration that has passed.
            float timePassed = timer/spell_2_SpeedBonus.duration[spellLevel];
            // Decay the speed bonus based on time since activated.
            float newBonus = Mathf.SmoothStep(bahri.spell_2_msBoost, 0f, timePassed);
            speedBonus.SetBonusPercent(newBonus);
            //float step = newSpeed - Mathf.SmoothStep(championStats.speed.GetValue(), newSpeed, timePassed);
            // Apply the current speed boost.
            //navMeshAgent.speed =  championStats.speed.GetValue() + step;
            timer += Time.deltaTime;
            yield return null;
        }
        // Ensure initial speed is reached after speed boost ran out.
        //navMeshAgent.speed = championStats.speed.GetValue();
    }

    /*
    *   Spell_3 - Sets up and creates the players third spell GameObject. The spell casts a GameObject in the target direction and 'Charms' the first enemy hit,
    *   disabling their actions and moving them towards Bahri at a decreased move speed.
    */
    public override void Spell_3(){
        if(!spell_3_onCd && !isCasting && championStats.currentMana >= bahri.spell3BaseMana[levelManager.spellLevels["Spell_3"]-1]){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = GetTargetDirection();
            // Set the target position to be in the direction of the mouse on cast and at max spell distance from the player.
            Vector3 targetPosition = (targetDirection - transform.position).normalized;
            targetPosition = transform.position + (targetPosition * bahri.spell_3_magnitude);
            // Start coroutines to handle the spells cast time and animation.
            StartCoroutine(CastTime(bahri.spell_3_castTime, false));
            StartCoroutine(Spell_3_Move(targetPosition));
            // Use mana and set the spell to be on cooldown.
            championStats.UseMana(bahri.spell3BaseMana[levelManager.spellLevels["Spell_3"]-1]);
            spell_3_onCd = true;
        }
    }

    /*
    *   Spell_3_Move - Animates the spell in the direction of the target position.
    *   @param targetPosition - Vector3 of the target position to cast the spell towards.
    */
    private IEnumerator Spell_3_Move(Vector3 targetPosition){
        // Wait for cast time.
        while(isCasting)
            yield return null;
        // Cooldown stats on cast.
        StartCoroutine(Spell_Cd_Timer(bahri.spell3BaseCd[levelManager.spellLevels["Spell_3"]-1], (myBool => spell_3_onCd = myBool), "Spell_3"));  
        // Create spell 3 GameObject and set its necessary variables.
        GameObject spell_3_object = (GameObject)Instantiate(Spell_3_object, transform.position, Quaternion.identity);
        SpellObjectCreated(spell_3_object);
        spell_3_object.GetComponent<Spell3Trigger>().bahriAbilityHit = bahriAbilityHit;
        spell_3_object.GetComponent<Spell3Trigger>().bahri = gameObject;
        // While the spell object still exists.
        while(spell_3_object){
            // If target location has not been reached then move the object towards the target location.
            if(spell_3_object.transform.position != targetPosition){
                spell_3_object.transform.position = Vector3.MoveTowards(spell_3_object.transform.position, targetPosition, bahri.spell_3_speed * Time.deltaTime);
            }
            else{
                Destroy(spell_3_object);
            }
            yield return null;
        }
    }

    /*
    *   Spell_4 - Sets up and creates the players fourth spell GameObjects. The spell quickly moves Bahri in the target direction and launches projectiles 
    *  spell4Effect at up to three enemies in range upon reaching the dashes end location. The spell lasts a set duration and can be re-casted 2 times with a 1s lockout on re-casting.
    */
    public override void Spell_4(){
        if(!spell_4_onCd && !isCasting && championStats.currentMana >= bahri.spell4BaseMana[levelManager.spellLevels["Spell_4"]-1]){
            StartCoroutine(Spell_4_Start());
            // Use mana and set spell on cooldown.
            championStats.UseMana(bahri.spell4BaseMana[levelManager.spellLevels["Spell_4"]-1]);
            spell_4_onCd = true;
        }
    }

    /*
    *   Spell_4_Start - Handles the fourth spells first cast and re-casting.
    */
    private IEnumerator Spell_4_Start(){
        spell4Effect = (Spell) spell4.InitializeEffect(levelManager.spellLevels["Spell_4"]-1, gameObject, gameObject);
        GetComponent<StatusEffectManager>().AddEffect(spell4Effect);
        spell_4_timer = 0.0f;
        spell_4_duration = bahri.spell_4_duration;
        float lastCastTimer = 0.0f;
        bool isCd = true;
        spell4Casting = true;
        Spell_4_Move();
        StartCoroutine(Spell_Cd_Timer(1.0f, (myBool => isCd = myBool), "Spell_4"));
        spell_4_chargesLeft = bahri.spell_4_charges - 1.0f;
        // While the spells duration has not expired.
        while(spell_4_timer < spell_4_duration){
            // If the player re-casts, isn't casting, has spell charges left, is re-casting at least 1s since last cast, and isn't dead.
            if(Input.GetKeyDown(KeyCode.R) && !isCasting && spell_4_chargesLeft > 0.0f && !isCd && !championStats.isDead){
                Spell_4_Move();
                isCd = true;
                StartCoroutine(Spell_Cd_Timer(1.0f, (myBool => isCd = myBool), "Spell_4"));
                lastCastTimer = 0.0f;
                spell_4_chargesLeft -= 1.0f;
                spell4Effect.UpdateStacks((int)(spell_4_chargesLeft));
            }
            uiManager.SetSpellActiveDuration(4, spell_4_duration, spell_4_timer);
            if(spell_4_chargesLeft == 0)
                uiManager.SetSpellCoverActive(4, true);
            spell_4_timer += Time.deltaTime;
            lastCastTimer += Time.deltaTime;
            yield return null;
        }
        // Reset charges and start spell cooldown timer.
        spell4Casting = false;
        uiManager.SetSpellDurationOver(4);
        StartCoroutine(Spell_Cd_Timer(bahri.spell4BaseCd[levelManager.spellLevels["Spell_4"]-1], (myBool => spell_4_onCd = myBool), "Spell_4"));
    }

    private void Spell_4_Takedown(GameObject killed){
        if(killed.GetComponent<UnitStats>().unit is Champion){
            if(spell_4_chargesLeft < bahri.spell_4_charges && spell4Casting){
                uiManager.SetSpellCoverActive(4, false);
                spell_4_chargesLeft += 1;
                spell_4_timer = 0.0f;
                spell_4_duration = 10.0f;
                spell4Effect.UpdateStacks((int)spell_4_chargesLeft);
                spell4Effect.ResetTimer();
                spell4Effect.SetDuration(spell_4_duration);
            }
        }
    }
    
    /*
    *   Spell_4 - Handles calculating where to move Bahri when spell 4 is casted so that the GameObject always ends up on the navmesh.
    */
    private void Spell_4_Move(){
        // Get the players mouse position on spell cast for spells target direction.
        Vector3 targetDirection = GetTargetDirection();
        // Set the target position to be in the direction of the mouse on cast and at max spell distance from the player.
        Vector3 targetPosition = (targetDirection - transform.position);
        if(targetPosition.magnitude > bahri.spell_4_maxMagnitude)
            targetPosition = transform.position + (targetPosition.normalized * bahri.spell_4_maxMagnitude);
        else
            targetPosition = transform.position + (targetDirection - transform.position);
        // Initalize variables 
        NavMeshHit meshHit;
        int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
        // Sample for a point on the walkable navmesh within 4 units of target position.
        if(NavMesh.SamplePosition(targetPosition, out meshHit, 4.0f, walkableMask)){
            Vector3 temp = meshHit.position;
            temp.y = targetPosition.y;
            // If temp does not equal targetPosition, then the targetPosition was not on a walkable area.
            if(targetPosition != temp){
                // Raycast between the target position and the players current position.
                // If the ray hits any NavMesh areas besides walkableArea then the RayCast returns true.
                // The Raycast should always return true because we know the target position is not a walkable area.
                if(NavMesh.Raycast(transform.position, targetPosition, out meshHit, walkableMask)){
                    // Use the value returned in meshHit to set a new target position on a walkable area in the direction of the original target position.
                    temp = targetPosition;
                    targetPosition = meshHit.position;
                    targetPosition.y = temp.y;
                }
            }
        }
        // Start coroutines to handle the spells cast time and animation.
        StartCoroutine(Spell_4_Speed(targetPosition));
    }

    /*
    *   Spell_4_Speed - Moves Bahri to the targetPosition of the last cast of spell 4.
    *   @param targetPosition - Vector3 of the target position to cast the spell towards.
    */
    private IEnumerator Spell_4_Speed(Vector3 targetPosition){
        // Set necessary values and disable navmesh.
        isCasting = true;
        float newSpeed = navMeshAgent.speed + bahri.spell_4_speed;
        navMeshAgent.ResetPath();
        navMeshAgent.enabled = false;
        // While not at target position or not dead.
        while (transform.position != targetPosition && !championStats.isDead){
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, newSpeed * Time.deltaTime);
            yield return null;
        }
        // Only fire end of dash projectiles if still alive.
        if(!championStats.isDead)
            Spell_4_Missiles();
        navMeshAgent.enabled = true;
        isCasting = false;
    }

    /*
    *   Spell_4_Missles - Handles creating up to three missiles from the end of Bahris spell 4 dash if any targets are found.
    */
    private void Spell_4_Missiles(){
        // Set up necessary variables.
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        List<GameObject> targets = new List<GameObject>();
        Collider [] hitColliders = Physics.OverlapSphere(transform.position, bahri.spell_4_radius, enemyMask);
        // If a target is in range.
        if(hitColliders.Length > 0){
            foreach(Collider collider in hitColliders){
                // If the target is alive.
                if(!collider.gameObject.GetComponent<UnitStats>().isDead && collider.gameObject != gameObject){
                    // If three targets have already been found.
                    if(targets.Count > 2){
                        // Set the farthest enemy as first in the targets found list.
                        int furthestEnemyIndex = 0;
                        float furthestEnemyDist = (transform.position - targets[0].transform.position).magnitude;
                        // Check the other two targets against the first and set the farthest target in the targets found list.
                        for(int i = 1; i < 3; i++){
                            float distToEnemy = (transform.position - targets[i].transform.position).magnitude;
                            if(distToEnemy > furthestEnemyDist){
                                furthestEnemyIndex = i;
                                furthestEnemyDist = distToEnemy;
                            }
                        }
                        // If the farthest target in the targets found list is farther than the new target then replace it.
                        float newEnemyDist = (transform.position - collider.gameObject.transform.position).magnitude;
                        if(furthestEnemyDist > newEnemyDist){
                            targets.RemoveAt(furthestEnemyIndex);
                            targets.Add(collider.gameObject);
                        }
                    }
                    else{
                        targets.Add(collider.gameObject);
                    }
                }
            }
        }
        // If a target has been found start the target found animation.
        if(targets.Count > 0){
            foreach(GameObject target in targets){
                // Create missile and set necessary variables
                GameObject missile = (GameObject)Instantiate(Spell_2_object, transform.position, Quaternion.identity);
                SpellObjectCreated(missile);
                missile.GetComponent<Spell2Trigger>().bahriAbilityHit = bahriAbilityHit;
                Spell2Trigger spell2Trigger = missile.GetComponent<Spell2Trigger>();
                spell2Trigger.target = target;
                spell2Trigger.spellCast = 4;
                // Use the same animation as spell two to send the missiles to their target.
                StartCoroutine(Spell_2_Target(missile, target));
            }
        }
    }

    /*
    *   OnDeathCleanUp - Handles any spell cleanup that needs to be done for Bahri when they die.
    */
    public override void OnDeathCleanUp(){
        // Destroy the spell 2 parent to stop spell 2 children without targets from existing.
        if(activeSpellObjects.Count > 0){
            for(int i = 0; i < activeSpellObjects.Count; i++){
                if(activeSpellObjects[i].name == "Spell_2_base")
                    Destroy(activeSpellObjects[i]);
            }
        }
    }

    /*
    *   OnRespawnCleanUp - Handles any spell cleanup that needs to be done for Bahri when they respawn.
    */
    public override void OnRespawnCleanUp(){
        // Destroy any GameObjects created from Bahri's spell casts.
        if(activeSpellObjects.Count > 0){
            for(int i = activeSpellObjects.Count - 1; i >=0; i--){
                if(activeSpellObjects[i] == null)
                    activeSpellObjects.RemoveAt(i);
            }
        }
    }
}
