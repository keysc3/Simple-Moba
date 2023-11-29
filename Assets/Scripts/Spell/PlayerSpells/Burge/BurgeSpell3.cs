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
            //OnCd = true;
            //StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
        }
    }

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
        if(Input.GetKey(spellInput)){
            PutOnCd(false);
        }
        else{
            StartCoroutine(SpellCast(timer));
        }
    }


    private IEnumerator SpellCast(float heldDuration){
        Vector3 targetDirection = spellController.GetTargetDirection();
        player.MouseOnCast = targetDirection;
        Vector3 position = transform.position + ((targetDirection - transform.position).normalized * (spellData.hitboxLength/2));
        Transform visualHitbox = ((GameObject) Instantiate(spellData.visualHitbox, position, transform.rotation)).transform;
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        visualHitbox.position = new Vector3(visualHitbox.position.x, visualHitbox.position.y - capsule.bounds.size.y/2f, visualHitbox.position.z);
        visualHitbox.localScale = new Vector3(spellData.hitboxWidth, visualHitbox.localScale.y, spellData.hitboxLength);
        float timer = 0.0f;
        while(timer < spellData.castTime){
            timer += Time.deltaTime;
            yield return null;
        }
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
        if(heldDuration >= spellData.maxChargeDuration)
            StartCoroutine(SecondCast());
        else{
            PutOnCd(true);
        }
        Destroy(visualHitbox.gameObject);
    }

    private IEnumerator SecondCast(){
        float timer = 0.0f;
        while(timer < spellData.timeBetweenDash){
            timer += Time.deltaTime;
            yield return null;
        }
        // Get second cast position
        Vector3 targetDirection = spellController.GetTargetDirection();
        player.MouseOnCast = targetDirection;
        //Vector3 position = transform.position + ((targetDirection - transform.position).normalized * (spellData.chargedHitboxLength/2));
        //List<Collider> hits = new List<Collider>(Physics.OverlapBox(position, new Vector3(spellData.chargedHitboxWidth/2, 0.5f, spellData.chargedHitboxLength/2), transform.rotation));
        //List<IUnit> pastHits = new List<IUnit>();
        //pastHits = CheckForSpellHits(hits, pastHits);
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

    private IEnumerator SecondSpellDash(Vector3 targetDirection){
        Vector3 targetPosition = (targetDirection - transform.position).normalized;
        targetPosition = transform.position + (targetPosition * spellData.dashMagnitude);
        targetPosition = GetFinalPosition(targetPosition);
        float timer = 0.0f;
        List<IUnit> pastHits = new List<IUnit>();
        Vector3 startingPosition = transform.position;
        // While still dashing.
        while(timer < spellData.dashTime && !player.IsDead){
            // Move towards target position.
            Vector3 position = transform.position + ((targetDirection - transform.position).normalized * (spellData.chargedHitboxLength/2));
            List<Collider> hits = new List<Collider>(Physics.OverlapBox(position, new Vector3(spellData.chargedHitboxWidth/2, 0.5f, spellData.chargedHitboxLength/2), transform.rotation));
            pastHits = CheckForSpellHits(hits, pastHits);
            transform.position = Vector3.Lerp(startingPosition, targetPosition, timer/spellData.dashTime);
            timer += Time.deltaTime;
            //navMeshAgent.isStopped = true;
            yield return null;
        }
        // Apply last tick dash and enable pathing.
        if(!player.IsDead)
            transform.position = Vector3.Lerp(startingPosition, targetPosition, 1);
        //navMeshAgent.isStopped = false;
        PutOnCd(true);
    }

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

    private void PutOnCd(bool casted){
        player.IsCasting = false;
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = false;
        float cd = spellData.baseCd[SpellLevel];
        if(!casted)
            cd = cd * (1f - spellData.cdRefundPercent);
        OnCd = true;
        StartCoroutine(spellController.Spell_Cd_Timer(cd));
    }   
    public void Hit(IUnit hit){
        Debug.Log("HIT: " + hit.GameObject.transform.name);
    }
}
