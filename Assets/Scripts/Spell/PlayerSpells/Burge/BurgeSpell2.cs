using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements Burge's second spell. Burge throws a spinning dagger at the target location that grows in size over the spells duration.
* The spell deals damage to any Unit touching the dagger every x seconds then explodes, units hit by the explosion take double damage and are knocked up.
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
    *   Cast - Casts the spell.
    */
    public void Cast(){
        // Start cast time then cast the spell.
        StartCoroutine(spellController.CastTime());
        // Get the players mouse position on spell cast for spells target direction.
        Vector3 targetDirection = spellController.GetTargetDirection();
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

    /*
    *   Spell_2_Cast - Handles animating the spell and calling for hitbox checks.
    *   @param targetPosition - Vector3 of the spells cast position.
    */
    public IEnumerator Spell_2_Cast(Vector3 targetPosition){
        while(player.IsCasting)
            yield return null;
        Transform spellTransform = ((GameObject) Instantiate(spellData.prefab, targetPosition, Quaternion.identity)).transform;
        Vector3 endSize = spellTransform.localScale * spellData.sizeMultiplier;
        Vector3 startingSize = spellTransform.localScale;
        float nextTick = 0.0f;
        float timer = 0.0f;
        float timeBetweenTicks = spellData.duration/(spellData.numberOfHits - 1);
        while(timer < spellData.duration){
            if(timer > nextTick){
                // Check for hit
                CheckForSpellHits(spellTransform);
                nextTick += timeBetweenTicks;
            }
            spellTransform.localScale = Vector3.Lerp(startingSize, endSize, timer/spellData.duration);
            timer += Time.deltaTime;
            yield return null;
        }
        // Last hit
        spellTransform.localScale = Vector3.Lerp(startingSize, endSize, 1);
        CheckForSpellHits(spellTransform);
        //TODO: knock up and double damage last hit
        Destroy(spellTransform.gameObject);
    }

    /*
    *   CheckForSpellHits - Checks if there are any hits at the current players position using the spells hitbox.
    */
    private void CheckForSpellHits(Transform spellTransform){
        Vector3 overlapCheck = spellTransform.position;
        overlapCheck.y = player.hitbox.transform.position.y;
        List<Collider> outerHit = new List<Collider>(Physics.OverlapSphere(overlapCheck, spellTransform.localScale.x/2f));
        foreach(Collider collider in outerHit){
            if(collider.transform.name == "Hitbox" && collider.transform.parent != transform){
                IUnit hitUnit = collider.gameObject.GetComponentInParent<IUnit>();
                if(hitUnit != null){
                    Hit(hitUnit);
                }
            }
        }
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
