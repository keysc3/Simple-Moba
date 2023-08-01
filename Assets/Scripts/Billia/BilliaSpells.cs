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
    // Called when the script instance is being loaded.
    protected override void Awake()
    {
        passive = new BilliaPassive(this, "Passive", passiveData);
        spell1 = new BilliaSpell1(this, "Spell_1", spell1Data);
        spell2 = new BilliaSpell2(this, "Spell_2", spell2Data);
        spell3 = new BilliaSpell3(this, "Spell_3", spell3Data);
        spell4 = new BilliaSpell4(this, "Spell_4", spell4Data);
        base.Awake();
    }
}
