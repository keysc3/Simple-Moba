using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements Billia's fourth spell. Billia puts a Drowsy on any champion with her passive dot on them. After a set time of Drowsy the 
* champion gets the Sleep effect. If the champion is woken up from the sleep by non-dot champion damage, they take bonus damage.
*
* @author: Colin Keys
*/
public class BilliaSpell4 : Spell, IHasCast
{
    new private BilliaSpell4Data spellData;
    private bool canUseSpell = false;
    private GameObject spellCDCover;

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BilliaSpell4Data) base.spellData;
        if(SpellNum == null){
            SpellNum = spellData.defaultSpellNum;
        }
        CanMove = true;
        IsQuickCast = true;
        if(player.playerUI != null){
            spellCDCover = player.playerUI.transform.Find("Player/Combat/SpellsContainer/" + SpellNum + "_Container/SpellContainer/Spell/CD/Cover").gameObject;
        }
    }

    // Called after all Update functions have been called
    private void LateUpdate(){
        CanUseSpell();
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        // Only allow cast if a champion has passive on them.
        if(canUseSpell){
            if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
                // Start cast time then cast the spell.
                StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
                StartCoroutine(spellController.CastTime(spellData.castTime));
                StartCoroutine(Spell_4_Cast(GetChampionsWithPassive()));
                // Use mana.
                championStats.UseMana(spellData.baseMana[SpellLevel]);
                OnCd = true;
            }
        }
    }

    /*
    *   Spell_4_Cast - Casts and starts the cd timer for spell 4. Would have to be refactored to implement projectile destruction/blocking.
    *   @param List<GameObject> - List of GameObjects to apply the drowsy to.
    */
    private IEnumerator Spell_4_Cast(List<GameObject> applyDrowsy){
        while(player.IsCasting)
            yield return null;
        StartCoroutine(Spell_4_Projectile(applyDrowsy));
    }

    /*
    *   Spell_4_Projectile - Handle the travel time of spell 4.
    *   @param List<GameObject> - List of GameObjects to apply the drowsy to.
    */
    private IEnumerator Spell_4_Projectile(List<GameObject> applyDrowsy){
        float travelTime = spellData.travelTime;
        float startTime = Time.time;
        while(Time.time - startTime < travelTime){
            // Move projectile.
            yield return null;
        }
        // Apply drowsy debuff.
        Spell_4_Drowsy(applyDrowsy);
    }

    /*
    *   Spell_4_Drowsy - Applies the drowsy debuff from spell 4 to any champions applied with the passive dot.
    *   @param List<GameObject> - List of GameObjects to apply the drowsy to.
    */
    private void Spell_4_Drowsy(List<GameObject> applyDrowsy){
        foreach(GameObject enemy in applyDrowsy){
            IUnit unit = enemy.GetComponent<IUnit>();
            if(unit is IPlayer){
                // Add drowsy to enemy player and update the bonus damage delegate.
                Drowsy newDrowsy = (Drowsy) spellData.drowsy.InitializeEffect(SpellLevel, player, unit);
                unit.statusEffects.AddEffect(newDrowsy);
                unit.bonusDamage += Spell_4_SleepProc;
                // Animate the drowsy effect.
                GameObject drowsyObject = (GameObject) Instantiate(spellData.drowsyVisual, enemy.transform.position, Quaternion.identity);
                drowsyObject.transform.SetParent(enemy.transform);
                BilliaDrowsyVisual visualScript = drowsyObject.GetComponent<BilliaDrowsyVisual>();
                visualScript.drowsyDuration = newDrowsy.EffectDuration;
                visualScript.drowsy = spellData.drowsy;
                visualScript.source = player;
            }
        }
    }

    /*
    *   CanUseSpell - Checks if any champion has Billia's passive on them, which allows the use of this spell.
    */
    private void CanUseSpell(){
        if(SpellLevel >= 0 && !OnCd){
            List<GameObject> passiveAppliedChamps = GetChampionsWithPassive();
            if(passiveAppliedChamps.Count > 0){
                canUseSpell = true;
                if(spellCDCover != null)
                    spellCDCover.SetActive(false);
            }
            else{
                canUseSpell = false;
                if(spellCDCover != null)
                    spellCDCover.SetActive(true);
            }
        }
    }

    /*
    *   GetChampionsWithPassive - Get all champions with a Billia passive on them.
    *   @return List<GameObject> - List of champion GameObjects with Billia passive dot on them.
    */
    private List<GameObject> GetChampionsWithPassive(){
        List<GameObject> passiveAppliedChamps = new List<GameObject>();
        // Get all StatusEffectManagers.
        Player[] playerScripts = FindObjectsOfType<Player>();
        for (int i = 0; i < playerScripts.Length; i++){
            // If it is a champion and has the Billia dot then add it to the list.
            if(playerScripts[i].statusEffects.CheckForEffectByName(spellData.passiveDot, spellData.passiveDot.name)){
                passiveAppliedChamps.Add(playerScripts[i].gameObject);
            }
        }
        return passiveAppliedChamps;
    }

    /*
    *   Spell_4_SleepProc - Deals fourth spells damage to the enemy hit. Magic damage if target has sleep effect.
    *   @param enemy - GameObject of the enemy hit.
    *   @param isDot - bool of whether or not the damage taken was from a dot.
    */
    public void Spell_4_SleepProc(IUnit unit, bool isDot){
        if(unit is IDamageable){
            // Dots do not proc the sleep.
            if(!isDot){
                if(unit.statusEffects.CheckForEffectWithSource(spellData.drowsy.sleep, player)){
                    // Remove sleep, deal damage and remove function from delegate.
                    unit.statusEffects.RemoveEffect(spellData.drowsy.sleep, player);
                    unit.bonusDamage -= Spell_4_SleepProc;
                    ((IDamageable) unit).TakeDamage(spellData.baseDamage[SpellLevel] + (0.4f * championStats.magicDamage.GetValue()), DamageType.Magic, player, false);
                }
                // If effect fell off before damage was dealt, remove the bonus damage method.
                else{
                    unit.bonusDamage -= Spell_4_SleepProc;
                }
            }
        }
    }
}
