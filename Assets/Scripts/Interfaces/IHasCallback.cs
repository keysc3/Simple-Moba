using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasCallback
{
    void SetupCallbacks(List<Spell> newSpells);
}
