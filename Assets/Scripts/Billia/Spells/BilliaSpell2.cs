using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaSpell2 : DamageSpell
{
    public BilliaSpell2(ChampionSpells championSpells) : base(championSpells){

    }

    public override void Cast(){
        GameObject hitObject = null;
        Debug.Log("Spell2");
        Hit(hitObject);
    }

    public override void Hit(GameObject hit){
        GameObject hitObject = null;
        spellHitCallback?.Invoke(hitObject);
        Debug.Log("Spell2Hit");
    }
}
