using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasCallback
{
    List<ISpell> callbackSet { get; }
    void SetupCallbacks(Dictionary<string, ISpell> spells);
}
