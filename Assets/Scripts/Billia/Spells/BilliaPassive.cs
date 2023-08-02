using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements Billia's passive spell. Billia applies a dot to any unit hit by a damage dealing ability. Each dot tick also heals Billia.
*
* @author: Colin Keys
*/
public class BilliaPassive : Spell, IHasCallback
{
    private BilliaPassiveData passiveData;
    [SerializeField] private List<GameObject> passiveApplied = new List<GameObject>();

    /*
    *   BilliaPassive - Initialize Billia's passive spell.
    *   @param championSpells - ChampionSpells instance this spell is a part of.
    *   @param spellNum - string of the spell number this spell is.
    *   @param spellData - SpellData to use.
    */
    public BilliaPassive(ChampionSpells championSpells, string spellNum, SpellData spellData) : base(championSpells, spellNum){
        this.passiveData = (BilliaPassiveData) spellData;
    }

    /*
    *   Passive - Passive implementation for Billia. Applies a dot to enemies hit by any of Billia's abilities and heals Billia over the duration.
    *   @param enemy - GameObject of the unit to apply the passive to.
    *   @param spellHit - Spell that has hit the GameObject.
    */
    public void Passive(GameObject hit, Spell spellHit){
        hit.GetComponent<Unit>().statusEffects.AddEffect(passiveData.passiveDot.InitializeEffect(30f, 0, gameObject, hit));
        if(!passiveApplied.Contains(hit)){
            passiveApplied.Add(hit);
            championSpells.StartCoroutine(PassiveHeal(hit));
        }
    }

    /*
    *   PassiveHeal - Heals Billia while the unit has her passive applied to them.
    *   @param enemy - GameObject the dot is applied to and the passive healing is coming from.
    */
    private IEnumerator PassiveHeal(GameObject enemy){
        // Check to make sure the dot is still on the unit.
        Unit unit = enemy.GetComponent<Unit>();
        while(unit.statusEffects.CheckForEffectWithSource(passiveData.passiveDot, gameObject)){
            // Heal the champion amount if unit is a champion.
            if(unit.unit is ScriptableChampion){
                Debug.Log("Billia passive found on: " + enemy.name);
                float healAmount = (6f + ((84f / 17f) * (float)(levelManager.level - 1)))/passiveData.passiveDot.duration[0];
                championStats.SetHealth(championStats.currentHealth + healAmount);
                Debug.Log("Billia passive healed " + healAmount + " health from passive tick.");
            }
            else if(unit.unit is ScriptableMonster){
                if(((ScriptableMonster) unit.unit).size == "large"){
                    Debug.Log("Billia passive found on: " + enemy.name);
                    float healAmount = (39f + ((15f / 17f) * (float)(levelManager.level - 1)))/passiveData.passiveDot.duration[0];
                    championStats.SetHealth(championStats.currentHealth + healAmount);
                    Debug.Log("Billia passive healed " + healAmount + " health from passive tick.");
                }
            }
            yield return new WaitForSeconds(passiveData.passiveDot.tickRate);
        }
        passiveApplied.Remove(enemy);
    }
    
    /*
    *   SetupCallbacks - Sets up the necessary callbacks for the spell.
    *   @param mySpells - List of Spells to set callbacks.
    */
    public void SetupCallbacks(List<Spell> mySpells){
        // If the Spell is a DamageSpell then add this spells passive proc to its spell hit callback.
        foreach(Spell newSpell in mySpells){
            if(newSpell is DamageSpell && !(newSpell is BilliaPassive)){
                ((DamageSpell) newSpell).spellHitCallback += Passive;
            }
        }
    }
}
