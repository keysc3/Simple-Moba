using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* Purpose: Implements Burge's third spell. Burge plants herself and charges up her daggers for a jab attack, the longer the charge the more damage the ability does. 
* If the ability is charged to the max then Burge will do a short dash in a direction along with a second, wider and less length jab.
*
* @author: Colin Keys
*/
public class BurgeSpell3 : Spell, IHasCast, IHasHit
{
    new private BurgeSpell3Data spellData;
    public SpellHitCallback spellHitCallback { get; set; }
    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BurgeSpell3Data) base.spellData;
        navMeshAgent = GetComponent<NavMeshAgent>();
        IsQuickCast = true;
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            StartCoroutine(ChargeUp());
            // Use mana.
            championStats.UseMana(spellData.baseMana[SpellLevel]);
        }
    }

    /*
    *   ChargeUp - Handles the spells charge up while holding down its input key.
    */
    private IEnumerator ChargeUp(){
        player.IsCasting = true;
        player.CurrentCastedSpell = this;
        navMeshAgent.isStopped = true;
        KeyCode spellInput = KeyCode.E;
        float timer = 0.0f;
        while(Input.GetKey(spellInput) && timer < spellData.holdDuration){
            Vector3 targetDirection = spellController.GetTargetDirection();
            player.MouseOnCast = targetDirection;
            timer += Time.deltaTime;
            Debug.Log("holding: " + timer);
            yield return null;
        }
        // If the key was not released do not cast the spell.
        if(Input.GetKey(spellInput)){
            PutOnCd(false);
        }
        else{
            StartCoroutine(SpellCast(timer));
        }
    }


    /*
    *   SpellCast - Handles casting the spell
    *   @param heladDuration - float of how long the spell was charging for.
    */
    private IEnumerator SpellCast(float heldDuration){
        Vector3 targetDirection = spellController.GetTargetDirection();
        player.MouseOnCast = targetDirection;
        // Center of the spells hitbox.
        Vector3 position = transform.position + ((targetDirection - transform.position).normalized * (spellData.hitboxLength/2));
        Transform visualHitbox = ((GameObject) Instantiate(spellData.visualHitbox, position, transform.rotation)).transform;
        // Setup the spells visual.
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        visualHitbox.position = new Vector3(visualHitbox.position.x, visualHitbox.position.y - capsule.bounds.size.y/2f, visualHitbox.position.z);
        visualHitbox.localScale = new Vector3(spellData.hitboxWidth, visualHitbox.localScale.y, spellData.hitboxLength);
        float timer = 0.0f;
        // Wait out the cast time.
        while(timer < spellData.castTime){
            timer += Time.deltaTime;
            yield return null;
        }
        // Check for any hits. Hitbox starts from the middle of the player.
        List<Collider> hits = new List<Collider>(Physics.OverlapBox(position, new Vector3(spellData.hitboxWidth/2, 0.5f, spellData.hitboxLength/2), transform.rotation));
        List<IUnit> pastHits = new List<IUnit>();
        pastHits = CheckForSpellHits(hits, pastHits);
        //TODO: REMOVE
        #region "Hitbox debug lines"
        Vector3 startingPosition = transform.position;
        startingPosition.y = player.hitbox.transform.position.y;
        Vector3 targetPos = (targetDirection - transform.position).normalized;
        targetPos = transform.position + (targetPos * spellData.hitboxLength);
        targetPos.y = player.hitbox.transform.position.y;
        Debug.DrawLine(startingPosition, targetPos, Color.blue, 5f);
        Debug.DrawLine(targetPos, targetPos + (transform.right * spellData.hitboxWidth/2), Color.blue, 5f);
        Debug.DrawLine(targetPos, targetPos - (transform.right * spellData.hitboxWidth/2), Color.blue, 5f);
        #endregion
        // Cast second part of spell if it was charged enough.
        if(heldDuration >= spellData.maxChargeDuration)
            StartCoroutine(SecondCast());
        else{
            PutOnCd(true);
        }
        Destroy(visualHitbox.gameObject);
    }

    /*
    *   SecondCast - Handle casting the second part of the spell.
    */
    private IEnumerator SecondCast(){
        float timer = 0.0f;
        // Wait out cast time.
        while(timer < spellData.timeBetweenDash){
            timer += Time.deltaTime;
            yield return null;
        }
        // Get second cast position.
        Vector3 targetDirection = spellController.GetTargetDirection();
        player.MouseOnCast = targetDirection;
        //TODO: REMOVE
        #region "Hitbox2 debug lines"
        Vector3 startingPosition = transform.position;
        startingPosition.y = player.hitbox.transform.position.y;
        Vector3 targetPos = (targetDirection - transform.position).normalized;
        targetPos = transform.position + (targetPos * spellData.dashMagnitude);
        targetPos.y = player.hitbox.transform.position.y;
        IPlayerMover playerMover = GetComponentInParent<IPlayerMover>();
        playerMover.CurrentTarget = targetDirection;
        Vector3 endPosition = targetDirection;
        Debug.DrawLine(startingPosition, targetPos, Color.red, 5f);
        Debug.DrawLine(targetPos, targetPos + (transform.right * spellData.chargedHitboxWidth/2), Color.red, 5f);
        Debug.DrawLine(targetPos, targetPos - (transform.right * spellData.chargedHitboxWidth/2), Color.red, 5f);
        #endregion
        StartCoroutine(SecondSpellDash(targetDirection));
    }

    /*
    *   SecondSpellDash - Handle moving and hitbox checking of the second cast.
    *   @param targetDirection - Vector3 of the target direction of the spell cast.
    */
    private IEnumerator SecondSpellDash(Vector3 targetDirection){
        // Get the final position after dash is applied.
        Vector3 targetPosition = (targetDirection - transform.position).normalized;
        targetPosition = transform.position + (targetPosition * spellData.dashMagnitude);
        targetPosition = GetFinalPosition(targetPosition);
        float timer = 0.0f;
        List<IUnit> pastHits = new List<IUnit>();
        Vector3 startingPosition = transform.position;
        // While still dashing.
        while(timer < spellData.dashTime && !player.IsDead){
            // Move towards target position. Hitbox starts at center of player.
            Vector3 position = transform.position + ((targetDirection - transform.position).normalized * (spellData.chargedHitboxLength/2));
            List<Collider> hits = new List<Collider>(Physics.OverlapBox(position, new Vector3(spellData.chargedHitboxWidth/2, 0.5f, spellData.chargedHitboxLength/2), transform.rotation));
            pastHits = CheckForSpellHits(hits, pastHits);
            transform.position = Vector3.Lerp(startingPosition, targetPosition, timer/spellData.dashTime);
            timer += Time.deltaTime;
            //navMeshAgent.isStopped = true;
            yield return null;
        }
        //TODO: Last hit check, move repeated lines.
        // Apply last tick dash and enable pathing.
        if(!player.IsDead)
            transform.position = Vector3.Lerp(startingPosition, targetPosition, 1);
        //navMeshAgent.isStopped = false;
        PutOnCd(true);
    }

    /*
    *   GetFinalPosition - Gets the position the player will end up at after the second dash.
    *   @param targetPosition - Vector3 of the targeted position.
    */
    private Vector3 GetFinalPosition(Vector3 targetPosition){
        // Initialize variables 
        NavMeshHit meshHit;
        int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
        // Check if there is terrain between the target location and Burge.
        if(NavMesh.Raycast(transform.position, targetPosition, out meshHit, walkableMask)){
            // Use the value returned in meshHit to set a new target position.
            Vector3 temp = targetPosition;
            targetPosition = meshHit.position;
            targetPosition.y = temp.y;
        }
        return targetPosition;
    }

    /*
    *   CheckForSpellHits - Checks for any spell hits from a list of hit colliders.
    *   @param hits - List of colliders to check.
    *   @param pastHits - List of IUnits that have already been hit by the spell.
    *   @return List<IUnit> - List of IUnits that have been hit.
    */
    private List<IUnit> CheckForSpellHits(List<Collider> hits, List<IUnit> pastHits){
        foreach(Collider collider in hits){
            if(collider.transform.name == "Hitbox" && collider.transform.parent != transform){
                IUnit hitUnit = collider.gameObject.GetComponentInParent<IUnit>();
                if(hitUnit != null && !pastHits.Contains(hitUnit)){
                    Hit(hitUnit);
                    pastHits.Add(hitUnit);
                }
            }
        }
        return pastHits;
    }

    /*
    *   PutOnCd - Handles the end of spell clean up.
    *   @param casted - bool for if the spell has been casted.
    */
    private void PutOnCd(bool casted){
        // Set fields back to non-casting state.
        player.IsCasting = false;
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = false;
        // Put spell on full cd if it was casted.
        float cd = spellData.baseCd[SpellLevel];
        if(!casted)
            cd = cd * (1f - spellData.cdRefundPercent);
        OnCd = true;
        StartCoroutine(spellController.Spell_Cd_Timer(cd));
    }

    /*
    *   Hit - Deals third spells damage to the enemy hit.
    *   @param unit - IUnit of the enemy hit.
    */
    public void Hit(IUnit hit){
        //TODO: Handle hit damage.
        Debug.Log("HIT: " + hit.GameObject.transform.name);
    }
}
