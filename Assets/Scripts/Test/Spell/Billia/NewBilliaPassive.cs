using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBilliaPassive : InterSpell, INewHasCallback
{
    new private BilliaPassiveData spellData;
    private List<IUnit> passiveApplied = new List<IUnit>();
    public List<ISpell> callbackSet { get; } = new List<ISpell>();

    // Start is called before the first frame update
    protected override void Start(){
        base.Start();
        this.spellData = (BilliaPassiveData) base.spellData;
        if(SpellNum == null){
            SpellNum = spellData.defaultSpellNum;
        }
        //this.passiveData = (BilliaPassiveData) spellData;
    }

    public void OnDisable(){
        foreach(ISpell spell in callbackSet){
            ((IHasHit) spell).spellHitCallback -= Passive;
        }
    }

    /*
    *   Passive - Passive implementation for Billia. Applies a dot to enemies hit by any of Billia's abilities and heals Billia over the duration.
    *   @param enemy - GameObject of the unit to apply the passive to.
    *   @param spellHit - Spell that has hit the GameObject.
    */
    public void Passive(IUnit unit, ISpell spellHit){
        unit.statusEffects.AddEffect(spellData.passiveDot.InitializeEffect(30f, 0, gameObject, (unit as MonoBehaviour).gameObject));
        if(!passiveApplied.Contains(unit)){
            passiveApplied.Add(unit);
            StartCoroutine(PassiveHeal(unit));
        }
    }

    /*
    *   PassiveHeal - Heals Billia while the unit has her passive applied to them.
    *   @param enemy - GameObject the dot is applied to and the passive healing is coming from.
    */
    private IEnumerator PassiveHeal(IUnit unit){
        // Check to make sure the dot is still on the unit.
        while(unit.statusEffects.CheckForEffectWithSource(spellData.passiveDot, gameObject)){
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
    *   @param mySpells - List of Spells to set callbacks.
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
