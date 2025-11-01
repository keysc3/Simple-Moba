using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements Bahri'a fourth spell. Bahri dashes in a target direction and fires a damaging projectile at a number of units upon landing. 
* The spell can be recasted a number of times over a duration. Takedowns during the spells duration extend the duration and give another charge.
*
* @author: Colin Keys
*/
public class BahriSpell4 : Spell, IHasCast, IHasHit
{
    public SpellHitCallback spellHitCallback { get; set; }

    new private BahriSpell4Data spellData;
    private NavMeshAgent navMeshAgent;
    private PersonalSpell spell4Effect = null;
    private float spell_4_timer;
    private float spell_4_duration;
    private int spell_4_chargesLeft;
    public int Spell_4_ChargesLeft {
        get => spell_4_chargesLeft;
        set {
            spell_4_chargesLeft = value;
            if(spell4Effect != null)
                spell4Effect.Stacks = value;
        }
    }
    private bool spell4Casting;
    public bool Spell4Casting {
        get => spell4Casting;
        set {
            spell4Casting = value;
            RaiseSetComponentActiveEvent(SpellNum, SpellComponent.DurationSlider, value);
        }
    }
    private bool canRecast = false;

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BahriSpell4Data) base.spellData;
        player.score.takedownCallback += Spell_4_Takedown;
        IsQuickCast = true;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void OnDisable(){
        player.score.takedownCallback -= Spell_4_Takedown;
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        DrawSpellUIHitbox(0, 0f, Vector2.one * spellData.maxMagnitude * 2f, false);
    }

    /*
    *   Cast - Casts the spell.
    */
    public bool Cast(){
        if(!player.IsCasting){
            if(!Spell4Casting){
                if(!OnCd && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
                    StartCoroutine(Spell_4_LifeCycle());
                    OnCd = true;
                    // Use mana and set spell on cooldown.
                    championStats.UseMana(spellData.baseMana[SpellLevel]);
                    return true;
                }
            }
            else{
                return Recast();
            }
        }
        return false;
    }

    /*
    *   NextCastCd - Handles the spells recast cooldown.
    *   @param spell_cd - float of the cooldown between casts.
    *   @param spell - string of the spell number.
    */
    private IEnumerator NextCastCd(float spell_cd){
        RaiseSpellCDSetActiveEvent(SpellNum, true);
        float spell_timer = 0.0f;
        // While time since last cast is less than or equal to the cd between casts.
        while(spell_timer <= spell_cd){
            spell_timer += Time.deltaTime;
            // Update the UI cooldown text and slider.
            spellController.RaiseSpellCDUpdateEvent(SpellNum, spell_cd - spell_timer, spell_cd);
            if(Spell_4_ChargesLeft == 0)
                RaiseSetComponentActiveEvent(SpellNum, SpellComponent.CDCover, true);
            yield return null;
        }
        canRecast = true;
    }
    
    /*
        Recast - Handles the actions of the spells recast.
    */
    private bool Recast(){
        // If the player re-casts, isn't casting, has spell charges left, is re-casting at least 1s since last cast, and isn't dead.
        if(Spell_4_ChargesLeft > 0 && canRecast){
            Spell_4_Move();
            Spell_4_ChargesLeft--;
            return true;
        }
        return false;
    }
    /*
    *   Spell_4_LifeCycle - Handles the fourth spells initialization and life cycle.
    */
    private IEnumerator Spell_4_LifeCycle(){
        spell4Effect = (PersonalSpell) spellData.spell4.InitializeEffect(SpellLevel, player, player);
        player.statusEffects.AddEffect(spell4Effect);
        spell_4_timer = 0.0f;
        spell_4_duration = spellData.duration;
        Spell4Casting = true;
        Spell_4_Move();
        Spell_4_ChargesLeft = spellData.charges - 1;
        // While the spells duration has not expired.
        while(spell_4_timer < spell_4_duration){
            RaiseSpellSliderUpdateEvent(SpellNum, spell_4_duration, spell_4_timer);
            spell_4_timer += Time.deltaTime;
            yield return null;
        }
        // Reset charges and start spell cooldown timer.
        Spell4Casting = false;
        StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
    }

    /*
    *   Spell_4_Takedown - Grants a charge and increases the spells duration if the takedown was on a champion.
    *   @param killed - GameObject the takedown was on.
    */
    private void Spell_4_Takedown(IUnit killed){
        if(killed is IPlayer){
            if(Spell_4_ChargesLeft < spellData.charges && Spell4Casting){
                Spell_4_ChargesLeft += 1;
                spell_4_timer = 0.0f;
                spell_4_duration = 10.0f;
                spell4Effect.ResetTimer();
                spell4Effect.EffectDuration = spell_4_duration;
                if(canRecast)
                    RaiseSetComponentActiveEvent(SpellNum, SpellComponent.CDCover, false);
            }
        }
    }
    
    /*
    *   Spell_4 - Handles calculating where to move Bahri when spell 4 is casted so that the GameObject always ends up on the navmesh.
    */
    private void Spell_4_Move(){
        // Get the players mouse position on spell cast for spells target direction.
        Vector3 targetDirection = spellController.GetTargetDirection();
        player.MouseOnCast = targetDirection;
        // Set the target position to be in the direction of the mouse on cast and at max spell distance from the player.
        Vector3 targetPosition = (targetDirection - transform.position);
        if(targetPosition.magnitude > spellData.maxMagnitude)
            targetPosition = transform.position + (targetPosition.normalized * spellData.maxMagnitude);
        else
            targetPosition += transform.position;
        targetPosition = spellController.GetPositionOnWalkableNavMesh(targetPosition, true);
        // Start coroutines to handle the spells cast time and animation.
        StartCoroutine(Spell_4_Speed(targetPosition));
        canRecast = false;
    }

    /*
    *   Spell_4_Speed - Moves Bahri to the targetPosition of the last cast of spell 4.
    *   @param targetPosition - Vector3 of the target position to cast the spell towards.
    */
    private IEnumerator Spell_4_Speed(Vector3 targetPosition){
        // Set necessary values and disable navmesh.
        player.IsCasting = true;
        player.CurrentCastedSpell = this;
        float newSpeed = championStats.speed.GetValue() + spellData.speed;
        navMeshAgent.ResetPath();
        navMeshAgent.enabled = false;
        // While not at target position or not dead.
        while(transform.position != targetPosition && !player.IsDead){
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, newSpeed * Time.deltaTime);
            yield return null;
        }
        // Only fire end of dash projectiles if still alive.
        if(!player.IsDead)
            Spell_4_Missiles();
        navMeshAgent.enabled = true;
        player.IsCasting = false;
        player.CurrentCastedSpell = this;
        StartCoroutine(NextCastCd(1.0f));
    }

    /*
    *   Spell_4_Missles - Handles creating up to three missiles from the end of Bahris spell 4 dash if any targets are found.
    */
    private void Spell_4_Missiles(){
        // Set up necessary variables.
        //LayerMask enemyMask = LayerMask.GetMask("Enemy");
        List<Transform> targets = new List<Transform>();
        Collider [] hitColliders = Physics.OverlapSphere(player.hitbox.transform.position, spellData.radius);
        // If a target is in range.
        if(hitColliders.Length > 0){
            foreach(Collider collider in hitColliders){
                IUnit enemyUnit = collider.gameObject.GetComponentInParent<IUnit>();
                if(enemyUnit == null  || collider.transform.name != "Hitbox")
                    continue;
                // If the target is alive.
                if(!enemyUnit.IsDead && collider.transform.parent != transform && collider.gameObject.GetComponentInParent<IDamageable>() != null){
                    Transform enemyHitbox = enemyUnit.hitbox.transform.parent;
                    // If three targets have already been found.
                    if(targets.Count > 2){
                        // Set the farthest enemy as first in the targets found list.
                        int furthestEnemyIndex = 0;
                        float furthestEnemyDist = (transform.position - targets[0].position).magnitude;
                        // Check the other two targets against the first and set the farthest target in the targets found list.
                        for(int i = 1; i < 3; i++){
                            float distToEnemy = (transform.position - targets[i].position).magnitude;
                            if(distToEnemy > furthestEnemyDist){
                                furthestEnemyIndex = i;
                                furthestEnemyDist = distToEnemy;
                            }
                        }
                        // If the farthest target in the targets found list is farther than the new target then replace it.
                        float newEnemyDist = (transform.position - enemyHitbox.position).magnitude;
                        if(furthestEnemyDist > newEnemyDist){
                            targets.RemoveAt(furthestEnemyIndex);
                            targets.Add(enemyHitbox);
                        }
                    }
                    else{
                        targets.Add(enemyHitbox);
                    }
                }
            }
        }
        // If a target has been found start the target found animation.
        if(targets.Count > 0){
            foreach(Transform target in targets){
                // Create missile and set necessary variables
                GameObject missile = (GameObject) Instantiate(spellData.missile, transform.position, Quaternion.identity);
                missile.transform.localScale = Vector3.one * spellData.projectileScale;
                TargetedProjectile targetedProjectile = missile.GetComponentInChildren<TargetedProjectile>();
                targetedProjectile.hit = Hit;
                targetedProjectile.TargetUnit = target.GetComponentInParent<IUnit>();
                // Use the same animation as spell two to send the missiles to their target.
                StartCoroutine(Spell_4_Target(missile, target));
            }
        }
    }

    /*
    *   Spell_4_Target - Move the object towards the enemy it has targeted.
    *   @param missile - Transform to move towards the target.
    *   @param target - Target GameObject to move the spell towards.
    */
    private IEnumerator Spell_4_Target(GameObject missile, Transform target){
        // While the GameObject still exists move it towards the target.
        while(missile && target){
            missile.transform.position = Vector3.MoveTowards(missile.transform.position, target.position, spellData.missileSpeed * Time.deltaTime);
            yield return null;
        }
    }

    /*
    *   Hit - Deals fourth spells damage to the enemy hit.
    *   @param unit - IUnit of the enemy hit.
    */
    public void Hit(IUnit unit, params object[] args){
        spellHitCallback?.Invoke(unit, this);
        if(unit is IDamageable){
            ((IDamageable) unit).TakeDamage(spellData.baseDamage[SpellLevel] + (0.35f * championStats.magicDamage.GetValue()), DamageType.Magic, player, false);
        }
    }
}
