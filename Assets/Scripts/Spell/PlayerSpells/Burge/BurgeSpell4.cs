using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private PersonalSpell spellEffect;
    private bool canCast = false;
    private float minFillToCast;
    private Image fillImage;
    IPlayerMover playerMover;

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Vector3 targetPosition = (spellController.GetTargetDirection() - transform.position).normalized;
        targetPosition = transform.position + (targetPosition * spellData.length);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, targetPosition);
    }

    // Start is called before the first frame update
    protected override void Start(){
        base.Start();
        this.spellData = (BurgeSpell4Data) base.spellData;
        IsQuickCast = true;
        minFillToCast = (spellData.minDuration/spellData.maxDuration) * 100f;
        Transform uiComp = transform.Find(transform.name + "UI" + "/PlayerUI/Player/Combat/SpellsContainer/" + SpellNum + "_Container/SpellContainer/Spell/Fill/Amount");
        if(uiComp != null){
            fillImage = uiComp.GetComponent<Image>();
            fillImage.fillAmount = 0f;
        }
        playerMover = GetComponentInParent<IPlayerMover>();
    }

    // Called after all Update functions have been called
    private void LateUpdate(){
        CanUseSpell();
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        if(!player.IsCasting){
            if(!casted){
                if(championStats.CurrentMana >= spellData.baseMana[SpellLevel] && canCast){
                    casted = true;
                    canCast = false;
                    player.MouseOnCast = transform.position + transform.forward;
                    StartCoroutine(spellController.CastTime(spellData.castTime, spellData.name));
                    StartCoroutine(SpellDuration(CalculateDuration()));
                    // Use mana.
                    championStats.UseMana(spellData.baseMana[SpellLevel]);
                    IsQuickCast = false;
                }
            }
            else{
                Recast();
            }
        }   
    }

    /*
        Recast - Handles the recast actions of the spell.
    */
    private void Recast(){
        StartCoroutine(spellController.CastTime(spellData.castTime, spellData.name));
        SecondCastTarget();
        SpellFinished();
    }
    
    /*
        SpellFinished - Handles settings fields on finishing.
    */
    private void SpellFinished(){
        casted = false;
        IsQuickCast = true;
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
        UpdateSpellSprite();
        if(fillImage != null)
                fillImage.fillAmount = 0f;
        spellData.spellEffect.duration[0] = duration;
        spellEffect = (PersonalSpell) spellData.spellEffect.InitializeEffect(0, player, player);
        player.statusEffects.AddEffect(spellEffect);
        float timer = 0f;
        RaiseSetComponentActiveEvent(SpellNum, SpellComponent.DurationSlider, true);
        while(timer < duration && casted){
            timer += Time.deltaTime;
            RaiseSpellSliderUpdateEvent(SpellNum, duration, timer);
            yield return null;
        }
        RaiseSetComponentActiveEvent(SpellNum, SpellComponent.DurationSlider, false);
        player.statusEffects.RemoveEffect(spellEffect.effectType, player);
        spellEffect = null;
        currentFill = 0f;
        if(casted)
            SpellFinished();
        UpdateSpellSprite();
        OnCd = true;
        StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
    }

    /*
    *   SecondCastTarget - Handles getting the target of the recast.
    */
    private void SecondCastTarget(){
        Vector3 targetDirection = spellController.GetTargetDirection();
        player.MouseOnCast = targetDirection;
        playerMover.CurrentTarget = targetDirection;
        Vector3 position = transform.position + ((targetDirection - transform.position).normalized * (spellData.length/2));
        //position.y = player.hitbox.transform.position.y;
        StartCoroutine(SecondCastHitbox(position));
    }

    /*
    *   SecondCastHitBox - Handles hit checking for the spells damage cast.
    *   @param position - Vector3 of the center of the hitbox.
    */
    private IEnumerator SecondCastHitbox(Vector3 position){
        GameObject visualHitbox = CreateCastVisual(position);
        while(player.IsCasting){
            yield return null;
        }
        CheckForSpellHits(position);
        StartCoroutine(spellController.Fade(visualHitbox, spellData.fadeTime));
    }

    /*
    *   CreateCastVisual - Creates the visual effect for the cast jab.
    *   @param position - Vector3 of the x and z center of the visual.
    *   @return GameObject - The created visual.
    */
    private GameObject CreateCastVisual(Vector3 position){
        Transform visualHitbox = ((GameObject) Instantiate(spellData.visualHitbox, position, transform.rotation)).transform;
        // Setup the spells visual.
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        visualHitbox.position = new Vector3(visualHitbox.position.x, visualHitbox.position.y - capsule.bounds.size.y/2f, visualHitbox.position.z);
        visualHitbox.localScale = new Vector3(spellData.width, visualHitbox.localScale.y, spellData.length);
        return visualHitbox.gameObject;
    }

    /*
    *   CheckForSpellHits - Checks for any spell hits from a list of hit colliders.
    */
    private void CheckForSpellHits(Vector3 position){
        position.y = player.hitbox.transform.position.y;
        List<Collider> hits = new List<Collider>(Physics.OverlapBox(position, new Vector3(spellData.width/2f, 0.5f, spellData.length/2f), transform.rotation, hitboxMask));
        foreach(Collider collider in hits){
            IUnit hitUnit = collider.gameObject.GetComponentInParent<IUnit>();
            if(hitUnit != player && hitUnit != null){
                Hit(hitUnit);
            }
        }
    }

    /*
    *   CanUseSpell - Checks if the spell has enough stacks to be used.
    */
    private void CanUseSpell(){
        if(SpellLevel >= 0 && !OnCd && !casted){
            if(currentFill >= minFillToCast){
                RaiseSetComponentActiveEvent(SpellNum, SpellComponent.CDCover, false);
                canCast = true;
            }
            else{
                canCast = false;
                RaiseSetComponentActiveEvent(SpellNum, SpellComponent.CDCover, true);
            }
        }
    }
    /*
    *   BasicSpellHit - Increments necessary fields when the player lands a basic spell.
    *   @param hitUnit - IUnit of the enemy hit.
    *   @param spellHit - ISpell the hit is from.
    */
    public void BasicSpellHit(IUnit hitUnit, ISpell spellHit){
        if(SpellLevel >= 0 && !OnCd){
            if(!casted){
                if(currentFill < 100f){
                    float toFill = spellData.fillPerSpellHit[spellHit.spellData.spellID];
                    currentFill = Mathf.Clamp(toFill + currentFill, 0f, 100f);
                    if(fillImage != null)
                        fillImage.fillAmount = Mathf.Clamp01(currentFill/100f);
                }
            }
            else{
                if(castedHits < spellData.maxCastedHits){
                    castedHits += 1;
                    if(spellEffect != null)
                        spellEffect.Stacks = castedHits;
                }
            }
        }
    }
    
    private void UpdateSpellSprite(){
        if(casted)
            RaiseSetSpriteEvent(SpellNum, SpellComponent.SpellImage, spellData.castedSprite);
        else
            RaiseSetSpriteEvent(SpellNum, SpellComponent.SpellImage, spellData.sprite);
    }

    /*
    *   Hit - Deals fourth spells damage to the enemy hit.
    *   @param unit - IUnit of the enemy hit.
    */
    public void Hit(IUnit hit){
        Debug.Log("Imagine getting hit :skull:");
        if(hit is IDamageable){
            ((IDamageable) hit).TakeDamage(TotalDamage(hit), DamageType.Magic, player, false);   
        }
    }

    /*
    *   TotalDamage - Calculate the pre-mitigation damage to deal.
    *   @return float - pre-mitigation damage damage to deal.
    */
    private float TotalDamage(IUnit unit){
        // Damage increased by number of casted hits.
        float damage = (spellData.baseDamage[SpellLevel] + (0.3f * player.unitStats.physicalDamage.GetValue()));
        damage += damage * (castedHits/spellData.maxCastedHits);
        if(unit is not IPlayer){
            damage *= 0.5f;
        }
        return damage;
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
