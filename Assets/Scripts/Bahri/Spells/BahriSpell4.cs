using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BahriSpell4 : DamageSpell
{
    private BahriSpell4Data spellData;

    private PersonalSpell spell4Effect = null;
    private float spell_4_timer;
    private float spell_4_duration;
    private int spell_4_chargesLeft;
    private bool spell4Casting;

    public BahriSpell4(ChampionSpells championSpells, SpellData spellData) : base(championSpells){
        this.spellData = (BahriSpell4Data) spellData;
        player.score.takedownCallback += Spell_4_Takedown;
    }

    /*
    *   Spell_4 - Sets up and creates the players fourth spell GameObjects. The spell quickly moves Bahri in the target direction and launches projectiles 
    *  spell4Effect at up to three enemies in range upon reaching the dashes end location. The spell lasts a set duration and can be re-casted 2 times with a 1s lockout on re-casting.
    */
    public override void Cast(){
        if(levelManager.spellLevels["Spell_4"] > 0){
            if(!onCd && !player.isCasting && championStats.currentMana >= spellData.baseMana[levelManager.spellLevels["Spell_4"]-1]){
                championSpells.StartCoroutine(Spell_4_Start());
                // Use mana and set spell on cooldown.
                championStats.UseMana(spellData.baseMana[levelManager.spellLevels["Spell_4"]-1]);
                onCd = true;
            }
        }
    }

    /*
    *   Spell_4_Start - Handles the fourth spells first cast and re-casting.
    */
    private IEnumerator Spell_4_Start(){
        spell4Effect = (PersonalSpell) spellData.spell4.InitializeEffect(levelManager.spellLevels["Spell_4"]-1, gameObject, gameObject);
        player.statusEffects.AddEffect(spell4Effect);
        spell_4_timer = 0.0f;
        spell_4_duration = spellData.duration;
        float lastCastTimer = 0.0f;
        bool isCd = true;
        spell4Casting = true;
        Spell_4_Move();
        championSpells.StartCoroutine(Spell_Cd_Timer(1.0f, (myBool => isCd = myBool), "Spell_4"));
        spell_4_chargesLeft = spellData.charges - 1;
        // While the spells duration has not expired.
        while(spell_4_timer < spell_4_duration){
            // If the player re-casts, isn't casting, has spell charges left, is re-casting at least 1s since last cast, and isn't dead.
            if(Input.GetKeyDown(KeyCode.R) && !player.isCasting && spell_4_chargesLeft > 0 && !isCd && !player.isDead){
                Spell_4_Move();
                isCd = true;
                championSpells.StartCoroutine(Spell_Cd_Timer(1.0f, (myBool => isCd = myBool), "Spell_4"));
                lastCastTimer = 0.0f;
                spell_4_chargesLeft -= 1;
                spell4Effect.UpdateStacks(spell_4_chargesLeft);
            }
            UIManager.instance.SetSpellActiveDuration(4, spell_4_duration, spell_4_timer, player.playerUI);
            if(spell_4_chargesLeft == 0)
                UIManager.instance.SetSpellCoverActive(4, true, player.playerUI);
            spell_4_timer += Time.deltaTime;
            lastCastTimer += Time.deltaTime;
            yield return null;
        }
        // Reset charges and start spell cooldown timer.
        spell4Casting = false;
        UIManager.instance.SetSpellDurationOver(4, player.playerUI);
        championSpells.StartCoroutine(Spell_Cd_Timer(spellData.baseCd[levelManager.spellLevels["Spell_4"]-1], (myBool => onCd = myBool), "Spell_4"));
    }

    private void Spell_4_Takedown(GameObject killed){
        if(killed.GetComponent<Unit>().unit is ScriptableChampion){
            if(spell_4_chargesLeft < spellData.charges && spell4Casting){
                UIManager.instance.SetSpellCoverActive(4, false, player.playerUI);
                spell_4_chargesLeft += 1;
                spell_4_timer = 0.0f;
                spell_4_duration = 10.0f;
                spell4Effect.UpdateStacks(spell_4_chargesLeft);
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
        Vector3 targetPosition = (targetDirection - gameObject.transform.position);
        if(targetPosition.magnitude > spellData.maxMagnitude)
            targetPosition = gameObject.transform.position + (targetPosition.normalized * spellData.maxMagnitude);
        else
            targetPosition = gameObject.transform.position + (targetDirection - gameObject.transform.position);
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
                if(NavMesh.Raycast(gameObject.transform.position, targetPosition, out meshHit, walkableMask)){
                    // Use the value returned in meshHit to set a new target position on a walkable area in the direction of the original target position.
                    temp = targetPosition;
                    targetPosition = meshHit.position;
                    targetPosition.y = temp.y;
                }
            }
        }
        // Start coroutines to handle the spells cast time and animation.
        championSpells.StartCoroutine(Spell_4_Speed(targetPosition));
    }

    /*
    *   Spell_4_Speed - Moves Bahri to the targetPosition of the last cast of spell 4.
    *   @param targetPosition - Vector3 of the target position to cast the spell towards.
    */
    private IEnumerator Spell_4_Speed(Vector3 targetPosition){
        // Set necessary values and disable navmesh.
        player.SetIsCasting(true);
        float newSpeed = championStats.speed.GetValue() + spellData.speed;
        navMeshAgent.ResetPath();
        navMeshAgent.enabled = false;
        // While not at target position or not dead.
        while(gameObject.transform.position != targetPosition && !player.isDead){
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition, newSpeed * Time.deltaTime);
            yield return null;
        }
        // Only fire end of dash projectiles if still alive.
        if(!player.isDead)
            Spell_4_Missiles();
        navMeshAgent.enabled = true;
        player.SetIsCasting(false);
    }

    /*
    *   Spell_4_Missles - Handles creating up to three missiles from the end of Bahris spell 4 dash if any targets are found.
    */
    private void Spell_4_Missiles(){
        // Set up necessary variables.
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        List<GameObject> targets = new List<GameObject>();
        Collider [] hitColliders = Physics.OverlapSphere(gameObject.transform.position, spellData.radius, enemyMask);
        // If a target is in range.
        if(hitColliders.Length > 0){
            foreach(Collider collider in hitColliders){
                // If the target is alive.
                if(!collider.gameObject.GetComponent<Unit>().isDead && collider.gameObject != gameObject){
                    // If three targets have already been found.
                    if(targets.Count > 2){
                        // Set the farthest enemy as first in the targets found list.
                        int furthestEnemyIndex = 0;
                        float furthestEnemyDist = (gameObject.transform.position - targets[0].transform.position).magnitude;
                        // Check the other two targets against the first and set the farthest target in the targets found list.
                        for(int i = 1; i < 3; i++){
                            float distToEnemy = (gameObject.transform.position - targets[i].transform.position).magnitude;
                            if(distToEnemy > furthestEnemyDist){
                                furthestEnemyIndex = i;
                                furthestEnemyDist = distToEnemy;
                            }
                        }
                        // If the farthest target in the targets found list is farther than the new target then replace it.
                        float newEnemyDist = (gameObject.transform.position - collider.gameObject.transform.position).magnitude;
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
                GameObject missile = (GameObject) GameObject.Instantiate(spellData.missile, gameObject.transform.position, Quaternion.identity);
                //SpellObjectCreated(missile);
                TargetedProjectile targetedProjectile = missile.GetComponent<TargetedProjectile>();
                targetedProjectile.hit = Hit;
                targetedProjectile.SetTarget(target);
                //spell2Trigger.SetSpellCast(4);
                // Use the same animation as spell two to send the missiles to their target.
                championSpells.StartCoroutine(Spell_4_Target(missile, target));
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
        while(missile){
            missile.transform.position = Vector3.MoveTowards(missile.transform.position, target.transform.position, spellData.missileSpeed * Time.deltaTime);
            yield return null;
        }
    }

     /*
    *   Spell_4_Hit - Deals fourth spells damage to the enemy hit.
    *   @param enemy - GameObject of the enemy hit.
    */
    public override void Hit(GameObject enemy){
        float magicDamage = championStats.magicDamage.GetValue();
        enemy.GetComponent<Unit>().TakeDamage(spellData.baseDamage[levelManager.spellLevels["Spell_4"]-1] + magicDamage, "magic", gameObject, false);
    }
    
}
