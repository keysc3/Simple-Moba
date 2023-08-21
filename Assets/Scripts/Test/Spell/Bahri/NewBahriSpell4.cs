using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using UnityEngine.UI;

/*
* Purpose: Implements Bahri'a fourth spell. Bahri dashes in a target direction and fires a damaging projectile at a number of units upon landing. 
* The spell can be recasted a number of times over a duration. Takedowns during the spells duration extend the duration and give another charge.
*
* @author: Colin Keys
*/
public class NeewBahriSpell4 : InterSpell, IHasCast, IHasHit
{
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
            if(spellCDCover!= null){
                if(value > 0)
                    spellCDCover.SetActive(false);
                else
                    spellCDCover.SetActive(true);
            }
        }
    }
    private bool spell4Casting;
    public bool Spell4Casting {
        get => spell4Casting;
        set {
            spell4Casting = value;
            if(spellDurationSlider != null)
                spellDurationSlider.SetActive(value);
        }
    }
    private bool canRecast = false;
    public SpellHitCallback spellHitCallback { get; set; }
    private GameObject spellDurationSlider;
    private Image imageSlider;
    private GameObject spellCDCover;

    /*
    *   BahriSpell4 - Initialize Bahri's fourth spell.
    *   @param championSpells - ChampionSpells instance this spell is a part of.
    *   @param spellNum - string of the spell number this spell is.
    *   @param spellData - SpellData to use.
    */
    /*public BahriSpell4(ChampionSpells championSpells, SpellData spellData) : base(championSpells, spellData){
        this.spellData = (BahriSpell4Data) spellData;
        player.score.takedownCallback += Spell_4_Takedown;
        isQuickCast = true;
    }*/

    void Start(){
        if(SpellNum == null)
            SpellNum = spellData.defaultSpellNum;
        player.score.takedownCallback += Spell_4_Takedown;
        IsQuickCast = true;
        if(player.playerUI != null){
            spellDurationSlider = player.playerUI.transform.Find("Player/Combat/SpellsContainer/" + SpellNum + "_Container/SpellContainer/Outline/Slider").gameObject;
            imageSlider = spellDurationSlider.transform.Find("Fill").GetComponent<Image>();
            spellCDCover = spellCDTransform.Find("Cover").gameObject;
        }
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void OnDisable(){
        player.score.takedownCallback -= Spell_4_Takedown;
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Handles.color = Color.cyan;
        Vector3 drawPosition = transform.position;
        drawPosition.y -= (player.myCollider.bounds.size.y/2) + 0.01f;
        Handles.DrawWireDisc(drawPosition, Vector3.up, spellData.maxMagnitude, 1f);
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            StartCoroutine(Spell_4_Start());
            // Use mana and set spell on cooldown.
            championStats.UseMana(spellData.baseMana[SpellLevel]);
            OnCd = true;
        }
    }

    /*
    *   NextCastCd - Handles the spells recast cooldown.
    *   @param spell_cd - float of the cooldown between casts.
    *   @param spell - string of the spell number.
    */
    private IEnumerator NextCastCd(float spell_cd){
        float spell_timer = 0.0f;
        // While time since last cast is less than or equal to the cd between casts.
        while(spell_timer <= spell_cd){
            spell_timer += Time.deltaTime;
            if(spellCDTransform != null){
                // Update the UI cooldown text and slider.
                float cooldownLeft = spell_cd - spell_timer;
                spellCDText.SetText(Mathf.Ceil(cooldownLeft).ToString());
                float fill = Mathf.Clamp(cooldownLeft/spell_cd, 0f, 1f);
                spellCDImage.fillAmount = fill;
            }
            yield return null;
        }
        // Allow the spell to be cast again.
        canRecast = true;
    }

    /*
    *   Spell_4_Start - Handles the fourth spells first cast and re-casting.
    */
    private IEnumerator Spell_4_Start(){
        spell4Effect = (PersonalSpell) spellData.spell4.InitializeEffect(SpellLevel, gameObject, gameObject);
        player.statusEffects.AddEffect(spell4Effect);
        spell_4_timer = 0.0f;
        spell_4_duration = spellData.duration;
        Spell4Casting = true;
        Spell_4_Move();
        Spell_4_ChargesLeft = spellData.charges - 1;
        // While the spells duration has not expired.
        while(spell_4_timer < spell_4_duration){
            // If the player re-casts, isn't casting, has spell charges left, is re-casting at least 1s since last cast, and isn't dead.
            if(Input.GetKeyDown(KeyCode.R) && !player.IsCasting && Spell_4_ChargesLeft > 0 && canRecast && !player.IsDead){
                Spell_4_Move();
                Spell_4_ChargesLeft--;
            }
            sc.UpdateActiveSpellSlider(imageSlider, spell_4_duration, spell_4_timer);
            spell_4_timer += Time.deltaTime;
            yield return null;
        }
        // Reset charges and start spell cooldown timer.
        Spell4Casting = false;
        StartCoroutine(sc.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
    }

    /*
    *   Spell_4_Takedown - Grants a change and increases the spells duration if the takedown was on a champion.
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
            }
        }
    }
    
    /*
    *   Spell_4 - Handles calculating where to move Bahri when spell 4 is casted so that the GameObject always ends up on the navmesh.
    */
    private void Spell_4_Move(){
        // Get the players mouse position on spell cast for spells target direction.
        Vector3 targetDirection = sc.GetTargetDirection();
        // Set the target position to be in the direction of the mouse on cast and at max spell distance from the player.
        Vector3 targetPosition = (targetDirection - transform.position);
        if(targetPosition.magnitude > spellData.maxMagnitude)
            targetPosition = transform.position + (targetPosition.normalized * spellData.maxMagnitude);
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
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        List<GameObject> targets = new List<GameObject>();
        Collider [] hitColliders = Physics.OverlapSphere(transform.position, spellData.radius, enemyMask);
        // If a target is in range.
        if(hitColliders.Length > 0){
            foreach(Collider collider in hitColliders){
                // If the target is alive.
                if(!collider.gameObject.GetComponent<IUnit>().IsDead && collider.gameObject != gameObject && collider.gameObject.GetComponent<INewDamagable>() != null){
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
                GameObject missile = (GameObject) Instantiate(spellData.missile, transform.position, Quaternion.identity);
                NewTargetedProjectile targetedProjectile = missile.GetComponent<NewTargetedProjectile>();
                targetedProjectile.hit = Hit;
                targetedProjectile.TargetUnit = target.GetComponent<IUnit>();
                // Use the same animation as spell two to send the missiles to their target.
                StartCoroutine(Spell_4_Target(missile, target));
            }
        }
    }

    /*
    *   Spell_4_Target - Move the object towards the enemy it has targeted.
    *   @param missile - GameObject to move towards the target.
    *   @param target - Target GameObject to move the spell towards.
    */
    private IEnumerator Spell_4_Target(GameObject missile, GameObject target){
        // While the GameObject still exists move it towards the target.
        while(missile && target){
            missile.transform.position = Vector3.MoveTowards(missile.transform.position, target.transform.position, spellData.missileSpeed * Time.deltaTime);
            yield return null;
        }
    }

    /*
    *   Hit - Deals fourth spells damage to the enemy hit.
    *   @param enemy - GameObject of the enemy hit.
    */
    public void Hit(IUnit unit){
        spellHitCallback?.Invoke(unit, this);
        if(unit is INewDamagable){
            float magicDamage = championStats.magicDamage.GetValue();
            ((INewDamagable) unit).TakeDamage(spellData.baseDamage[SpellLevel] + magicDamage, "magic", player, false);
        }
    }
}
