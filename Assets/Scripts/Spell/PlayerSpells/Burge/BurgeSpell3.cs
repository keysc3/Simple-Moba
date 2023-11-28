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
            SpellCast(timer);
        }
    }

    private void SpellCast(float heldDuration){
        Vector3 targetDirection = spellController.GetTargetDirection();
        player.MouseOnCast = targetDirection;
        List<Collider> hits = new List<Collider>(Physics.OverlapBox(player.hitbox.transform.position, new Vector3(spellData.hitboxWidth/2, 0.5f, spellData.hitboxLength/2), transform.rotation));
        CheckForSpellHits(hits);
        if(heldDuration >= spellData.maxChargeDuration)
            StartCoroutine(SecondCast());
        else{
            PutOnCd(true);
        }
    }

    private IEnumerator SecondCast(){
        float timer = 0.0f;
        while(timer < spellData.timeBetweenDash){
            timer += Time.deltaTime;
            yield return null;
        }
        List<Collider> hits = new List<Collider>(Physics.OverlapBox(player.hitbox.transform.position, new Vector3(spellData.chargedHitboxWidth/2, 0.5f, spellData.chargedHitboxLength/2), transform.rotation));
        CheckForSpellHits(hits);
        StartCoroutine(SecondSpellDash());
    }

    private IEnumerator SecondSpellDash(){
        // Disable pathing.
        //navMeshAgent.ResetPath();
        // Get dash position
        Vector3 targetDirection = spellController.GetTargetDirection();
        player.MouseOnCast = targetDirection;
        Vector3 targetPosition = (targetDirection - transform.position).normalized;
        targetPosition = transform.position + (targetPosition * spellData.dashMagnitude);
        targetPosition = GetFinalPosition(targetPosition);
        // Get dash speed since dash duration is a fixed time.
        float dashSpeed = (targetPosition - transform.position).magnitude/spellData.dashTime; 
        float timer = 0.0f;
        // While still dashing.
        while(timer < spellData.dashTime && !player.IsDead){
            // Move towards target position.
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            //navMeshAgent.isStopped = true;
            yield return null;
        }
        // Apply last tick dash and enable pathing.
        if(!player.IsDead)
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
        //navMeshAgent.isStopped = false;
        PutOnCd(true);
    }

    private Vector3 GetFinalPosition(Vector3 targetPosition){
        // Initalize variables 
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

    private void CheckForSpellHits(List<Collider> hits){
        foreach(Collider collider in hits){
            if(collider.transform.name == "Hitbox" && collider.transform.parent != transform){
                IUnit hitUnit = collider.gameObject.GetComponentInParent<IUnit>();
                if(hitUnit != null){
                    Hit(hitUnit);
                }
            }
        }
    }

    private void PutOnCd(bool casted){
        player.IsCasting = false;
        navMeshAgent.isStopped = false;
        float cd = spellData.baseCd[SpellLevel];
        if(!casted)
            cd = cd * (1f - spellData.cdRefundPercent);
        OnCd = true;
        StartCoroutine(spellController.Spell_Cd_Timer(cd));
    }   
    public void Hit(IUnit hit){

    }
}
