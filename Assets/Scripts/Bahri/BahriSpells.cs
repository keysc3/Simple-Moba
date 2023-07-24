using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BahriSpells : ChampionSpells
{
    // Start is called before the first frame update
    void Start()
    {
        passive = new BahriPassive(this, passiveData);
        spell1 = new BahriSpell1(this, spell1Data);
        spell2 = new BahriSpell2(this, spell2Data);
        spell3 = new BahriSpell3(this, spell3Data);
        spell4 = new BahriSpell4(this, spell4Data);
        List<Spell> mySpells = new List<Spell>(){passive, spell1, spell2, spell3, spell4};
        foreach(Spell newSpell in mySpells){
            if(newSpell is IHasCallback){
                ((IHasCallback) newSpell).SetupCallbacks(mySpells);
            }
        }
    }
}
