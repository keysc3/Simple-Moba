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
public class BilliaSpell2 : DamageSpell, ICastable
{
    new private BilliaSpell2Data spellData;
    private string radius;

    /*
    *   BilliaSpell2 - Initialize Billia's second spell.
    *   @param championSpells - ChampionSpells instance this spell is a part of.
    *   @param spellNum - string of the spell number this spell is.
    *   @param spellData - SpellData to use.
    */
    public BilliaSpell2(ChampionSpells championSpells, SpellData spellData) : base(championSpells, spellData){
        this.spellData = (BilliaSpell2Data) spellData;
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Vector3 targetDirection = GetTargetDirection();
        // Set the target position to be in the direction of the mouse on cast.
        Vector3 targetPosition = (targetDirection - player.gameObject.transform.position);
        // Set the spell cast position to max range if casted past that value.
        if(targetPosition.magnitude > spellData.maxMagnitude)
            targetPosition = player.gameObject.transform.position + (targetPosition.normalized * spellData.maxMagnitude);
        // Set the spell cast position to the dashOffset if target positions magnitude is less than it.
        else if(targetPosition.magnitude < spellData.dashOffset)
            targetPosition = player.gameObject.transform.position + (targetPosition.normalized * spellData.dashOffset);
        // Set target position to calculated mouse position.
        else
            targetPosition = player.gameObject.transform.position + targetPosition;
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(targetPosition, Vector3.up, spellData.outerRadius, 1f);
        Handles.color = Color.red;
        Handles.DrawWireDisc(targetPosition, Vector3.up, spellData.innerRadius, 1f);
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(player.gameObject.transform.position, Vector3.up, spellData.maxMagnitude, 1f);
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        // If the spell is off cd, Billia is not casting, and has enough mana.
        if(!player.isCasting && championStats.CurrentMana >= spellData.baseMana[player.levelManager.spellLevels[spellNum]-1]){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = GetTargetDirection();
            // Set the target position to be in the direction of the mouse on cast.
            Vector3 targetPosition = (targetDirection - player.gameObject.transform.position);
            // Set the spell cast position to max range if casted past that value.
            if(targetPosition.magnitude > spellData.maxMagnitude)
                targetPosition = player.gameObject.transform.position + (targetPosition.normalized * spellData.maxMagnitude);
            // Set the spell cast position to the dashOffset if target positions magnitude is less than it.
            else if(targetPosition.magnitude < spellData.dashOffset)
                targetPosition = player.gameObject.transform.position + (targetPosition.normalized * spellData.dashOffset);
            // Set target position to calculated mouse position.
            else
                targetPosition = player.gameObject.transform.position + targetPosition;

            Vector3 initialTarget = targetPosition;
            // Initalize variables 
            NavMeshHit meshHit;
            int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
            // Check if there is terrain between the target location and billia.
            if(NavMesh.Raycast(player.gameObject.transform.position, targetPosition, out meshHit, walkableMask)){
                // Use the value returned in meshHit to set a new target position.
                Vector3 temp = targetPosition;
                targetPosition = meshHit.position;
                targetPosition.y = temp.y;
            }
            // Get the direction to move Billia in using initial target.
            Vector3 directionToMove = (new Vector3(initialTarget.x, targetDirection.y,initialTarget.z) - player.gameObject.transform.position).normalized;
            // Get the position offset to place Billia from the spell cast position.
            Vector3 billiaTargetPosition = targetPosition - (directionToMove * spellData.dashOffset);
            championSpells.StartCoroutine(CastTime(spellData.castTime, canMove));
            // Show the spells hitbox.
            Spell_2_Visual(targetPosition);
            championSpells.StartCoroutine(Spell_2_Cast(billiaTargetPosition, targetPosition));
            // Use mana.
            championStats.UseMana(spellData.baseMana[player.levelManager.spellLevels[spellNum]-1]);
            onCd = true;
        }
    }

    /*
    *   Spell_2_Visual - Visual hitbox indicator for Billia's second spell.
    */
    private void Spell_2_Visual(Vector3 targetPosition){
        // Create the spells visual hitbox and set necessary values.
        GameObject visualHitbox = (GameObject) GameObject.Instantiate(spellData.visualPrefab, targetPosition, Quaternion.identity);
        visualHitbox.name = "BilliaSpell_2";
        visualHitbox.transform.position = new Vector3(visualHitbox.transform.position.x, 0.5f, visualHitbox.transform.position.z);
        float yScale = visualHitbox.transform.GetChild(0).localScale.y;
        visualHitbox.transform.GetChild(0).localScale = new Vector3(spellData.innerRadius * 2f, yScale, spellData.innerRadius * 2f);
        visualHitbox.transform.GetChild(1).localScale = new Vector3(spellData.outerRadius * 2f, yScale, spellData.outerRadius * 2f);
    }

