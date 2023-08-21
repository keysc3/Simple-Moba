using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INewHasCallback
{
    List<ISpell> callbackSet { get; }
    void SetupCallbacks(Dictionary<string, ISpell> spells);
}
