using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
* Purpose: Implements an interface for a spell that has a hit
*
* @author: Colin Keys
*/
public delegate void SpellHitCallback(IUnit unit, ISpell spellHit);

public interface IHasHit
{
    SpellHitCallback spellHitCallback { get; set; }

    void Hit(IUnit unit);
}
