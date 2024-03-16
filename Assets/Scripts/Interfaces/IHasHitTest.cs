using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements an interface for a spell that has a hit
*
* @author: Colin Keys
*/
public delegate void SpellHitCallbackTest(IUnit unit, ISpell spellHit);

public interface IHasHitTest
{
    SpellHitCallbackTest spellHitCallback { get; set; }

    void Hit(IUnit unit, params object[] args);
}
