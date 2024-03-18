using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements Bahri'a second spell. Bahri summons a number of projectiles that orbit her and gains a decaying speed boost.
* If an enemy unit enters a projectiles target radius it locks onto the unit and deals damage upon reaching it.
* The projectiles prioritize the a charmed target and then the closest target when multiple units are in its target radius.
*
* @author: Colin Keys
*/
public class BahriSpell2 : Spell, IDeathCleanUp, IHasCast, IHasHit
{
    public List<GameObject> activeSpellObjects { get; } = new List<GameObject>();
    public SpellHitCallback spellHitCallback { get; set; }
    
    new private BahriSpell2Data spellData;
    private List<IUnit> enemiesHit = new List<IUnit>();

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BahriSpell2Data) base.spellData;
        IsQuickCast = true;
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        DrawSpellUIHitbox(0, 0f, Vector2.one * (spellData.radius + spellData.magnitude) * 2f, false);
    }

    /*
    *   Cast - Casts the spell.
    */
    public bool Cast(){
        if(!OnCd && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            // Create a parent for the spells GameObjects.
            GameObject spell_2_parent = new GameObject("Spell_2_Parent");
            activeSpellObjects.Add(spell_2_parent);
            float angle = 0.0f;
            // Create 3 GameObjects and set their position at a set magnitude from the players center and 120 degrees apart from each other.
            for(int i = 0; i < 3; i++){
                GameObject missile = (GameObject) Instantiate(spellData.missile, spell_2_parent.transform.position, Quaternion.identity);
                missile.transform.localScale = Vector3.one * spellData.projectileScale;
                TargetedProjectile targetedProjectile = missile.GetComponentInChildren<TargetedProjectile>();
                targetedProjectile.hit = Hit;
                missile.transform.SetParent(spell_2_parent.transform);
                missile.transform.localPosition = new Vector3(1,0,1).normalized * spellData.magnitude;
                missile.transform.RotateAround(spell_2_parent.transform.position, Vector3.up, angle);
                angle += 120.0f;
            }
            // Start the coroutines for animating the spell and dealing with its effects.
            StartCoroutine(Spell_2_Move(spell_2_parent));
            StartCoroutine(Spell_2_Cast(spell_2_parent));
            StartCoroutine(Spell_2_Speed());
            championStats.UseMana(spellData.baseMana[SpellLevel]);
            OnCd = true;
            return true;
        }
        return false;
    }

    /*
    *   Spell_2_Move - Rotates the second spells parent GameObject to animate the spell spinning around the player.
    */
    private IEnumerator Spell_2_Move(GameObject spell_2_parent){
        // While the spell is active.
        while(spell_2_parent){
            // Keep the spells parent at the same position as the player and rotate it.
            spell_2_parent.transform.position = 
            new Vector3(transform.position.x, transform.position.y - spellData.heightOffset, transform.position.z);
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
        //LayerMask enemyMask = LayerMask.GetMask("Enemy");
        RaiseSetComponentActiveEvent(SpellNum, SpellComponent.DurationSlider, true);
        // While spells still active.
        while(spell_2_parent && timer < spellData.duration && spell_2_parent.transform.childCount > 0){
            // Wait for 0.25s wind up on cast.
            if(timer > 0.25f){
                // Check each childs proximity for an enemy.
                foreach(Transform child in spell_2_parent.transform){
                    Transform target = null;
                    float closestDist = 0f;
                    Transform hitbox = child.GetChild(0);
                    Collider [] hitColliders = Physics.OverlapSphere(hitbox.position, spellData.radius, hitboxMask);
                    // If an enemy is in the childs proximity find the closest one.
                    if(hitColliders.Length > 0){
                        // Check each enemy hit for closest to the GameObject.
                        foreach(Collider enemy in hitColliders){
                            Debug.Log("Collider: " + enemy.transform.name);
                            IUnit enemyUnit = enemy.gameObject.GetComponentInParent<IUnit>();
                            if(enemyUnit == null)
                                continue;
                            Debug.Log(enemy.transform.name + " processed.");
                            Transform enemyTransform = enemy.transform.parent;
                            // Only want to target alive units.
                            if(!enemyUnit.IsDead && enemyUnit != player){
                                // If a player is currently under spell 3 effects, prioritize that player.
                                if(enemyUnit.statusEffects.CheckForEffectWithSource(spellData.charm, player)){
                                    target = enemyTransform;
                                    break;
                                }
                                // Set closest enemy if there isn't one yet.
                                if(target == null){
                                    target = enemyTransform;
                                    closestDist = (hitbox.position - enemyTransform.position).magnitude;
                                }
                                else{
                                    float distToEnemy = (hitbox.position - enemyTransform.position).magnitude;
                                    // Set closest enemy if closer one found.
                                    if (distToEnemy < closestDist){
                                        target = enemyTransform;
                                        closestDist = distToEnemy;
                                    }
                                }
                            }
                        }
                    }
                    // If a target has been found start the target found animation.
                    if(target != null){
                        StartCoroutine(Spell_2_Target(child.gameObject, target));
                        TargetedProjectile targetedProjectile = child.gameObject.GetComponentInChildren<TargetedProjectile>();
                        targetedProjectile.TargetUnit = target.GetComponentInParent<IUnit>();
                        child.parent = null;
                    }
                }
            }
            timer += Time.deltaTime;
            RaiseSpellSliderUpdateEvent(SpellNum, spellData.duration, timer);
            yield return null;
        }
        // Once spell is complete, destroy the parent and start the cooldown timer.
        if(spell_2_parent)
            Destroy(spell_2_parent);
        enemiesHit.Clear();
        RaiseSetComponentActiveEvent(SpellNum, SpellComponent.DurationSlider, false);
        StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
    }

    /*
    *   Spell_2_Target - Move the object towards the enemy it has targeted.
    *   @param spell_2_child - Transform to move towards the target.
    *   @param target - Target GameObject to move the spell towards.
    */
    private IEnumerator Spell_2_Target(GameObject spell_2_child, Transform target){
        // While the GameObject still exists move it towards the target.
        while(spell_2_child && target){
            spell_2_child.transform.position = Vector3.MoveTowards(spell_2_child.transform.position, target.position, spellData.speed * Time.deltaTime);
            yield return null;
        }
    }

    /*
    *   Spell_2_Speed - Apply the spell casts decaying speed boost to the player.
    */
    private IEnumerator Spell_2_Speed(){
        float timer = 0.0f;
        int spellLevel = SpellLevel;
        // Create and add a new speed bonus effect.
        SpeedBonus speedBonus = (SpeedBonus) spellData.speedBonus.InitializeEffect(spellLevel, spellData.bonusSpeedPercent, player, player);
        player.statusEffects.AddEffect(speedBonus);
        // While speed boost is still active.
        while (timer < spellData.speedBonus.duration[spellLevel]){
            // Calculate the fraction of the speed boosts duration that has passed.
            float timePassed = timer/spellData.speedBonus.duration[spellLevel];
            // Decay the speed bonus based on time since activated.
            float newBonus = Mathf.SmoothStep(spellData.bonusSpeedPercent, 0f, timePassed);
            speedBonus.BonusPercent = newBonus;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    /*
    *   Hit - Deals second spells damage to the enemy hit. Reduced damage on missiles that hit the same target more than once.
    *   @param unit - IUnit of the unit hit.
    */
    public void Hit(IUnit unit, params object[] args){
        spellHitCallback?.Invoke(unit, this);
        if(unit is IDamageable){
            float finalDamage = spellData.baseDamage[SpellLevel] + (0.3f * championStats.magicDamage.GetValue());
            // Reduce damage of spell if hitting the same target more than once.
            if(enemiesHit.Contains(unit)){
                finalDamage = finalDamage * spellData.multiplier;
            }
            ((IDamageable) unit).TakeDamage(finalDamage, DamageType.Magic, player, false);
            enemiesHit.Add(unit);
        }
    }

    /*
    *   OnDeathCleanUp - Handles any clean up this spell needs on Bahri's death.
    */
    public void OnDeathCleanUp(){
        // Destroy all spell parent objects.
        if(activeSpellObjects.Count > 0){
            for(int i = 0; i < activeSpellObjects.Count; i++){
                Destroy(activeSpellObjects[i]);
            }
        }
        activeSpellObjects.Clear();
    }
}
