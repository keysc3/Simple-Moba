using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MockSpellBehaviour : MonoBehaviour, ISpell
{
    public bool OnCd { get; set; }
    public bool CanMove { get; set; }
    public bool IsQuickCast { get; set; }
    public bool IsDisplayed { get; set; }
    public string SpellNum { get; set; }
    public SpellData spellData { get; set; }
    public Transform spellCDTransform { get; set; }
    public TMP_Text spellCDText { get; set; }
    public Image spellCDImage { get; set; }
    public void DisplayCast(){
        IsDisplayed = true;
    }
    public void HideCast(){
        IsDisplayed = false;
    }
}
