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
    private float chargeAmount = 0.0f;
    private List<IUnit> pastHits = new List<IUnit>();
    private bool isFirstCast = false;
    IPlayerMover playerMover;

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BurgeSpell3Data) base.spellData;
        navMeshAgent = GetComponent<NavMeshAgent>();
        IsQuickCast = true;
        playerMover = GetComponentInParent<IPlayerMover>();
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Vector3 targetDirection = spellController.GetTargetDirection();
        DrawCast(targetDirection, false);
        DrawCast(targetDirection, true);
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            isFirstCast = true;
            chargeAmount = 0.0f;
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
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = true;
        KeyCode spellInput = KeyCode.E;
        chargeAmount = 0.0f;
        while(Input.GetKey(spellInput) && chargeAmount < spellData.holdDuration){
            Vector3 targetDirection = spellController.GetTargetDirection();
            DrawCast(targetDirection, true);
            player.MouseOnCast = targetDirection;
            spellController.ChargeUpUI(chargeAmount, spellData.maxChargeDuration, false);
            chargeAmount += Time.deltaTime;
            yield return null;
        }
        spellController.ChargeUpUI(spellData.maxChargeDuration, spellData.maxChargeDuration, true);
        // If the key was not released do not cast the spell.
        if(Input.GetKey(spellInput)){
            PutOnCd(false);
        }
        else{
            StartCoroutine(SpellCast());
        }
    }

    private void DrawCast(Vector3 targetDirection, bool isFirst){
        Color color = Color.cyan;
        float multi = spellData.hitboxLength;
        if(!isFirst){
            color = Color.red;
            multi = spellData.dashMagnitude + spellData.chargedHitboxLength;
        }
        Vector3 startingPosition = transform.position;
        Vector3 targetPos = (targetDirection - transform.position).normalized;
        targetPos = transform.position + (targetPos * multi);
        Debug.DrawLine(startingPosition, targetPos, color);
        //Debug.DrawLine(targetPos, targetPos + (transform.right * spellData.hitboxWidth/2), Color.blue);
        //Debug.DrawLine(targetPos, targetPos - (transform.right * spellData.hitboxWidth/2), Color.blue);
    }

    /*
    *   SpellCast - Handles casting the spell.
    */
    private IEnumerator SpellCast(){
        Vector3 targetDirection = spellController.GetTargetDirection();
        player.MouseOnCast = targetDirection;
        playerMover.CurrentTarget = targetDirection;
        // Center of the spells hitbox.
        Vector3 position = transform.position + ((targetDirection - transform.position).normalized * (spellData.hitboxLength/2));
        GameObject visualHitbox = CreateFirstCastVisual(position);
        float timer = 0.0f;
        // Wait out the cast time.
        while(timer < spellData.castTime){
            if(chargeAmount >= spellData.maxChargeDuration)
                DrawCast(spellController.GetTargetDirection(), false);
            timer += Time.deltaTime;
            yield return null;
        }
        // Check for any hits.
        CheckForSpellHits(position, spellData.hitboxWidth, spellData.hitboxLength);
        StartCoroutine(spellController.Fade(visualHitbox, spellData.firstCastFadeTime));
        // Cast second part of spell if it was charged enough.
        if(chargeAmount >= spellData.maxChargeDuration)
            StartCoroutine(SecondCast());
        else{
            PutOnCd(true);
        }
    }

    /*
    *   CreateFirstCastVisual - Creates the visual effect for the first cast jab.
    *   @param position - Vector3 of the x and z center of the visual.
    *   @return GameObject - The created visual.
    */
    private GameObject CreateFirstCastVisual(Vector3 position){
        Transform visualHitbox = ((GameObject) Instantiate(spellData.visualHitbox, position, transform.rotation)).transform;
        // Setup the spells visual.
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        visualHitbox.position = new Vector3(visualHitbox.position.x, visualHitbox.position.y - capsule.bounds.size.y/2f, visualHitbox.position.z);
        visualHitbox.localScale = new Vector3(spellData.hitboxWidth, visualHitbox.localScale.y, spellData.hitboxLength);
        return visualHitbox.gameObject;
    }

    /*
    *   SecondCast - Handle casting the second part of the spell.
    */
    private IEnumerator SecondCast(){
        float timer = 0.0f;
        // Wait out cast time.
        while(timer < spellData.timeBetweenDash){
            DrawCast(spellController.GetTargetDirection(), false);
            timer += Time.deltaTime;
            yield return null;
        }
        // Get second cast position.
        Vector3 targetDirection = spellController.GetTargetDirection();
        player.MouseOnCast = targetDirection + ((targetDirection - transform.position).normalized * (spellData.dashMagnitude + spellData.chargedHitboxLength));
        StartCoroutine(SecondSpellDash(targetDirection));
    }

    /*
    *   SecondSpellDash - Handle moving and hitbox checking of the second cast.
    *   @param targetDirection - Vector3 of the target direction of the spell cast.
    */
    private IEnumerator SecondSpellDash(Vector3 targetDirection){
        isFirstCast = false;
        playerMover.CurrentTarget = targetDirection;
        // Get the final position after dash is applied.
        Vector3 targetPosition = (targetDirection - transform.position).normalized;
        GameObject visual = CreateSecondCastVisual(targetPosition);
        targetPosition = transform.position + (targetPosition * spellData.dashMagnitude);
        targetPosition = GetFinalPosition(targetPosition);
        float timer = 0.0f;
        Vector3 startingPosition = transform.position;
        float dashSpeed = spellData.dashMagnitude/spellData.dashTime;
        // While still dashing.
        while(timer < spellData.dashTime && !player.IsDead){
            SecondSpellTick(startingPosition, targetPosition, targetDirection, dashSpeed);
            timer += Time.deltaTime;
            yield return null;
        }
        // Apply last tick dash and enable pathing.
        if(!player.IsDead)
            SecondSpellTick(startingPosition, targetPosition, targetDirection, dashSpeed);
        //navMeshAgent.isStopped = false;
        PutOnCd(true);
        Destroy(visual);
    }

    /*
    *   SecondSpellTick - Handles checking the second spells hitbox and moving the player.
    *   @param startingPosition - Vector3 of the players starting position.
    *   @param targetPosition - Vector3 of the players target position.
    *   @param targetDirection - Vector3 of the players direction of movement.
    *   @param speed - The speed of the dash
    */
    private void SecondSpellTick(Vector3 startingPosition, Vector3 targetPosition, Vector3 targetDirection, float speed){
        // Move towards target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
        // Hitbox starts at center of player.
        Vector3 position = transform.position + ((targetDirection - transform.position).normalized * (spellData.chargedHitboxLength/2));
        CheckForSpellHits(position, spellData.chargedHitboxWidth, spellData.chargedHitboxLength);
    }

    /*
    *   CreateSecondCastVisual - Creates the second casts visual.
    *   @param direction - Vector3 of the direction the cast is in.
    *   @return GameObject - The created visual effect.
    */
    private GameObject CreateSecondCastVisual(Vector3 direction){
        Transform visual = ((GameObject) Instantiate(spellData.secondCastVisual, transform.position, transform.rotation)).transform;
        visual.SetParent(transform);
        visual.localScale = new Vector3(spellData.chargedHitboxWidth, 0.01f, spellData.chargedHitboxLength);
        visual.position = transform.position + (direction * (spellData.chargedHitboxLength/2));
        return visual.gameObject;
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
    */
    private void CheckForSpellHits(Vector3 position, float xSize, float zSize){
        position.y = player.hitbox.transform.position.y;
        List<Collider> hits = new List<Collider>(Physics.OverlapBox(position, new Vector3(xSize/2f, 0.5f, zSize/2f), transform.rotation, hitboxMask));
        foreach(Collider collider in hits){
            IUnit hitUnit = collider.gameObject.GetComponentInParent<IUnit>();
            if(hitUnit != player && hitUnit != null){
                if(!pastHits.Contains(hitUnit)){
                    Hit(hitUnit);
                    if(!isFirstCast)
                        pastHits.Add(hitUnit);
                }
            }
        }
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
        pastHits.Clear();
        OnCd = true;
        StartCoroutine(spellController.Spell_Cd_Timer(cd));
    }

    /*
    *   Hit - Deals third spells damage to the enemy hit.
    *   @param unit - IUnit of the enemy hit.
    */
    public void Hit(IUnit hit){
        spellHitCallback?.Invoke(hit, this);
        if(hit is IDamageable){
            float dmg = TotalDamage();
            ((IDamageable) hit).TakeDamage(dmg, DamageType.Magic, player, false);   
        }
    }

    /*
    *   TotalDamage - Calculate the pre-mitigation damage to deal.
    *   @return float - pre-mitigation damage damage to deal.
    */
    private float TotalDamage(){
        float damage = spellData.baseDamage[SpellLevel];
        if(isFirstCast){
            damage += 0.4f * player.unitStats.physicalDamage.GetValue();
            //Charge damage. 10% per 1/10 of max charge duration.
            damage += damage * (0.1f * (Mathf.Floor(Mathf.Clamp(chargeAmount, 0f, spellData.maxChargeDuration)/(spellData.maxChargeDuration/10f))));
        }
        else
            damage += 1f * player.unitStats.physicalDamage.GetValue();
        return damage;
    }
}
