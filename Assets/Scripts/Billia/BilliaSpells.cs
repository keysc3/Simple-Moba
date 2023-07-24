using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaSpells : ChampionSpells
{
    // Start is called before the first frame update
    protected override void Start()
    {
        passive = new BilliaPassive(this, passiveData);
        spell1 = new BilliaSpell1(this, spell1Data);
        spell2 = new BilliaSpell2(this, spell2Data);
        spell3 = new BilliaSpell3(this, spell3Data);
        spell4 = new BilliaSpell4(this, spell4Data);
        base.Start();
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
}
