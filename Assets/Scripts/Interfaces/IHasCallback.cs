using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements an interface for a spell that needs callback setup and teardown.
*
* @author: Colin Keys
*/
public interface IHasCallback
{
    List<ISpell> callbackSet { get; }
    void SetupCallbacks(Dictionary<SpellType, ISpell> spells);
}
