using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaSpell3 : DamageSpell
{
    public BilliaSpell3(ChampionSpells championSpells) : base(championSpells){

    }

    public override void Cast(){
        GameObject hitObject = null;
        Debug.Log("Spell3");
        Hit(hitObject);
    }

    public override void Hit(GameObject hit){
        GameObject hitObject = null;
        spellHitCallback?.Invoke(hitObject);
        Debug.Log("Spell3Hit");
    }
}
