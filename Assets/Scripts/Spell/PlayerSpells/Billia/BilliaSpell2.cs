using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

/*
* Purpose: Implements Billia's second spell. Billia dashes to a target location dealing damage in circle. The damage circle contains an inner circle
* that deals double damage if a unit is hit by it.
*
* @author: Colin Keys
*/
public class BilliaSpell2 : Spell, IHasHit, IHasCast
{
    public SpellHitCallback spellHitCallback { get; set; }

    new private BilliaSpell2Data spellData;
    private NavMeshAgent navMeshAgent;
    private string radius;

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BilliaSpell2Data) base.spellData;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Vector3 targetDirection = spellController.GetTargetDirection();
        // Set the target position to be in the direction of the mouse on cast.
        Vector3 targetPosition = (targetDirection - transform.position);
        // Set the spell cast position to max range if casted past that value.
        if(targetPosition.magnitude > spellData.maxMagnitude)
            targetPosition = transform.position + (targetPosition.normalized * spellData.maxMagnitude);
        // Set the spell cast position to the minimum range if target positions magnitude is less than it.
        else if(targetPosition.magnitude < spellData.minMagnitude)
            targetPosition = transform.position + (targetPosition.normalized * spellData.minMagnitude);
        // Set target position to calculated mouse position.
        else
            targetPosition = transform.position + targetPosition;
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(targetPosition, Vector3.up, spellData.outerRadius, 1f);
        Handles.color = Color.red;
        Handles.DrawWireDisc(targetPosition, Vector3.up, spellData.innerRadius, 1f);
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.up, spellData.maxMagnitude, 1f);
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        // If the spell is off cd, Billia is not casting, and has enough mana.
        if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = spellController.GetTargetDirection();
            // Set the target position to be in the direction of the mouse on cast.
            Vector3 targetPosition = (targetDirection - transform.position);
            // Set the spell cast position to max range if casted past that value.
            if(targetPosition.magnitude > spellData.maxMagnitude)
                targetPosition = transform.position + (targetPosition.normalized * spellData.maxMagnitude);
            // Set the spell cast position to the minimum range if target positions magnitude is less than it.
            else if(targetPosition.magnitude < spellData.minMagnitude)
                targetPosition = transform.position + (targetPosition.normalized * spellData.minMagnitude);
            // Set target position to calculated mouse position.
            else
                targetPosition = transform.position + targetPosition;

            Vector3 initialTarget = targetPosition;
            // Initalize variables 
            NavMeshHit meshHit;
            int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
            // Check if there is terrain between the target location and billia.
            if(NavMesh.Raycast(transform.position, targetPosition, out meshHit, walkableMask)){
                // Use the value returned in meshHit to set a new target position.
                Vector3 temp = targetPosition;
                targetPosition = meshHit.position;
                targetPosition.y = temp.y;
            }
            // Get the direction to move Billia in using initial target.
            Vector3 directionToMove = (new Vector3(initialTarget.x, targetDirection.y, initialTarget.z) - transform.position).normalized;
            // Get the position offset to place Billia from the spell cast position.
            Vector3 billiaTargetPosition = targetPosition - (directionToMove * spellData.dashOffset);
            // Show the spells hitbox.
            Spell_2_Visual(targetPosition);
            StartCoroutine(spellController.CastTime());
            StartCoroutine(Spell_2_Dash(billiaTargetPosition, targetPosition));
            // Use mana.
            championStats.UseMana(spellData.baseMana[SpellLevel]);
            OnCd = true;
        }
    }

    /*
    *   Spell_2_Visual - Visual hit box indicator for Billia's second spell.
    *   @param targetPosition - Vector3 for the spills visual hit box.
    */
    private void Spell_2_Visual(Vector3 targetPosition){
        // Create the spells visual hitbox and set necessary values.
        GameObject visualHitbox = (GameObject) Instantiate(spellData.visualPrefab, targetPosition, Quaternion.identity);
        visualHitbox.name = "BilliaSpell_2";
        visualHitbox.transform.position = new Vector3(visualHitbox.transform.position.x, 0.5f, visualHitbox.transform.position.z);
        float yScale = visualHitbox.transform.GetChild(0).localScale.y;
        visualHitbox.transform.GetChild(0).localScale = new Vector3(spellData.innerRadius * 2f, yScale, spellData.innerRadius * 2f);
        visualHitbox.transform.GetChild(1).localScale = new Vector3(spellData.outerRadius * 2f, yScale, spellData.outerRadius * 2f);
    }

    /*
    *   Spell_2_Dash - Moves Billia to the target offset position from the spell casts position.
    *   @param targetPosition - Vector3 of the position to move Billia to.
    *   @param spellTargetPosition - Vector3 of the center of the spell.
    */
    private IEnumerator Spell_2_Dash(Vector3 targetPosition, Vector3 spellTargetPosition){
        player.CurrentCastedSpell = this;
        // Disable pathing.
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;
        // Get dash speed since dash duration is a fixed time.
        float dashSpeed = (targetPosition - transform.position).magnitude/spellData.dashTime; 
        float timer = 0.0f;
        // While still dashing.
        while(timer < spellData.dashTime){
            // Move towards target position.
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            navMeshAgent.isStopped = true;
            yield return null;
        }
        // Apply last tick dash and enable pathing.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
        navMeshAgent.isStopped = false;
        player.CurrentCastedSpell = this;
        Spell_2_Finished(spellTargetPosition);
    }

    /*
    *   Spell_2_Finished - Handles spell cd and hitbox checking when the spell 2 is finished animating.
    *   @param targetPosition - Vector3 of the center of the spell.
    */
    private void Spell_2_Finished(Vector3 targetPosition){
        StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
        // Hitbox starts from center of calculated target position.
        HitboxCheck(targetPosition);
        Destroy(GameObject.Find("/BilliaSpell_2"));
    }

    /*
    *   HitboxCheck - Checks an outer radius for any collider hits then checks if those hits are part of the inner radius damage.
    *   @param hitboxCenter - Vector3 of the position of the center of the radius' hit box.
    */
    private void HitboxCheck(Vector3 hitboxCenter){
        hitboxCenter = new Vector3(hitboxCenter.x, player.hitbox.transform.position.y, hitboxCenter.z);
        //LayerMask enemyMask = LayerMask.GetMask("Enemy");
        List<Collider> outerHit = new List<Collider>(Physics.OverlapSphere(hitboxCenter, spellData.outerRadius));
        foreach(Collider collider in outerHit){
            IUnit enemyUnit = collider.gameObject.GetComponentInParent<IUnit>();
            if(enemyUnit == null || enemyUnit == player)
                continue;
            // Check if the center of the hit collider is within the spell hitbox.
            Vector3 colliderHitCenter = collider.transform.position;
            float distToHitboxCenter = (colliderHitCenter - hitboxCenter).magnitude;
            if(distToHitboxCenter < spellData.outerRadius){
                Vector3 closestPoint = collider.ClosestPoint(hitboxCenter);
                closestPoint.y = collider.transform.position.y;
                distToHitboxCenter = (closestPoint - hitboxCenter).magnitude;
                // Check if the unit was hit by the specified spells inner damage.
                if(distToHitboxCenter < spellData.innerRadius)
                    radius = "inner";
                // Unit hit by outer portion.
                else
                    radius = "outer";
                Hit(enemyUnit);
            }
        }
    }

    /*
    *   Hit - Deals second spells damage to the enemy hit. Magic damage with inner hit dealing increased magic damage.
    *   @param unit - IUnit of the enemy hit.
    */
    public void Hit(IUnit unit){
        spellHitCallback?.Invoke(unit, this);
        if(unit is IDamageable){
            ((IDamageable) unit).TakeDamage(TotalDamage(unit), DamageType.Magic, player, false);   
        }
    }

    private float TotalDamage(IUnit unit){
        float damage;
        if(unit is IMinion)
            damage = spellData.minionDamage[SpellLevel] + (0.175f * player.unitStats.magicDamage.GetValue());
        else
            damage = spellData.baseDamage[SpellLevel] + (0.35f * player.unitStats.magicDamage.GetValue());
        if(radius == "inner")
            damage *= 2f;
        return damage;
    }
}
