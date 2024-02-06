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
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.up, spellData.magnitude, 1f);
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = spellController.GetTargetDirection();
            player.MouseOnCast = targetDirection;
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

    /*
    *   SetupBezierPoints - Sets up the bezier points for Burge's vault path.
    *   @return List<Vector> - List of Vector3 representing the bezier points.
    */
    private List<Vector3> SetupBezierPoints(Vector3 targetPosition){
        Vector3 targetDirection = targetPosition - transform.position;
        // Set p0.
        Vector3 p0 = transform.position;
        // Set p1. X and Z of p1 are halfway between Burge and target position. Y of p1 is an offset value.
        Vector3 p1 = transform.position;
        p1.y = p1.y + p1_y_offset;
        Vector3 dir = targetDirection.normalized;
        float mag = targetDirection.magnitude;
        p1.x = p1.x + (dir.x * (mag/2f));
        p1.z = p1.z + (dir.z * (mag/2f));
        Vector3 p2 = targetPosition;
        return new List<Vector3>(){p0, p1, p2};
    }

    /*
    *   Move - Handles calculating where to move Burge when spell 1 is casted.
    */
    private IEnumerator Move(Vector3 targetPosition){
        while(player.IsCasting)
            yield return null;
        // Set necessary values and disable navmesh.
        player.IsCasting = true;
        player.CurrentCastedSpell = this;
        navMeshAgent.ResetPath();
        navMeshAgent.enabled = false;
        targetPosition = spellController.GetPositionOnWalkableNavMesh(targetPosition, true);
        List<Vector3> bezierPoints = SetupBezierPoints(targetPosition);
        List<float> hitTimes = GetHitPositions();
        int nextHitIndex = 0;
        // While lob time has not finished.
        float timer = 0.0f;
        while(timer < spellData.jumpTime){
            // Get t value, a value between 0 and 1.
            float t = Mathf.Clamp01(timer/spellData.jumpTime);
            // Get the next position on the Quadratic Bezier curve.
            Vector3 point = spellController.QuadraticBezierCurvePoint(t, bezierPoints[0], bezierPoints[1], bezierPoints[2]);
            // Set the players new position.
            transform.position = point;
            if(nextHitIndex <= hitTimes.Count - 1 && timer >= hitTimes[nextHitIndex]){
                CheckForSpellHits();
                nextHitIndex += 1;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        // Set the final point.
        Vector3 lastPoint = spellController.QuadraticBezierCurvePoint(1, bezierPoints[0], bezierPoints[1], bezierPoints[2]);
        transform.position = lastPoint;
        navMeshAgent.enabled = true;
        player.IsCasting = false;
        StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
    }

    /*
    *   CheckForSpellHits - Checks if there are any hits at the current players position using the spells hitbox.
    */
    private void CheckForSpellHits(){
        List<Collider> outerHit = new List<Collider>(Physics.OverlapBox(player.hitbox.transform.position, new Vector3(spellData.hitboxWidth/2, 0.5f, spellData.hitboxLength/2), transform.rotation, hitboxMask));
        foreach(Collider collider in outerHit){
            IUnit hitUnit = collider.gameObject.GetComponentInParent<IUnit>();
            if(hitUnit != null && hitUnit != player){
                Hit(hitUnit);
            }
        }
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
