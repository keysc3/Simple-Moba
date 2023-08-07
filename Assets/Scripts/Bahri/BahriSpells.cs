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
    // Start is called before the first frame update.
    protected override void Start()
    {
        Passive = new BahriPassive(this, passiveData);
        Spell1 = new BahriSpell1(this, spell1Data);
        Spell2 = new BahriSpell2(this, spell2Data);
        Spell3 = new BahriSpell3(this, spell3Data);
        Spell4 = new BahriSpell4(this, spell4Data);
        base.Start();
    }
}
