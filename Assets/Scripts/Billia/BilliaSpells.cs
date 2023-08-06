using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements spells for Billia.
*
* @author: Colin Keys
*/
public class BilliaSpells : ChampionSpells
{
    // Start is called before the first frame update.
    protected override void Start()
    {
        Passive = new BilliaPassive(this, "Passive", passiveData);
        Spell1 = new BilliaSpell1(this, "Spell_1", spell1Data);
        Spell2 = new BilliaSpell2(this, "Spell_2", spell2Data);
        Spell3 = new BilliaSpell3(this, "Spell_3", spell3Data);
        Spell4 = new BilliaSpell4(this, "Spell_4", spell4Data);
        base.Start();
    }
}
