using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
* Purpose: Implements Burge's second spell. Burge throws a spinning dagger at the target location that grows in size over the spells duration.
* The spell deals damage to any Unit touching the dagger every x seconds then explodes, units hit by the explosion take damage and are knocked up.
*
* @author: Colin Keys
*/
public class BurgeSpell2 : Spell, IHasCast, IHasHit
{
    new private BurgeSpell2Data spellData;
    public SpellHitCallback spellHitCallback { get; set; }

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BurgeSpell2Data) base.spellData;
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.up, spellData.magnitude, 1f);
       // Get the players mouse position on spell cast for spells target direction.
        Vector3 targetDirection = spellController.GetTargetDirection();
        // Set the target position to be in the direction of the mouse on cast.
        Vector3 targetPosition = (targetDirection - transform.position);
        // Set target to lob seed to max lob distance if casted at a greater distance.
        if(targetPosition.magnitude > spellData.magnitude)
            targetPosition = transform.position + (targetPosition.normalized * spellData.magnitude);
        else
            targetPosition = transform.position + (targetDirection - transform.position);
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(targetPosition, Vector3.up, (spellData.startingSize/2f) * spellData.sizeMultiplier, 1f);
        Gizmos.color = Color.cyan;
        Handles.DrawWireDisc(targetPosition, Vector3.up, spellData.startingSize/2f, 1f);
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            // Start cast time then cast the spell.
            StartCoroutine(spellController.CastTime(spellData.castTime, spellData.name));
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = spellController.GetTargetDirection();
            player.MouseOnCast = targetDirection;
            // Set the target position to be in the direction of the mouse on cast.
            Vector3 targetPosition = (targetDirection - transform.position);
            // Set target position to max distance if casted at a greater distance.
            if(targetPosition.magnitude > spellData.magnitude)
                targetPosition = transform.position + (targetPosition.normalized * spellData.magnitude);
            else
                targetPosition = transform.position + (targetDirection - transform.position);
            StartCoroutine(Spell_2_Cast(targetPosition));
            // Use mana.
            championStats.UseMana(spellData.baseMana[SpellLevel]);
            OnCd = true;
            StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
        }
    }

    /*
    *   Spell_2_Cast - Handles animating the spell and calling for hitbox checks.
    *   @param targetPosition - Vector3 of the spells cast position.
    */
    public IEnumerator Spell_2_Cast(Vector3 targetPosition){
        while(player.IsCasting)
            yield return null;
        Dictionary<IUnit, float> lastHits = new Dictionary<IUnit, float>();
        Transform spellTransform = ((GameObject) Instantiate(spellData.prefab, targetPosition, Quaternion.identity)).transform;
        spellTransform.position = new Vector3(targetPosition.x, 0.5f, targetPosition.z);
        float endSize = spellData.startingSize * spellData.sizeMultiplier;
        float timer = 0.0f;
        while(timer < spellData.duration){
            // Check for hit
            lastHits = CheckForSpellHits(spellTransform, false, lastHits);
            SetScale(endSize, spellTransform, timer);
            timer += Time.deltaTime;
            yield return null;
        }
        // Last hit
        SetScale(endSize, spellTransform, spellData.duration);
        CheckForSpellHits(spellTransform, true, lastHits);
        Destroy(spellTransform.gameObject);
    }

    /*
        SetScale - Sets the scale values of the hitbox GameObject.
        @param endSize - float of th final scale.
        @param spellTransform - The transform of the hitbox.
        @param timer - float of the time the spell has been active.
    */
    private void SetScale(float endSize, Transform spellTransform, float timer){
        float yScale = spellTransform.localScale.y;
        float size = Mathf.Lerp(spellData.startingSize, endSize, timer/spellData.duration);
        spellTransform.localScale = new Vector3(size, yScale, size);
    }

    /*
    *   CheckForSpellHits - Checks if there are any hits using the spells hitbox.
    */
    private Dictionary<IUnit, float> CheckForSpellHits(Transform spellTransform, bool spellFinished, Dictionary<IUnit, float> lastHits){
        Vector3 overlapCheck = spellTransform.position;
        overlapCheck.y = player.hitbox.transform.position.y;
        List<Collider> outerHit = new List<Collider>(Physics.OverlapSphere(overlapCheck, spellTransform.localScale.x/2f, hitboxMask));
        foreach(Collider collider in outerHit){
            IUnit hitUnit = collider.gameObject.GetComponentInParent<IUnit>();
            if(hitUnit != player && hitUnit != null){
                if(!spellFinished){
                    float lastHitTime;
                    // If unit has been hit once already and its not last hit, check if time for another hit, otherwise hit and add to dictionary.
                    if(lastHits.TryGetValue(hitUnit, out lastHitTime)){
                        if(Time.time >= (lastHitTime + spellData.timeBetweenTicks)){
                            Hit(hitUnit);
                            lastHits[hitUnit] = Time.time;
                        }
                    }
                    else{
                        lastHits.Add(hitUnit, Time.time);
                        Hit(hitUnit);
                    }
                }
                else{
                    Hit(hitUnit);
                    Hit(hitUnit);
                    hitUnit.statusEffects.AddEffect(spellData.knockup.InitializeEffect(SpellLevel, player, hitUnit));
                }
            }
        }
        return lastHits;
    }

    /*
    *   Hit - Deals second spells damage to the enemy hit.
    *   @param unit - IUnit of the enemy hit.
    */
    public void Hit(IUnit unit){
        spellHitCallback?.Invoke(unit, this);
        if(unit is IDamageable){
            float damageValue = spellData.baseDamage[SpellLevel];// + (0.2f * championStats.physicalDamage.GetValue());
            ((IDamageable) unit).TakeDamage(damageValue, DamageType.Physical, player, false);
        }
    }
}
