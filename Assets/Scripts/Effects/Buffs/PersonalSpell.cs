using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements a spell effect. This is for showing any lingering effects or active spell timers.
*
* @author: Colin Keys
*/
public class PersonalSpell : Effect
{
    private int stacks;
    public int Stacks { 
        get => stacks;
        set => stacks = value >= 0 ? value : 0;
    }
    
    /*
    *   Spell- Initialize a new spell effect.
    *   @param spellEffect - ScriptableSpell of the spell effect to apply.
    *   @param duration - float of the duration for the spell to last.
    *   @param casted - IUnit of the unit that caused the effect.
    *   @param effected - IUnit of the unit that is being affected by the effect.
    */
    public PersonalSpell(ScriptablePersonalSpell spellEffect, float duration, int stacks, IUnit casted, IUnit effected) : base(spellEffect, duration, casted, effected){
        this.stacks = stacks;
    }
}
