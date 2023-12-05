using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void Cast(){
        float minFillToCast = (spellData.minDuration/spellData.maxDuration) * 100f;
        if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel] && currentFill >= minFillToCast){
            StartCoroutine(spellController.CastTime());
            StartCoroutine(SpellDuration(CalculateDuration()));
            // Use mana.
            championStats.UseMana(spellData.baseMana[SpellLevel]);
            OnCd = true;
        }      
    }

    private float CalculateDuration(){
        float duration = currentFill/100f;
        return Mathf.Clamp(duration * spellData.maxDuration, spellData.minDuration, spellData.maxDuration);
    }

    private IEnumerator SpellDuration(float duration){
        while(player.IsCasting)
            yield return null;
        float timer = 0f;
        casted = true;
        while(timer < duration && casted){
            if(Input.GetKeyDown(KeyCode.R) && !player.IsCasting){
                SecondCast();
                casted = false;
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }

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

    public void BasicSpellHit(IUnit hitUnit, ISpell spellHit){
        if(!casted){
            if(currentFill < 100f)
                currentFill = Mathf.Clamp(spellData.spellFill + currentFill, 0f, 100f);
        }
        else{
            castedHits += 1;
        }
    }

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
