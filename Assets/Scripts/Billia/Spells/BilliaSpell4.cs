using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaSpell4 : Spell
{
    public BilliaSpell4(ChampionSpells championSpells) : base(championSpells){

    }

    public override void Cast(){
        Debug.Log("Spell4");
    }
}
