using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements spells for Bahri.
*
* @author: Colin Keys
*/
public class BahriSpells : ChampionSpells
{
    // Called when the script instance is being loaded.
    protected override void Awake()
    {
        passive = new BahriPassive(this, "Passive", passiveData);
        spell1 = new BahriSpell1(this, "Spell_1", spell1Data);
        spell2 = new BahriSpell2(this, "Spell_2", spell2Data);
        spell3 = new BahriSpell3(this, "Spell_3", spell3Data);
        spell4 = new BahriSpell4(this, "Spell_4",spell4Data);
        base.Awake();
    }
}
