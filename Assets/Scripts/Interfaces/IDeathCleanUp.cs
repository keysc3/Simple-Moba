using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDeathCleanUp
{
    List<GameObject> activeSpellObjects { get; }
    void OnDeathCleanUp();
}
