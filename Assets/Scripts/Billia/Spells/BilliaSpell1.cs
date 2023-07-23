using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaSpell1 : DamageSpell, IHasCallback
{

    private BilliaSpell1Data spell1Data;
    private List<Effect> passiveEffectTracker = new List<Effect>();
    private int passiveStacks;
    private string radius;

    public BilliaSpell1(ChampionSpells championSpells, SpellData spell1Data) : base(championSpells){
        this.spell1Data = (BilliaSpell1Data) spell1Data;
        championSpells.lateUpdateCallback += RemoveSpell_1_PassiveStack;
    }

    /*
    *   Spell_1 - Sets up Billia's first spell. She swirls her weapon in a radius around her. Players hit by the outer portion take bonus damage.
    *   Passive: Gain a stacking speed bonus whenever a unit is hit with any spell, up to 4 stacks.
    */
    public override void Cast(){
        // If the spell is off cd, Billia is not casting, and has enough mana.
        if(!onCd && !player.isCasting && championStats.currentMana >= spell1Data.baseMana[levelManager.spellLevels["Spell_1"]-1]){
            // Start cast time then cast the spell.
            championSpells.StartCoroutine(CastTime(spell1Data.castTime, true));
            championSpells.StartCoroutine(Spell_1_Cast(Spell_1_Visual()));
            // Use mana.
            championStats.UseMana(spell1Data.baseMana[levelManager.spellLevels["Spell_1"]-1]);
            onCd = true;
        }        
    }

    /*
    *   Spell_1_Cast - Casts Billia's first spell.
    */
    private IEnumerator Spell_1_Cast(GameObject visualHitbox){
        // Animate the beginning of the spell.
        championSpells.StartCoroutine(Spell_1_Animation(visualHitbox, spell1Data.initialAlpha, spell1Data.finalAlpha));
        while(player.isCasting){
            yield return null;
        }
        championSpells.StartCoroutine(Spell_Cd_Timer(spell1Data.baseCd[levelManager.spellLevels["Spell_1"]-1], (myBool => onCd = myBool), "Spell_1"));
        // Hitbox starts from center of Billia.
        HitboxCheck();
        // Animate the ending of the spell.
        championSpells.StartCoroutine(Spell_1_Animation(visualHitbox, spell1Data.finalAlpha, spell1Data.initialAlpha));
    }

    private void HitboxCheck(){
        bool passiveStack = false;
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        List<Collider> outerHit = new List<Collider>(Physics.OverlapSphere(gameObject.transform.position, spell1Data.outerRadius, enemyMask));
        foreach(Collider collider in outerHit){
            // Check if the center of the hit collider is within the spell hitbox.
            Vector3 colliderHitCenter = collider.bounds.center;
            float distToHitboxCenter = (colliderHitCenter - gameObject.transform.position).magnitude;
            if(distToHitboxCenter < spell1Data.outerRadius){
                // Check if the unit was hit by the specified spells inner damage.
                if(distToHitboxCenter < spell1Data.innerRadius){
                    radius = "inner";
                    Hit(collider.gameObject);
                }
                // Unit hit by outer portion.
                else{
                    radius = "outer";
                    Hit(collider.gameObject);
                }
                passiveStack = true;
            }
        }
        // If a unit was hit proc the spells passive.
        if(passiveStack)
            Spell_1_PassiveProc(gameObject);
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
        while(timer < spell1Data.castTime){
            // Animate the spell cast.
            float step = (Time.time - startTime)/spell1Data.castTime;
            newColor.a = Mathf.Lerp(initialAlpha, finalAlpha, step)/255f;
            outerRenderer.material.color = newColor;
            timer += Time.deltaTime;
            yield return null;
        }
        // Last tick.
        newColor.a = finalAlpha/255f;
        // Destroy the hitbox visual is this was the ending animation.
        if(finalAlpha < initialAlpha)
            Object.Destroy(visualHitbox);
    }

    /*
    *   Spell_1_PassiveProc - Handles spell 1's passive being activated or refreshed.
    */
    private void Spell_1_PassiveProc(GameObject hit){
        //spell_1_lastStackTime = Time.time;
        if(levelManager.spellLevels["Spell_1"] > 0 && passiveStacks < spell1Data.passiveMaxStacks){
            // Create a new speed bonus with the 
            float bonusPercent = spell1Data.passiveSpeed[levelManager.spellLevels["Spell_1"]-1];
            SpeedBonus speedBonus = (SpeedBonus) spell1Data.passiveSpeedBonus.InitializeEffect(levelManager.spellLevels["Spell_1"]-1, gameObject, gameObject);
            speedBonus.SetBonusPercent(bonusPercent);
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
        if(passiveStacks == spell1Data.passiveMaxStacks){
            int last = passiveEffectTracker.Count - 1;
            ChangeSpell_1_PassiveEffect(last, 0f);
        }
        // Reset each passive stacks timer to the base plus an increase based on where it is in the stack list.
        for(int i = 0; i < passiveEffectTracker.Count - 1; i++){
            ChangeSpell_1_PassiveEffect(i, spell1Data.passiveExpireDuration * multiplier);
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
        float newDuration = spell1Data.passiveSpeedDuration + baseIncrease;
        passiveEffectTracker[index].SetDuration(newDuration);
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
    *   Spell_1_Visual - Visual hitbox indicator for Billia's first spell.
    *   @return GameObject - Created visual hitbox GameObject.
    */
    private GameObject Spell_1_Visual(){
        // Create the spells visual hitbox and set necessary values.
        GameObject visualHitbox = (GameObject) Object.Instantiate(spell1Data.visualPrefab, gameObject.transform.position, Quaternion.identity);
        visualHitbox.name = "BilliaSpell_1";
        visualHitbox.transform.SetParent(gameObject.transform);
        float yScale = visualHitbox.transform.GetChild(0).localScale.y;
        visualHitbox.transform.GetChild(0).localScale = new Vector3(spell1Data.innerRadius * 2f, yScale, spell1Data.innerRadius * 2f);
        visualHitbox.transform.GetChild(1).localScale = new Vector3(spell1Data.outerRadius * 2f, yScale, spell1Data.outerRadius * 2f);
        return visualHitbox;
    }

    /*
    *   Spell_1_Hit - Deals first spells damage to the enemy hit. Magic damage with additional true damage on outer hit.
    *   @param enemy - GameObject of the enemy hit.
    *   @param radius - string of which radius was hit.
    */
    public override void Hit(GameObject hit){
        spellHitCallback?.Invoke(hit);
        float magicDamage = championStats.magicDamage.GetValue();
        Unit enemyUnit = hit.GetComponent<Unit>();
        if(radius == "inner")
            enemyUnit.TakeDamage(spell1Data.baseDamage[levelManager.spellLevels["Spell_1"]-1] + magicDamage, "magic", gameObject, false);   
        else{
            enemyUnit.TakeDamage(spell1Data.baseDamage[levelManager.spellLevels["Spell_1"]-1] + magicDamage, "magic", gameObject, false);
            enemyUnit.TakeDamage(spell1Data.baseDamage[levelManager.spellLevels["Spell_1"]-1] + magicDamage, "true", gameObject, false);
        }
    }

    public void SetupCallbacks(List<Spell> mySpells){
        foreach(Spell newSpell in mySpells){
            if(newSpell is DamageSpell && !(newSpell is BilliaSpell1)){
                ((DamageSpell) newSpell).spellHitCallback += Spell_1_PassiveProc;
            }
        }
    }
}