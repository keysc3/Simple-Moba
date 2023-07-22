using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaPassive : Spell, IHasCallback
{
    public BilliaPassive(ChampionSpells championSpells) : base(championSpells){

    }

    public override void Cast(){
        Debug.Log("Passive");
    }
    
    public void SetupCallbacks(List<Spell> mySpells){
        foreach(Spell newSpell in mySpells){
            if(newSpell is DamageSpell && !(newSpell is BilliaPassive)){
                ((DamageSpell) newSpell).spellHitCallback += Cast;
            }
        }
    }
}
