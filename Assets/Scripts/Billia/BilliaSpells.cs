using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilliaSpells : ChampionSpells
{
    // Start is called before the first frame update
    void Start()
    {
        passive = new BilliaPassive(this);
        spell1 = new BilliaSpell1(this);
        spell2 = new BilliaSpell2(this);
        spell3 = new BilliaSpell3(this);
        spell4 = new BilliaSpell4(this);
        List<Spell> mySpells = new List<Spell>(){passive, spell1, spell2, spell3, spell4};
        foreach(Spell newSpell in mySpells){
            if(newSpell is IHasCallback){
                ((IHasCallback) newSpell).SetupCallbacks(mySpells);
            }
        }
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
}
