using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements Bahri'a passive spell. Bahri gains a stack on minion and monster takedowns. Upon reaching a number of stacks Bahri
* heals herself an amount and resets the stacks. On Champion takedowns Bahri heals an amount.
*
* @author: Colin Keys
*/
public class BahriPassive : Spell
{
    private BahriPassiveData spellData;
    private int passiveStacks = 0;
    private PersonalSpell passive;

    /*
    *   BahriPassive - Initialize Bahri's passive spell.
    *   @param championSpells - ChampionSpells instance this spell is a part of.
    *   @param spellNum - string of the spell number this spell is.
    *   @param spellData - SpellData to use.
    */
    public BahriPassive(ChampionSpells championSpells, string spellNum, SpellData spellData) : base(championSpells, spellNum){
        this.spellData = (BahriPassiveData) spellData;
        passive = (PersonalSpell) this.spellData.passivePreset.InitializeEffect(0, gameObject, gameObject);
        championSpells.initializationEffects.Add(passive);
        player.score.takedownCallback += Passive;
    }

     /*
    *   Passive - Handles the passive implementation for Bahri.
    *   @param killed - GameObject of the unit that was killed.
    */
    public void Passive(GameObject killed){
        Debug.Log("Takedown; use passive");
        float healAmount;
        // Heal off champion kill
        if(killed.GetComponent<Unit>().unit is ScriptableChampion){
            healAmount = ((90f / 17f) * (float)(levelManager.level - 1)) + 75f;
            championStats.SetHealth(championStats.currentHealth + healAmount + championStats.magicDamage.GetValue());
            Debug.Log("Healed " + healAmount + " health from champion kill.");
        }
        // Heal off minion/monster kills if at 9 stacks.
        else{
            passiveStacks += 1;
            passive.UpdateStacks(passiveStacks);
            if(passiveStacks == 9){
                healAmount = ((60f / 17f) * (float)(levelManager.level - 1)) + 35f;
                championStats.SetHealth(championStats.currentHealth + healAmount + championStats.magicDamage.GetValue());
                passiveStacks = 0;
                Debug.Log("Healed " + healAmount + " health from minion/monster kill.");
            }
        }
    }
}
