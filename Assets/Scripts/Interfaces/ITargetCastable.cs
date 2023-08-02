using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Interface for a cast that needs a target.
*
* @author: Colin Keys
*/
public interface ITargetCastable
{
    void Cast(GameObject target);
}