    /*
    *   Spell_2_Cast - Handles cast time and dash initialization of Spell 2.
    *   @param billiaTargetPosition - Vector3 of the position to move Billia to.
    *   @param targetPosition - Vecto3 of the center of the spell.
    */
    private IEnumerator Spell_2_Cast(Vector3 billiaTargetPosition, Vector3 targetPosition){
        while(player.isCasting)
            yield return null;
        // Apply the dash.
        championSpells.StartCoroutine(Spell_2_Dash(billiaTargetPosition, targetPosition));
    }

    /*
    *   Spell_2_Dash - Moves Billia to the target offset position from the spell casts position.
    *   @param targetPosition - Vector3 of the position to move Billia to.
    *   @param spellTargetPosition - Vector3 of the center of the spell.
    */
    private IEnumerator Spell_2_Dash(Vector3 targetPosition, Vector3 spellTargetPosition){
        player.isCasting = true;
        player.CurrentCastedSpell = this;
        // Disable pathing.
        player.navMeshAgent.ResetPath();
        player.navMeshAgent.isStopped = true;
        // Get dash speed since dash duration is a fixed time.
        float dashSpeed = (targetPosition - player.gameObject.transform.position).magnitude/spellData.dashTime; 
        float timer = 0.0f;
        // While still dashing.
        while(timer < spellData.dashTime){
            // Move towards target position.
            player.gameObject.transform.position = Vector3.MoveTowards(player.gameObject.transform.position, targetPosition, dashSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        // Apply last tick dash and enable pathing.
        player.gameObject.transform.position = Vector3.MoveTowards(player.gameObject.transform.position, targetPosition, dashSpeed * Time.deltaTime);
        player.navMeshAgent.isStopped = false;
        player.isCasting = false;
        player.CurrentCastedSpell = this;
        Spell_2_Finished(spellTargetPosition);
    }

    /*
    *   Spell_2_Finished - Handles spell cd and hitbox checking when the spell 2 is finished animating.
    *   @param targetPosition - Vector3 of the center of the spell.
    */
    private void Spell_2_Finished(Vector3 targetPosition){
        championSpells.StartCoroutine(Spell_Cd_Timer(spellData.baseCd[player.levelManager.spellLevels[spellNum]-1], spellNum));
        // Hitbox starts from center of calculated target position.
        HitboxCheck(targetPosition);
        Object.Destroy(GameObject.Find("/BilliaSpell_2"));
    }

    /*
    *   HitboxCheck - Checks an outer radius for any collider hits then checks if those hits are part of the inner radius damage.
    *   @param hitboxCenter - Vector3 of the position of the center of the radius' hitbox.
    */
    private void HitboxCheck(Vector3 hitboxCenter){
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        List<Collider> outerHit = new List<Collider>(Physics.OverlapSphere(hitboxCenter, spellData.outerRadius, enemyMask));
        foreach(Collider collider in outerHit){
            // Check if the center of the hit collider is within the spell hitbox.
            Vector3 colliderHitCenter = collider.bounds.center;
            float distToHitboxCenter = (colliderHitCenter - hitboxCenter).magnitude;
            if(distToHitboxCenter < spellData.outerRadius){
                Vector3 closestPoint = collider.ClosestPoint(hitboxCenter);
                closestPoint.y = collider.bounds.center.y;
                distToHitboxCenter = (closestPoint - hitboxCenter).magnitude;
                // Check if the unit was hit by the specified spells inner damage.
                if(distToHitboxCenter < spellData.innerRadius){
                    radius = "inner";
                    Hit(collider.gameObject);
                }
                // Unit hit by outer portion.
                else{
                    radius = "outer";
                    Hit(collider.gameObject);
                }
            }
        }
    }

    /*
    *   Hit - Deals second spells damage to the enemy hit. Magic damage with inner hit dealing increased magic damage.
    *   @param enemy - GameObject of the enemy hit.
    *   @param radius - string of which radius was hit.
    */
    public override void Hit(GameObject hit){
        spellHitCallback?.Invoke(hit, this);
        float magicDamage = championStats.magicDamage.GetValue();
        if(radius == "inner")
            hit.GetComponent<Unit>().TakeDamage((spellData.baseDamage[player.levelManager.spellLevels[spellNum]-1] + magicDamage) * 2f, "magic", player.gameObject, false);   
        else
            hit.GetComponent<Unit>().TakeDamage(spellData.baseDamage[player.levelManager.spellLevels[spellNum]-1] + magicDamage, "magic", player.gameObject, false);
    }
}
