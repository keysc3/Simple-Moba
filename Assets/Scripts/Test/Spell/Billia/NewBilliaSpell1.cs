using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NewBilliaSpell1 : InterSpell, IHasHit, IHasCast, INewHasCallback
{
    new private BilliaSpell1Data spellData;
    private List<Effect> passiveEffectTracker = new List<Effect>();
    private int passiveStacks;
    private string radius;
    private List<ISpell> passiveStackSpells = new List<ISpell>();
    public List<ISpell> callbackSet { get; } = new List<ISpell>();
    public SpellHitCallback spellHitCallback { get; set; }


    protected override void Start(){
        base.Start();
        this.spellData = (BilliaSpell1Data) base.spellData;
        if(SpellNum == null){
            SpellNum = spellData.defaultSpellNum;
        }
        CanMove = true;
        IsQuickCast = true;
        //this.spellData = (BilliaSpell1Data) spellData;
    }

    void OnDisable(){
        foreach(ISpell spell in callbackSet){
            ((IHasHit) spell).spellHitCallback -= Spell_1_PassiveProc;
        }
    }

    void LateUpdate(){
        RemoveSpell_1_PassiveStack();
        ClearPassiveStackSpells();
    }
    
    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.up, spellData.outerRadius, 1f);
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, Vector3.up, spellData.innerRadius, 1f);
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        // If the spell is off cd, Billia is not casting, and has enough mana.
        if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            // Start cast time then cast the spell.
            StartCoroutine(sc.CastTime(spellData.castTime));
            StartCoroutine(Spell_1_Cast(Spell_1_Visual()));
            // Use mana.
            championStats.UseMana(spellData.baseMana[SpellLevel]);
            OnCd = true;
        }        
    }

    /*
    *   Spell_1_Cast - Casts Billia's first spell.
    */
    private IEnumerator Spell_1_Cast(GameObject visualHitbox){
        // Animate the beginning of the spell.
        StartCoroutine(Spell_1_Animation(visualHitbox, spellData.initialAlpha, spellData.finalAlpha));
        while(player.IsCasting){
            yield return null;
        }
        StartCoroutine(sc.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
        // Hitbox starts from center of Billia.
        HitboxCheck();
        // Animate the ending of the spell.
        StartCoroutine(Spell_1_Animation(visualHitbox, spellData.finalAlpha, spellData.initialAlpha));
    }

    /*
    *   HitboxCheck - Checks an outer radius for any collider hits then checks if those hits are part of the inner radius damage.
    *   @param hitboxCenter - Vector3 of the position of the center of the radius' hitbox.
    */
    private void HitboxCheck(){
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        List<Collider> outerHit = new List<Collider>(Physics.OverlapSphere(transform.position, spellData.outerRadius, enemyMask));
        foreach(Collider collider in outerHit){
            // Check if the center of the hit collider is within the spell hitbox.
            Vector3 colliderHitCenter = collider.bounds.center;
            float distToHitboxCenter = (colliderHitCenter - transform.position).magnitude;
            if(distToHitboxCenter < spellData.outerRadius){
                // Check if the unit was hit by the specified spells inner damage.
                if(distToHitboxCenter < spellData.innerRadius){
                    radius = "inner";
                    Hit(collider.gameObject.GetComponent<IUnit>());
                }
                // Unit hit by outer portion.
                else{
                    radius = "outer";
                    Hit(collider.gameObject.GetComponent<IUnit>());
                }
            }
        }
    }

    /*
    *   Spell_1_Animation - Animates the wind up or wind down of Billia's first spell.
    *   @param visualHitbox - GameObject of the hitbox visual.
    *   @param initialAlpha - float of the starting alpha.
    *   @param finalAlpha - float of the final alpha to reach.
    */
    private IEnumerator Spell_1_Animation(GameObject visualHitbox, float initialAlpha, float finalAlpha){
        // Get the outer radius's renderer and color.
        Renderer outerRenderer = visualHitbox.transform.GetChild(1).gameObject.GetComponent<Renderer>();
        Color newColor = outerRenderer.material.color;
        // Set up.
        float startTime = Time.time;
        float timer = 0.0f;
        // Animation time is spell cast time.
        while(timer < spellData.castTime){
            // Animate the spell cast.
            float step = (Time.time - startTime)/spellData.castTime;
            newColor.a = Mathf.Lerp(initialAlpha, finalAlpha, step)/255f;
            outerRenderer.material.color = newColor;
            timer += Time.deltaTime;
            yield return null;
        }
        // Last tick.
        newColor.a = finalAlpha/255f;
        // Destroy the hitbox visual is this was the ending animation.
        if(finalAlpha < initialAlpha)
            Destroy(visualHitbox);
    }

    /*
    *   Spell_1_PassiveProc - Handles spell 1's passive being activated or refreshed.
    */
    private void Spell_1_PassiveProc(IUnit unit, ISpell spellHit){
        // If a passive stack was received this frame, don't add another.
        if(passiveStackSpells.Contains(spellHit))
            return;
        passiveStackSpells.Add(spellHit);
        if(SpellLevel >= 0 && passiveStacks < spellData.passiveMaxStacks){
            // Create a new speed bonus with the 
            float bonusPercent = spellData.passiveSpeed[SpellLevel];
            SpeedBonus speedBonus = (SpeedBonus) spellData.passiveSpeedBonus.InitializeEffect(SpellLevel, gameObject, gameObject);
            speedBonus.BonusPercent = bonusPercent;
            player.statusEffects.AddEffect(speedBonus);
            passiveEffectTracker.Add(speedBonus);
            passiveStacks += 1;
        }
        if(passiveStacks > 1){
            ResetSpell_1_PassiveTimers();
        }
    }

    /*
    *   ResetSpell_1_PassiveTimers - Changes the duration of Billia's passive stacks. 
    *   This is for when a passive proc happens and the timers need to be updated.
    */
    private void ResetSpell_1_PassiveTimers(){
        int multiplier = passiveStacks - 1;
        // If at max stacks then reset the newest stack as well.
        if(passiveStacks == spellData.passiveMaxStacks){
            int last = passiveEffectTracker.Count - 1;
            ChangeSpell_1_PassiveEffect(last, 0f);
        }
        // Reset each passive stacks timer to the base plus an increase based on where it is in the stack list.
        for(int i = 0; i < passiveEffectTracker.Count - 1; i++){
            ChangeSpell_1_PassiveEffect(i, spellData.passiveExpireDuration * multiplier);
            multiplier -= 1;
        }
    }

    /*
    *   ChangeSpell_1_PassiveEffect - Resets the timer and changes the duration of a spell 1 passive effect.
    *   @param index - int of the stacks index being changed.
    *   @param baseIncrease - float of the increase from the base duration for the stack.
    */
    private void ChangeSpell_1_PassiveEffect(int index, float baseIncrease){
        passiveEffectTracker[index].ResetTimer();
        float newDuration = spellData.passiveSpeedDuration + baseIncrease;
        passiveEffectTracker[index].EffectDuration = newDuration;
    }

    /*
    *   RemoveSpell_1_PassiveStack - Removes any stacks from the passive list if they have finished.
    */
    private void RemoveSpell_1_PassiveStack(){
        for(int i = passiveEffectTracker.Count - 1; i >=0; i--){
            if(passiveEffectTracker[i].isFinished){
                passiveEffectTracker.RemoveAt(i);
                passiveStacks -= 1;
            }
        }
    }

    /*
    * ClearPassiveStackSpells - Clears the list of spells that granted a passive stack the current frame.
    * Used to make sure only one stack per ability is granted per frame.
    */
    private void ClearPassiveStackSpells(){
        passiveStackSpells.Clear();
    }

    /*
    *   Spell_1_Visual - Visual hitbox indicator for Billia's first spell.
    *   @return GameObject - Created visual hitbox GameObject.
    */
    private GameObject Spell_1_Visual(){
        // Create the spells visual hitbox and set necessary values.
        GameObject visualHitbox = (GameObject) Instantiate(spellData.visualPrefab, transform.position, Quaternion.identity);
        visualHitbox.name = "BilliaSpell_1";
        visualHitbox.transform.SetParent(transform);
        float yScale = visualHitbox.transform.GetChild(0).localScale.y;
        visualHitbox.transform.GetChild(0).localScale = new Vector3(spellData.innerRadius * 2f, yScale, spellData.innerRadius * 2f);
        visualHitbox.transform.GetChild(1).localScale = new Vector3(spellData.outerRadius * 2f, yScale, spellData.outerRadius * 2f);
        return visualHitbox;
    }

    /*
    *   Hit - Deals first spells damage to the enemy hit. Magic damage with additional true damage on outer hit.
    *   @param enemy - GameObject of the enemy hit.
    *   @param radius - string of which radius was hit.
    */
    public void Hit(IUnit unit){
        Debug.Log("HIT");
        spellHitCallback?.Invoke(unit, this);
        if(unit is INewDamagable){
            INewDamagable damageMethod = (INewDamagable) unit;
            Spell_1_PassiveProc(unit, this);
            float magicDamage = championStats.magicDamage.GetValue();
            if(radius == "inner")
                damageMethod.TakeDamage(spellData.baseDamage[SpellLevel] + magicDamage, "magic", player, false);   
            else{
                damageMethod.TakeDamage(spellData.baseDamage[SpellLevel] + magicDamage, "magic", player, false);
                damageMethod.TakeDamage(spellData.baseDamage[SpellLevel] + magicDamage, "true", player, false);
            }
        }
    }

    /*
    *   SetupCallbacks - Sets up the necessary callbacks for the spell.
    *   @param mySpells - List of Spells to set callbacks.
    */
    public void SetupCallbacks(Dictionary<string, ISpell> spells){
        // If the Spell is a DamageSpell then add this spells passive proc to its spell hit callback.
        foreach(KeyValuePair<string, ISpell> entry in spells){
            if(entry.Value is IHasHit && !(entry.Value is NewBilliaSpell1)){
                ((IHasHit) entry.Value).spellHitCallback += Spell_1_PassiveProc;
                callbackSet.Add(entry.Value);
            }
        }
    }
}
