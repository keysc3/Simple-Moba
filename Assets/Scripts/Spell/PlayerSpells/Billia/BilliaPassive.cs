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
    public List<ISpell> callbackSet { get; } = new List<ISpell>();

    new private BilliaPassiveData spellData;
    private List<IUnit> passiveApplied = new List<IUnit>();

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BilliaPassiveData) base.spellData;
        if(SpellNum == null){
            SpellNum = spellData.defaultSpellNum;
        }
    }

    public void OnDisable(){
        foreach(ISpell spell in callbackSet){
            ((IHasHit) spell).spellHitCallback -= Passive;
        }
    }

    /*
    *   Passive - Passive implementation for Billia. Applies a dot to enemies hit by any of Billia's abilities and heals Billia over the duration.
    *   @param unit - IUnit of the unit to apply the passive to.
    *   @param spellHit - Spell that has hit the GameObject.
    */
    public void Passive(IUnit unit, ISpell spellHit){
        unit.statusEffects.AddEffect(spellData.passiveDot.InitializeEffect(TotalPassiveDamage(unit), 0, player, unit));
        if(!passiveApplied.Contains(unit)){
            passiveApplied.Add(unit);
            StartCoroutine(PassiveHeal(unit));
        }
    }

    private float TotalPassiveDamage(IUnit unit){
        float percentHealthDamage = unit.unitStats.maxHealth.GetValue() * 0.05f;
        float magicDamage = 0.015f * Mathf.Floor(player.unitStats.magicDamage.GetValue()/100f);
        return percentHealthDamage + magicDamage;
    }

    /*
    *   PassiveHeal - Heals Billia while the unit has her passive applied to them.
    *   @param unit - IUnit the dot is applied to and the passive healing is coming from.
    */
    private IEnumerator PassiveHeal(IUnit unit){
        // Check to make sure the dot is still on the unit.
        while(unit.statusEffects.CheckForEffectWithSource(spellData.passiveDot, player)){
            // Heal the champion amount if unit is a champion.
            if(unit is IPlayer){
                //Debug.Log("Billia passive found on: " + enemy.name);
                float healAmount = (6f + ((84f / 17f) * (float)(player.levelManager.Level - 1)))/spellData.passiveDot.duration[0];
                championStats.CurrentHealth = championStats.CurrentHealth + healAmount;
                Debug.Log("Billia passive healed " + healAmount + " health from passive tick.");
            }
            else if(unit.SUnit is ScriptableMonster){
                if(((ScriptableMonster) unit.SUnit).size == "large"){
                    //Debug.Log("Billia passive found on: " + enemy.name);
                    float healAmount = (39f + ((15f / 17f) * (float)(player.levelManager.Level - 1)))/spellData.passiveDot.duration[0];
                    championStats.CurrentHealth = championStats.CurrentHealth + healAmount;
                    Debug.Log("Billia passive healed " + healAmount + " health from passive tick.");
                }
            }
            yield return new WaitForSeconds(spellData.passiveDot.tickRate);
        }
        passiveApplied.Remove(unit);
    }

    /*
    *   SetupCallbacks - Sets up the necessary callbacks for the spell.
    *   @param spells - Dictionary of the current spells.
    */
    public void SetupCallbacks(Dictionary<string, ISpell> spells){
        // If the Spell is a DamageSpell then add this spells passive proc to its spell hit callback.
        foreach(KeyValuePair<string, ISpell> entry in spells){
            if(entry.Value is IHasHit){
                ((IHasHit) entry.Value).spellHitCallback += Passive;
                callbackSet.Add(entry.Value);
            }
        }
    }
}
