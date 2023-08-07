using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Interface for a spell that has a callback to setup.
*
* @author: Colin Keys
*/
public interface IHasCallback
{
    List<Spell> callbackSet { get; }
    void SetupCallbacks(List<Spell> newSpells);
}
