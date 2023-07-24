using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BahriSpells : ChampionSpells
{
    // Start is called before the first frame update
    protected override void Start()
    {
        passive = new BahriPassive(this, passiveData);
        spell1 = new BahriSpell1(this, spell1Data);
        spell2 = new BahriSpell2(this, spell2Data);
        spell3 = new BahriSpell3(this, spell3Data);
        spell4 = new BahriSpell4(this, spell4Data);
        base.Start();
    }
}
