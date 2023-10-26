using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
* Purpose: Implements an interface for a spell
*
* @author: Colin Keys
*/
public interface ISpell
{
    bool OnCd { get; set; }

    bool CanMove { get; set; }
    bool IsQuickCast { get; set; }
    bool IsDisplayed { get; set; }
    bool IsSummonerSpell { get; set; }
    SpellType SpellNum { get; set; }

    SpellData spellData { get; }
    Transform spellCDTransform { get; set; }
    TMP_Text spellCDText { get; set; }
    Image spellCDImage { get; set; }
    void DisplayCast();
    void HideCast();
    //Camera MainCamera { get; set; }
    //protected ChampionStats championStats;
    //protected NewChampionSpells championSpells;
    //protected Camera mainCamera;
    //protected IPlayer player;
}
