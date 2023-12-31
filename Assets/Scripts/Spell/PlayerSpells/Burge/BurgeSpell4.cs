using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements Burge's fourth spell. Burge enters a trance and receives increased ability haste for the duration. The ability can be recast before the
* duration expires to deal damage in a line. Hitting basic abilities before increases the duration of the spell when cast, with a minimum amount to cast.
* The abilities damage varies based on how many spell hits happened during the spell cast, the more spells hit the more damage the recast will do.
*
* @author: Colin Keys
*/
public class BurgeSpell4 : Spell, IHasHit, IHasCast, IHasCallback
{
    public List<ISpell> callbackSet { get; } = new List<ISpell>();
    public SpellHitCallback spellHitCallback { get; set; }

    new private BurgeSpell4Data spellData;
    private float currentFill = 0f;
    private bool casted = false;
    private int castedHits = 0;

    // Start is called before the first frame update
    protected override void Start(){
        base.Start();
        this.spellData = (BurgeSpell4Data) base.spellData;
        IsQuickCast = true;
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        float minFillToCast = (spellData.minDuration/spellData.maxDuration) * 100f;
        if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel] && currentFill >= minFillToCast){
            casted = true;
            StartCoroutine(spellController.CastTime());
            StartCoroutine(SpellDuration(CalculateDuration()));
            // Use mana.
            championStats.UseMana(spellData.baseMana[SpellLevel]);
            OnCd = true;
        }      
    }

    /*
    *   CalculateDuration - Calculates how long the spell will last based on how much energy is stored.
    *   @return float - Duration the spell will last.
    */
    private float CalculateDuration(){
        float duration = currentFill/100f;
        return Mathf.Clamp(duration * spellData.maxDuration, spellData.minDuration, spellData.maxDuration);
    }

    /*
    *   SpellDuration - Handles the spells lifecycle.
    *   @param duration - float of the spells duration.
    */
    private IEnumerator SpellDuration(float duration){
        while(player.IsCasting)
            yield return null;
        float timer = 0f;
        while(timer < duration && casted){
            if(Input.GetKeyDown(KeyCode.R) && !player.IsCasting){
                SecondCast();
                casted = false;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
    }

    /*
    *   SecondCast - Handles the recast of the ability which ends it and deals damage.
    */
    private void SecondCast(){
        Vector3 targetDirection = spellController.GetTargetDirection();
        player.MouseOnCast = targetDirection;
        Vector3 position = transform.position + ((targetDirection - transform.position).normalized * (spellData.length/2));
        position.y = player.hitbox.transform.position.y;
        List<Collider> hits = new List<Collider>(Physics.OverlapBox(position, new Vector3(spellData.width, 0.5f, spellData.length), transform.rotation));
        CheckForSpellHits(hits);
    }

    /*
    *   CheckForSpellHits - Checks for any spell hits from a list of hit colliders.
    *   @param hits - List of colliders to check.
    */
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

    /*
    *   BasicSpellHit - Incrememnts necessary fields when the player lands a basic spell.
    *   @param hitUnit - IUnit of the enemy hit.
    *   @param spellHit - ISpell the hit is from.
    */
    public void BasicSpellHit(IUnit hitUnit, ISpell spellHit){
        if(!casted){
            if(currentFill < 100f)
                currentFill = Mathf.Clamp(spellData.spellFill + currentFill, 0f, 100f);
        }
        else{
            castedHits += 1;
        }
    }

    /*
    *   Hit - Deals fourth spells damage to the enemy hit.
    *   @param unit - IUnit of the enemy hit.
    */
    public void Hit(IUnit hit){
        Debug.Log("Imagine getting hit :skull:");
    }

    /*
    *   SetupCallbacks - Sets up the necessary callbacks for the spell.
    *   @param spells - Dictionary of the current spells.
    */
    public void SetupCallbacks(Dictionary<SpellType, ISpell> spells){
        // If the Spell is a DamageSpell then add this spells passive proc to its spell hit callback.
        foreach(KeyValuePair<SpellType, ISpell> entry in spells){
            if(entry.Value is IHasHit && !(entry.Value is BilliaSpell1)){
                ((IHasHit) entry.Value).spellHitCallback += BasicSpellHit;
                callbackSet.Add(entry.Value);
            }
        }
    }
}
