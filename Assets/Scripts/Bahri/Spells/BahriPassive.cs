using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BahriPassive : Spell
{
    private BahriPassiveData spellData;
    private int passiveStacks = 0;
    private PersonalSpell passive;

    public BahriPassive(ChampionSpells championSpells, string spellNum, SpellData spellData) : base(championSpells, spellNum){
        this.spellData = (BahriPassiveData) spellData;
        passive = (PersonalSpell) this.spellData.passivePreset.InitializeEffect(0, gameObject, gameObject);
        player.statusEffects.AddEffect(passive);
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
