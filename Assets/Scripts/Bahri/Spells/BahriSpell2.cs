using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BahriSpell2 : DamageSpell, IDeathCleanUp, ICastable
{
    private BahriSpell2Data spellData;
    private List<GameObject> enemiesHit = new List<GameObject>();
    public List<GameObject> activeSpellObjects { get; private set; } = new List<GameObject>();
    public bool isDisplayed { get; private set; } = false;

    public BahriSpell2(ChampionSpells championSpells, string spellNum, SpellData spellData) : base(championSpells, spellNum){
        this.spellData = (BahriSpell2Data) spellData;
        isQuickCast = true;
    }

    protected override void DrawSpell(){
        Handles.color = Color.cyan;
        Vector3 drawPosition = gameObject.transform.position;
        drawPosition.y -= (myCollider.bounds.size.y/2) + 0.01f;
        Handles.DrawWireDisc(drawPosition, Vector3.up, spellData.radius + spellData.magnitude, 1f);
    }

    /*
    *   Spell_2 - Sets up and creates Bahri's second spell GameObjects. The spell spawns three GameObjects that rotate around Bahri and 
    *   gives a decaying speed boost. Once a spell GameObject has a target it leaves Bahri and chases its target until they die or it collides with them.
    */
    public void Cast(){
        if(championStats.currentMana >= spellData.baseMana[levelManager.spellLevels[spellNum]-1]){
            // Create a parent for the spells GameObjects.
            GameObject spell_2_parent = new GameObject("Spell_2_Parent");
            activeSpellObjects.Add(spell_2_parent);
            float angle = 0.0f;
            // Create 3 GameObjects and set their position at a set magnitude from the players center and 120 degrees apart from each other.
            for(int i = 0; i < 3; i++){
                GameObject missile = (GameObject) GameObject.Instantiate(spellData.missile, spell_2_parent.transform.position, Quaternion.identity);
                TargetedProjectile targetedProjectile = missile.GetComponent<TargetedProjectile>();
                targetedProjectile.hit = Hit;
                missile.transform.SetParent(spell_2_parent.transform);
                missile.transform.localPosition = new Vector3(1,0,1).normalized * spellData.magnitude;
                missile.transform.RotateAround(spell_2_parent.transform.position, Vector3.up, angle);
                angle += 120.0f;
                //SpellObjectCreated(missile);
            }
            // Start the coroutines for animating the spell and dealing with its effects.
            championSpells.StartCoroutine(Spell_2_Move(spell_2_parent));
            championSpells.StartCoroutine(Spell_2_Cast(spell_2_parent));
            championSpells.StartCoroutine(Spell_2_Speed());
            championStats.UseMana(spellData.baseMana[levelManager.spellLevels[spellNum]-1]);
            onCd = true;
        }
    }

    /*
    *   Spell_2_Move - Rotates the second spells parent GameObject to animate the spell spinning around the player.
    */
    private IEnumerator Spell_2_Move(GameObject spell_2_parent){
        // While the spell is active.
        while(spell_2_parent){
            // Keep the spells parent at the same position as the player and rotate it.
            spell_2_parent.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - spellData.heightOffset, gameObject.transform.position.z);
            spell_2_parent.transform.Rotate(Vector3.up, spellData.rotationSpeed * Time.deltaTime);
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
        while(spell_2_parent && timer < spellData.duration && spell_2_parent.transform.childCount > 0){
            // Wait for 0.25s wind up on cast.
            if(timer > 0.25f){
                // Check each childs proximity for an enemy.
                foreach(Transform child in spell_2_parent.transform){
                    GameObject target = null;
                    float closestDist = 0f;
                    Collider [] hitColliders = Physics.OverlapSphere(child.position, spellData.radius, enemyMask);
                    // If an enemy is in the childs proximity find the closest one.
                    if(hitColliders.Length > 0){
                        // Check each enemy hit for closest to the GameObject.
                        foreach(Collider enemy in hitColliders){
                            Unit enemyUnit = enemy.gameObject.GetComponent<Unit>();
                            // Only want to target alive units.
                            if(!enemyUnit.isDead && enemy.gameObject != gameObject){
                                // If a player is currently under spell 3 effects, prioritize that player.
                                if(enemyUnit.statusEffects.CheckForEffectWithSource(spellData.charm, gameObject)){
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
                        championSpells.StartCoroutine(Spell_2_Target(child.gameObject, target));
                        TargetedProjectile targetedProjectile = child.gameObject.GetComponent<TargetedProjectile>();
                        targetedProjectile.SetTarget(target);
                        child.parent = null;
                    }
                }
            }
            UIManager.instance.SetSpellActiveDuration(2, spellData.duration, timer, player.playerUI);
            timer += Time.deltaTime;
            yield return null;
        }
        // Once spell is complete, destroy the parent and start the cooldown timer.
        if(spell_2_parent)
            GameObject.Destroy(spell_2_parent);
        enemiesHit.Clear();
        UIManager.instance.SetSpellDurationOver(2, player.playerUI);
        championSpells.StartCoroutine(Spell_Cd_Timer(spellData.baseCd[levelManager.spellLevels[spellNum]-1], spellNum));
    }

    /*
    *   Spell_2_Target - Move the object towards the enemy it has targeted.
    *   @param spell_2_child - GameObject to move towards the target.
    *   @param target - Target GameObject to move the spell towards.
    */
    private IEnumerator Spell_2_Target(GameObject spell_2_child, GameObject target){
        // While the GameObject still exists move it towards the target.
        while(spell_2_child && target){
            spell_2_child.transform.position = Vector3.MoveTowards(spell_2_child.transform.position, target.transform.position, spellData.speed * Time.deltaTime);
            yield return null;
        }
    }

    /*
    *   Spell_2_Speed - Apply the spell casts decaying speed boost to the player.
    */
    private IEnumerator Spell_2_Speed(){
        float timer = 0.0f;
        int spellLevel = levelManager.spellLevels[spellNum]-1;
        // Players speed with boost applied.
        //float newSpeed = navMeshAgent.speed + (championStats.speed.GetValue() * bahri.spell_2_msBoost);
        //navMeshAgent.speed = newSpeed;
        // Create and add a new speed bonus effect.
        SpeedBonus speedBonus = (SpeedBonus) spellData.speedBonus.InitializeEffect(spellLevel, gameObject, gameObject);
        player.statusEffects.AddEffect(speedBonus);
        // While speed boost is still active.
        while (timer < spellData.speedBonus.duration[spellLevel]){
            // Calculate the fraction of the speed boosts duration that has passed.
            float timePassed = timer/spellData.speedBonus.duration[spellLevel];
            // Decay the speed bonus based on time since activated.
            float newBonus = Mathf.SmoothStep(spellData.speedBonus.bonusPercent[spellLevel], 0f, timePassed);
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
    *   Spell_2_Hit - Deals second spells damage to the enemy hit. Reduced damage on missiles that hit the same target more than once.
    *   @param enemy - GameObject of the enemy hit.
    */
    public override void Hit(GameObject enemy){
        float magicDamage = championStats.magicDamage.GetValue();
        float finalDamage = spellData.baseDamage[levelManager.spellLevels[spellNum]-1] + magicDamage;
        // Reduce damage of spell if hitting the same target more than once.
        if(enemiesHit.Contains(enemy)){
            finalDamage = Mathf.Round(finalDamage * spellData.multiplier);
        }
        enemy.GetComponent<Unit>().TakeDamage(finalDamage, "magic", gameObject, false);
        enemiesHit.Add(enemy);
    }

    public void OnDeathCleanUp(){
        // Destroy all spell parent objects.
        if(activeSpellObjects.Count > 0){
            for(int i = 0; i < activeSpellObjects.Count; i++){
                Object.Destroy(activeSpellObjects[i]);
            }
        }
        activeSpellObjects.Clear();
    }
}
