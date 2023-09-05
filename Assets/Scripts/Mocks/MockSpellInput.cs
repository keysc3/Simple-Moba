using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockSpellInput : ISpellInput
{
    public bool ButtonClick { get; set; }
    public ISpell LastSpellPressed { get; set; }
    public KeyCode LastButtonPressed { get; set; }
    public Dictionary<string, int> SpellLevels { get; } = new Dictionary<string, int>();
}
