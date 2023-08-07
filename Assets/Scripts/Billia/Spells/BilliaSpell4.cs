using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements Billia's fourth spell. Billia puts a Drowsy on any champion with her passive dot on them. After a set time of Drowsy the 
* champion gets the Sleep effect. If the champion is woken up from the sleep by non-dot champion damage, they take bonus damage.
*
* @author: Colin Keys
*/
public class BilliaSpell4 : Spell, ICastable
{
    new private BilliaSpell4Data spellData;
    private bool canUseSpell = false;

    /*
    *   BilliaSpell4 - Initialize Billia's fourth spell.
    *   @param championSpells - ChampionSpells instance this spell is a part of.
    *   @param spellNum - string of the spell number this spell is.
    *   @param spellData - SpellData to use.
    */
    public BilliaSpell4(ChampionSpells championSpells, string spellNum, SpellData spellData) : base(championSpells, spellNum, spellData){
        this.spellData = (BilliaSpell4Data) spellData;
        championSpells.updateCallback += CanUseSpell;
        canMove = true;
        isQuickCast = true;
    }

    /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        // Only allow cast if a champion has passive on them.
        if(canUseSpell){
            if(!player.isCasting && championStats.CurrentMana >= spellData.baseMana[player.levelManager.spellLevels[spellNum]-1]){
                // Start cast time then cast the spell.
                championSpells.StartCoroutine(Spell_Cd_Timer(spellData.baseCd[player.levelManager.spellLevels[spellNum]-1], spellNum));
                championSpells.StartCoroutine(CastTime(spellData.castTime, canMove));
                championSpells.StartCoroutine(Spell_4_Cast(GetChampionsWithPassive()));
                // Use mana.
                championStats.UseMana(spellData.baseMana[player.levelManager.spellLevels[spellNum]-1]);
                onCd = true;
            }
        }
    }

    /*
    *   Spell_4_Cast - Casts and starts the cd timer for spell 4. Would have to be refactored to implement projectile destruction/blocking.
    *   @param List<GameObject> - List of GameObjects to apply the drowsy to.
    */
    private IEnumerator Spell_4_Cast(List<GameObject> applyDrowsy){
        while(player.isCasting)
            yield return null;
        championSpells.StartCoroutine(Spell_4_Projectile(applyDrowsy));
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
            Unit enemyUnit = enemy.GetComponent<Unit>();
            if(enemyUnit is Player){
                // Add drowsy to enemy player and update the bonus damage delegate.
                Drowsy newDrowsy = (Drowsy) spellData.drowsy.InitializeEffect(player.levelManager.spellLevels[spellNum]-1, player.gameObject, enemy);
                enemyUnit.statusEffects.AddEffect(newDrowsy);
                enemyUnit.bonusDamage += Spell_4_SleepProc;
                // Animate the drowsy effect.
                GameObject drowsyObject = (GameObject) Object.Instantiate(spellData.drowsyVisual, enemy.transform.position, Quaternion.identity);
                drowsyObject.transform.SetParent(enemy.transform);
                BilliaDrowsyVisual visualScript = drowsyObject.GetComponent<BilliaDrowsyVisual>();
                visualScript.drowsyDuration = newDrowsy.EffectDuration;
                visualScript.drowsy = spellData.drowsy;
                visualScript.source = player.gameObject;
            }
        }
    }

    /*
    *   CanUseSpell - Checks if any champion has Billia's passive on them, which allows the use of this spell.
    */
    private void CanUseSpell(){
        if(player.levelManager.spellLevels[spellNum] > 0){
            List<GameObject> passiveAppliedChamps = GetChampionsWithPassive();
            if(passiveAppliedChamps.Count > 0){
                canUseSpell = true;
                UIManager.instance.SetSpellCoverActive(spellNum, false, player.playerUI);
            }
            else{
                canUseSpell = false;
                UIManager.instance.SetSpellCoverActive(spellNum, true, player.playerUI);
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
        Player[] playerScripts = Object.FindObjectsOfType<Player>();
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
    public void Spell_4_SleepProc(GameObject enemy, bool isDot){
        // Dots do not proc the sleep.
        if(!isDot){
            Unit enemyUnit = enemy.GetComponent<Unit>();
            if(enemyUnit.statusEffects.CheckForEffectWithSource(spellData.drowsy.sleep, player.gameObject)){
                float magicDamage = championStats.magicDamage.GetValue();
                // Remove sleep, deal damage and remove function from delegate.
                enemyUnit.statusEffects.RemoveEffect(spellData.drowsy.sleep, player.gameObject);
                enemyUnit.bonusDamage -= Spell_4_SleepProc;
                enemyUnit.TakeDamage(spellData.baseDamage[player.levelManager.spellLevels[spellNum]-1] + magicDamage, "magic", player.gameObject, false);
            }
            // If effect fell off before damage was dealt, remove the bonus damage method.
            else{
                enemyUnit.bonusDamage -= Spell_4_SleepProc;
            }
        }
    }
}
