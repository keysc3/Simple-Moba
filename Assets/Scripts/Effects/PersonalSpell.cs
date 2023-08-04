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
    #region "Stacks property"
    public int Stacks { 
        get { 
            return stacks;
        } 
        set {
            if(value >= 0)
                stacks = value;
            else
                stacks = 0;
        }
    }
    #endregion

    /*
    *   Spell- Initialize a new spell effect.
    *   @param spellEffect - ScriptableSpell of the spell effect to apply.
    *   @param duration - float of the duration for the spell to last.
    *   @param unitCasted - GameObject of the unit that casted the spell.
    *   @param unitEffected - GameObject of the unit that the spell is affecting.
    */
    public PersonalSpell(ScriptablePersonalSpell spellEffect, float duration, int stacks, GameObject unitCasted, GameObject unitEffected) : base(spellEffect, duration, unitCasted, unitEffected){
        this.stacks = stacks;
    }

    /*
    *   StartEffect - Start the spell effect.
    */
    public override void StartEffect(){
    }

    /*
    *   EndEffect - End the spell effect.
    */
    public override void EndEffect(){
    }
}
