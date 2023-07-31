using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaSpell4 : Spell, ICastable
{
    private BilliaSpell4Data spellData;
    private bool canUseSpell_4 = false;

    public BilliaSpell4(ChampionSpells championSpells, string spellNum, SpellData spellData) : base(championSpells, spellNum){
        this.spellData = (BilliaSpell4Data) spellData;
        championSpells.updateCallback += CanUseSpell_4;
        canMove = true;
        isQuickCast = true;
    }

    /*
    *   Spell_4 - Champions fourth ability method.
    */
    public void Cast(){
        // Only allow cast if a champion has passive on them.
        if(canUseSpell_4){
            if(!player.isCasting && championStats.currentMana >= spellData.baseMana[levelManager.spellLevels[spellNum]-1]){
                // Start cast time then cast the spell.
                championSpells.StartCoroutine(Spell_Cd_Timer(spellData.baseCd[levelManager.spellLevels[spellNum]-1], spellNum));
                championSpells.StartCoroutine(CastTime(spellData.castTime, canMove));
                championSpells.StartCoroutine(Spell_4_Cast(GetChampionsWithPassive()));
                // Use mana.
                championStats.UseMana(spellData.baseMana[levelManager.spellLevels[spellNum]-1]);
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
                Drowsy newDrowsy = (Drowsy) spellData.drowsy.InitializeEffect(levelManager.spellLevels[spellNum]-1, gameObject, enemy);
                enemyUnit.statusEffects.AddEffect(newDrowsy);
                enemyUnit.bonusDamage += Spell_4_SleepProc;
                // Animate the drowsy effect.
                GameObject drowsyObject = (GameObject) Object.Instantiate(spellData.drowsyVisual, enemy.transform.position, Quaternion.identity);
                drowsyObject.transform.SetParent(enemy.transform);
                BilliaDrowsyVisual visualScript = drowsyObject.GetComponent<BilliaDrowsyVisual>();
                visualScript.SetDrowsyDuration(newDrowsy.effectDuration);
                visualScript.SetDrowsy(spellData.drowsy);
                visualScript.SetSource(gameObject);
            }
        }
    }

    /*
    *   CanUseSpell_4 - Checks if any champion has Billia's passive on them, which allows the use of spell 4.
    */
    private void CanUseSpell_4(){
        if(levelManager.spellLevels[spellNum] > 0){
            List<GameObject> passiveAppliedChamps = GetChampionsWithPassive();
            if(passiveAppliedChamps.Count > 0){
                canUseSpell_4 = true;
                UIManager.instance.SetSpellCoverActive(spellNum, false, player.playerUI);
            }
            else{
                canUseSpell_4 = false;
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
            if(enemyUnit.statusEffects.CheckForEffectWithSource(spellData.drowsy.sleep, gameObject)){
                float magicDamage = championStats.magicDamage.GetValue();
                // Remove sleep, deal damage and remove function from delegate.
                enemyUnit.statusEffects.RemoveEffect(spellData.drowsy.sleep, gameObject);
                enemyUnit.bonusDamage -= Spell_4_SleepProc;
                enemyUnit.TakeDamage(spellData.baseDamage[levelManager.spellLevels[spellNum]-1] + magicDamage, "magic", gameObject, false);
            }
            // If effect fell off before damage was dealt, remove the bonus damage method.
            else{
                enemyUnit.bonusDamage -= Spell_4_SleepProc;
            }
        }
    }
}
