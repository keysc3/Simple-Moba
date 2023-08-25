using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements an interface for a spell input.
*
* @author: Colin Keys
*/
public interface ISpellInput
{
    bool ButtonClick { get; set; }
    ISpell LastSpellPressed { get; set; }
    KeyCode LastButtonPressed { get; set; }
    Dictionary<string, int> SpellLevels { get; }
}
