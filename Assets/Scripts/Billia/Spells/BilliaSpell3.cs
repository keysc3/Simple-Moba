using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaSpell3 : DamageSpell
{
    public BilliaSpell3(ChampionSpells championSpells) : base(championSpells){

    }

    public override void Cast(){
        Debug.Log("Spell3");
        Hit();
    }

    public override void Hit(){
        GameObject hitObject = null;
        spellHitCallback?.Invoke(hitObject);
        Debug.Log("Spell3Hit");
    }
}
