using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Interface for a scripts that has need cleanup on death.
*
* @author: Colin Keys
*/
public interface IDeathCleanUp
{
    List<GameObject> activeSpellObjects { get; }
    void OnDeathCleanUp();
}
