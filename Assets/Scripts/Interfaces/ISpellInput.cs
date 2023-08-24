using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpellInput
{
    bool ButtonClick { get; set; }
    ISpell LastSpellPressed { get; set; }
    KeyCode LastButtonPressed { get; set; }
    Dictionary<string, int> SpellLevels { get; }
}
