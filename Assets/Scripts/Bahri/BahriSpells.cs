using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BahriSpells : ChampionSpells
{
    // Start is called before the first frame update
    protected override void Start()
    {
        passive = new BahriPassive(this, "Passive", passiveData);
        spell1 = new BahriSpell1(this, "Spell_1", spell1Data);
        spell2 = new BahriSpell2(this, "Spell_2", spell2Data);
        spell3 = new BahriSpell3(this, "Spell_3", spell3Data);
        spell4 = new BahriSpell4(this, "Spell_4",spell4Data);
        base.Start();
    }
}
