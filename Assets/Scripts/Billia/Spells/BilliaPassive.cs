using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaPassive : Spell, IHasCallback
{
    private BilliaPassiveData passiveData;
    [SerializeField] private List<GameObject> passiveApplied = new List<GameObject>();

    public BilliaPassive(ChampionSpells championSpells, SpellData passiveData) : base(championSpells){
        this.passiveData = (BilliaPassiveData) passiveData;
    }

    public override void Cast(){
        Debug.Log("Passive");
    }

    /*
    *   Passive - Passive implementation for Billia. Applies a dot to enemies hit by any of Billia's abilities and heals Billia over the duration.
    *   @param enemy - GameObject of the unit to apply the passive to.
    */
    public void Passive(GameObject hit){
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
        if(levelManager.spellLevels["Spell_4"] > 0 && passiveApplied.Count > 1){
            UIManager.instance.SetSpellCoverActive(4, false, player.playerUI);
        }
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
        if(levelManager.spellLevels["Spell_4"] > 0 && passiveApplied.Count < 1){
            UIManager.instance.SetSpellCoverActive(4, true, player.playerUI);
        }
        passiveApplied.Remove(enemy);
    }
    
    public void SetupCallbacks(List<Spell> mySpells){
        foreach(Spell newSpell in mySpells){
            if(newSpell is DamageSpell && !(newSpell is BilliaPassive)){
                ((DamageSpell) newSpell).spellHitCallback += Passive;
            }
        }
    }
}
