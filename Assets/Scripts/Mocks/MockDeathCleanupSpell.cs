using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockDeathCleanupSpell : MockSpell, IDeathCleanUp
{
    public List<GameObject> activeSpellObjects { get; set; }
    public void OnDeathCleanUp(){}
}
