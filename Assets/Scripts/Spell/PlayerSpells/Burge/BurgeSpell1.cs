using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

/*
* Purpose: Implements Burge's first spell. Burge vaults into the air spinning their weapon and deals damage on cast and every x seconds
* after as she goes through the air. Burge can go over terrain and always vaults a set distance.
*
* @author: Colin Keys
*/
public class BurgeSpell1 : Spell, IHasHit, IHasCast
{
    new private BurgeSpell1Data spellData;
    private NavMeshAgent navMeshAgent;
    private float p1_y_offset = 5f;

    public SpellHitCallback spellHitCallback { get; set; }

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BurgeSpell1Data) base.spellData;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = spellController.GetTargetDirection();
            // Set the target position to be in the direction of the mouse on cast and at max spell distance from the player.
            Vector3 targetPosition = (targetDirection - transform.position).normalized;
            targetPosition = transform.position + (targetPosition * spellData.magnitude);
            // Start coroutines to handle the spells cast time and animation.
            StartCoroutine(spellController.CastTime());
            StartCoroutine(Move(targetPosition));
            // Use mana and set spell on cooldown to true.
            championStats.UseMana(spellData.baseMana[SpellLevel]);
            OnCd = true;
        }
    }
    //TODO: SPLIT UP
    /*
    *   Spell_4 - Handles calculating where to move Bahri when spell 4 is casted so that the GameObject always ends up on the navmesh.
    */
    private IEnumerator Move(Vector3 targetPosition){
        while(player.IsCasting)
            yield return null;
         // Set necessary values and disable navmesh.
        player.IsCasting = true;
        player.CurrentCastedSpell = this;
        float newSpeed = championStats.speed.GetValue() + spellData.jumpTime;
        navMeshAgent.ResetPath();
        navMeshAgent.enabled = false;
        targetPosition = spellController.GetPositionOnWalkableNavMesh(targetPosition, true);
        Vector3 targetDirection = targetPosition - transform.position;
        // Set p0.
        Vector3 p0 = transform.position;
        // Set p1. X and Z of p1 are halfway between Billia and target position. Y of p1 is an offset value.
        Vector3 p1 = transform.position;
        p1.y = p1.y + p1_y_offset;
        Vector3 dir = targetDirection.normalized;
        float mag = targetDirection.magnitude;
        p1.x = p1.x + (dir.x * (mag/2f));
        p1.z = p1.z + (dir.z * (mag/2f));
        Vector3 p2 = targetPosition;
        List<float> hitTimes = GetHitPositions(targetDirection);
        int nextHitIndex = 0;
        Vector3 targetHitboxPosition = targetPosition;
        targetHitboxPosition.y = player.hitbox.transform.position.y;
        // While lob time has not finished.
        float timer = 0.0f;
        while(timer < spellData.jumpTime){
            // Get t value, a value between 0 and 1.
            float t = Mathf.Clamp01(timer/spellData.jumpTime);
            // Get the next position on the Quadratic Bezier curve.
            Vector3 point = spellController.QuadraticBezierCurvePoint(t, p0, p1, p2);
            // Set the seeds new position.
            transform.position = point;
            if(nextHitIndex <= hitTimes.Count - 1 && timer >= hitTimes[nextHitIndex]){
                Debug.Log("Hit position #" + nextHitIndex + " Hit time: " + hitTimes[nextHitIndex]);
                List<Collider> outerHit = new List<Collider>(Physics.OverlapBox(player.hitbox.transform.position, new Vector3(spellData.hitboxWidth/2, 0.5f, spellData.hitboxLength/2), transform.rotation));
                foreach(Collider collider in outerHit){
                    if(collider.transform.name == "Hitbox" && collider.transform.parent != transform){
                        IUnit hitUnit = collider.gameObject.GetComponentInParent<IUnit>();
                        if(hitUnit != null){
                            Hit(hitUnit);
                        }
                    }
                }
                nextHitIndex += 1;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        // Set the final point.
        Vector3 lastPoint = spellController.QuadraticBezierCurvePoint(1, p0, p1, p2);
        transform.position = lastPoint;
        navMeshAgent.enabled = true;
        player.IsCasting = false;
        StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
    }

    /*
    *   GetHitPositions - Gets the time to check for any spell hits.
    *   @return List<float> - List of floats representing the time after spell cast the spell will deal damage. 
    */
    private List<float> GetHitPositions(){
        float timeBetweenHits = spellData.jumpTime/spellData.numberOfHits;
        List<float> hitPositions = new List<float>();
        float hitTimer = 0f;
        for(int i = 0; i < spellData.numberOfHits; i++){
            hitPositions.Add(hitTimer);
            hitTimer += timeBetweenHits;
        }
        return hitPositions;
    }

    /*
    *   Hit - Deals first spells damage to the enemy hit.
    *   @param unit - IUnit of the enemy hit.
    */
    public void Hit(IUnit unit){
        spellHitCallback?.Invoke(unit, this);
        if(unit is IDamageable){
            float damageValue = spellData.baseDamage[SpellLevel] + (0.2f * championStats.physicalDamage.GetValue());
            ((IDamageable) unit).TakeDamage(damageValue, DamageType.Physical, player, false);
        }
    }
}
