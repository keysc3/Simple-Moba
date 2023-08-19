using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpell
{
    bool OnCd { get; set; }

    bool CanMove { get; set; }
    bool IsQuickCast { get; set; }
    bool IsDisplayed { get; set; }
    string SpellNum { get; set; }

    SpellData spellData { get; }
    //Camera MainCamera { get; set; }
    //protected ChampionStats championStats;
    //protected NewChampionSpells championSpells;
    //protected Camera mainCamera;
    //protected IPlayer player;
}
