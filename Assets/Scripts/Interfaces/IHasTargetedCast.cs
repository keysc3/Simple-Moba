using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements an interface for a spell that has a targeted cast.
*
* @author: Colin Keys
*/
public interface IHasTargetedCast
{
    void AttemptCast(IUnit unit);
    void Cast(IUnit unit);
}
