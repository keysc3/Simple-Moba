using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void SpellHitCallback(IUnit unit, ISpell spellHit);

public interface IHasHit
{
    SpellHitCallback spellHitCallback { get; set; }

    void Hit(IUnit unit);
}
