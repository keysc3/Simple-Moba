using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements Burge's passive. Burge applies a mark on any enemy player hit by their abilities. If Burge basic attacks a player with a mark
* they expunge the mark <what it do though?>.
*
* @author: Colin Keys
*/
public class BurgePassive : Spell, IHasCallback
{
    public List<ISpell> callbackSet { get; } = new List<ISpell>();

    new private BurgePassiveData spellData;
    private Dictionary<IPlayer, float> nextApply = new Dictionary<IPlayer, float>();

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BurgePassiveData) base.spellData;
        IBasicAttack ba = GetComponent<IBasicAttack>();
        ba.basicAttackHitCallback += ProcPassive;
    }

    /*
    *   ApplyPassive - Applies passive effect to enemy player.
    *   @param unit - IUnit hit by a spell.
    *   @param spellHit - ISpell that hit the unit.
    */
    public void ApplyPassive(IUnit unit, ISpell spellHit){
        if(unit is IPlayer){
            IPlayer playerHit = (IPlayer) unit;
            if(nextApply.ContainsKey(playerHit)){
                if(nextApply[playerHit] <= Time.time){
                    playerHit.statusEffects.AddEffect(spellData.passiveEffect.InitializeEffect(0, player, playerHit));
                }
            }
            else{
                nextApply.Add(playerHit, Time.time);
                playerHit.statusEffects.AddEffect(spellData.passiveEffect.InitializeEffect(0, player, playerHit));
            }
        }
    }

    /*
    *   ProcPassive - Handles passive proc using basic attack callback.
    *   @param hit - IUnit hit by basic attack.
    */
    public void ProcPassive(IUnit hit){
        if(hit is IPlayer){
            if(hit.statusEffects.CheckForEffectByName(spellData.passiveEffect.name)){
                nextApply[(IPlayer) hit] = Time.time + spellData.timeAfterProc;
                Debug.Log("PASSIVE PROC");
                // TODO: Proc passive.
            }
        }
    }

    /*
    *   SetupCallbacks - Sets up the necessary callbacks for the spell.
    *   @param spells - Dictionary of the current spells.
    */
    public void SetupCallbacks(Dictionary<SpellType, ISpell> spells){
        // If the Spell is a DamageSpell then add this spells passive proc to its spell hit callback.
        foreach(KeyValuePair<SpellType, ISpell> entry in spells){
            if(entry.Value is IHasHit){
                ((IHasHit) entry.Value).spellHitCallback += ApplyPassive;
                callbackSet.Add(entry.Value);
            }
        }
    }
}
