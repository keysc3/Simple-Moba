using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRespawnCleanUp
{
    List<GameObject> activeSpellObjects { get; }
    void OnRespawnCleanUp();
}
