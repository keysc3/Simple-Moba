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
    new private BahriPassiveData spellData;
    private int passiveStacks = 0;
    private PersonalSpell passive;

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BahriPassiveData) base.spellData;
        if(SpellNum == null)
            SpellNum = spellData.defaultSpellNum;
        // Initialize and add passive effect.
        passive = (PersonalSpell) this.spellData.passivePreset.InitializeEffect(0, gameObject, gameObject);
        player.statusEffects.AddEffect(passive);
        player.score.takedownCallback += Passive;
    }

    void OnDisable(){
        //player.statusEffects.RemoveEffect(passive.effectType, gameObject);
        player.score.takedownCallback -= Passive;
    }
    
     /*
    *   Passive - Handles the passive implementation for Bahri.
    *   @param killed - GameObject of the unit that was killed.
    */
    public void Passive(IUnit killed){
        Debug.Log("Takedown; use passive");
        float healAmount;
        // Heal off champion kill
        if(killed is IPlayer){
            healAmount = ((90f / 17f) * (float)(player.levelManager.Level - 1)) + 75f;
            championStats.CurrentHealth = championStats.CurrentHealth + healAmount + championStats.magicDamage.GetValue();
            Debug.Log("Healed " + healAmount + " health from champion kill.");
        }
        // Heal off minion/monster kills if at 9 stacks.
        else if(killed is IMinion){
            passiveStacks += 1;
            passive.Stacks = passiveStacks;
            if(passiveStacks == 9){
                healAmount = ((60f / 17f) * (float)(player.levelManager.Level - 1)) + 35f;
                championStats.CurrentHealth = championStats.CurrentHealth + healAmount + championStats.magicDamage.GetValue();
                passiveStacks = 0;
                Debug.Log("Healed " + healAmount + " health from minion/monster kill.");
            }
        }
    }
}
