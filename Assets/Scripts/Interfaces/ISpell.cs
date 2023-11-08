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
    void DisplayCast();
    void HideCast();

}
