using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaSpell3 : DamageSpell
{
    public BilliaSpell3(ChampionSpells championSpells) : base(championSpells){

    }

    public override void Cast(){
        spellHitCallback?.Invoke();
        Debug.Log("Spell3");
        Hit();
    }

    public override void Hit(){
        Debug.Log("Spell3Hit");
    }
}
