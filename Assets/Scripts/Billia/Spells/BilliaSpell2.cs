using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BilliaSpell2 : DamageSpell
{
    private BilliaSpell2Data spellData;
    private string radius;

    public BilliaSpell2(ChampionSpells championSpells, SpellData spellData) : base(championSpells){
        this.spellData = (BilliaSpell2Data) spellData;
    }

    /*public override void Cast(){
        GameObject hitObject = null;
        Debug.Log("Spell2");
        Hit(hitObject);
    }*/

    /*
    *   Spell_2 - Set up Billia's second spell. She dashed an offset distance towards her target location then deals damage in two radius'.
    *   The inner radius deals bonus damage.
    */
    public override void Cast(){
        // If the spell is off cd, Billia is not casting, and has enough mana.
        if(!onCd && !player.isCasting && championStats.currentMana >= spellData.baseMana[levelManager.spellLevels["Spell_2"]-1]){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = GetTargetDirection();
            // Set the target position to be in the direction of the mouse on cast.
            Vector3 targetPosition = (targetDirection - gameObject.transform.position);
            // Set the spell cast position to max range if casted past that value.
            if(targetPosition.magnitude > spellData.maxMagnitude)
                targetPosition = gameObject.transform.position + (targetPosition.normalized * spellData.maxMagnitude);
            // Set the spell cast position to the dashOffset if target positions magnitude is less than it.
            else if(targetPosition.magnitude < spellData.dashOffset)
                targetPosition = gameObject.transform.position + (targetPosition.normalized * spellData.dashOffset);
            // Set target position to calculated mouse position.
            else
                targetPosition = gameObject.transform.position + targetPosition;

            Vector3 initialTarget = targetPosition;
            // Initalize variables 
            NavMeshHit meshHit;
            int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
            // Check if there is terrain between the target location and billia.
            if(NavMesh.Raycast(gameObject.transform.position, targetPosition, out meshHit, walkableMask)){
                // Use the value returned in meshHit to set a new target position.
                Vector3 temp = targetPosition;
                targetPosition = meshHit.position;
                targetPosition.y = temp.y;
            }
            // Get the direction to move Billia in using initial target.
            Vector3 directionToMove = (new Vector3(initialTarget.x, targetDirection.y,initialTarget.z) - gameObject.transform.position).normalized;
            // Get the position offset to place Billia from the spell cast position.
            Vector3 billiaTargetPosition = targetPosition - (directionToMove * spellData.dashOffset);
            championSpells.StartCoroutine(CastTime(spellData.castTime, canMove));
            // Show the spells hitbox.
            Spell_2_Visual(targetPosition);
            championSpells.StartCoroutine(Spell_2_Cast(billiaTargetPosition, targetPosition));
            // Use mana.
            championStats.UseMana(spellData.baseMana[levelManager.spellLevels["Spell_2"]-1]);
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
        player.SetIsCasting(true, this);
        // Disable pathing.
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;
        // Get dash speed since dash duration is a fixed time.
        float dashSpeed = (targetPosition - gameObject.transform.position).magnitude/spellData.dashTime; 
        float timer = 0.0f;
        // While still dashing.
        while(timer < spellData.dashTime){
            // Move towards target position.
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition, dashSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
        // Apply last tick dash and enable pathing.
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition, dashSpeed * Time.deltaTime);
        navMeshAgent.isStopped = false;
        player.SetIsCasting(false, this);
        Spell_2_Finished(spellTargetPosition);
    }

    /*
    *   Spell_2_Finished - Handles spell cd and hitbox checking when the spell 2 is finished animating.
    *   @param targetPosition - Vector3 of the center of the spell.
    */
    private void Spell_2_Finished(Vector3 targetPosition){
        championSpells.StartCoroutine(Spell_Cd_Timer(spellData.baseCd[levelManager.spellLevels["Spell_2"]-1], (myBool => onCd = myBool), "Spell_2"));
        // Hitbox starts from center of calculated target position.
        HitboxCheck(targetPosition);
        Object.Destroy(GameObject.Find("/BilliaSpell_2"));
    }

    /*
    *   DoubleRadiusHitboxCheck - Checks an outer radius for any collider hits then checks if those hits are part of the inner radius damage.
    *   The appropriate spells damage method and radius is used based on the results.
    *   @param hitboxCenter - Vector3 of the position of the center of the radius' hitbox.
    *   @param outerRadius - float of the outer radius value to be used.
    *   @param innerRadius - float of the inner radius value to be used.
    *   @param spell - string of the spell that has been casted.
    *   @param DoubleRadiusHitboxHit - delegate containing the method to call if a spell hit is found.
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
    *   Spell_2_Hit - Deals second spells damage to the enemy hit. Magic damage with inner hit dealing increased magic damage.
    *   @param enemy - GameObject of the enemy hit.
    *   @param radius - string of which radius was hit.
    */
    public override void Hit(GameObject hit){
        spellHitCallback?.Invoke(hit, this);
        float magicDamage = championStats.magicDamage.GetValue();
        if(radius == "inner")
            hit.GetComponent<Unit>().TakeDamage((spellData.baseDamage[levelManager.spellLevels["Spell_2"]-1] + magicDamage) * 2f, "magic", gameObject, false);   
        else
            hit.GetComponent<Unit>().TakeDamage(spellData.baseDamage[levelManager.spellLevels["Spell_2"]-1] + magicDamage, "magic", gameObject, false);
    }

    /*public override void Hit(GameObject hit){
        GameObject hitObject = null;
        spellHitCallback?.Invoke(hitObject);
        Debug.Log("Spell2Hit");
    }*/
}
