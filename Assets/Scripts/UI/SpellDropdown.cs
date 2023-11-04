using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
* Purpose: Updates summoner spell dropdown UI and actions on spell selection.
*
* @author: Colin Keys
*/
public class SpellDropdown : MonoBehaviour
{
    public List<SpellData> spellData = new List<SpellData>();
    public List<System.Type> spells = new List<System.Type>(){typeof(Flash), typeof(Ignite), typeof(Ghost)};
    public SpellType spellNum;

    /*
    *   OnValueChanged - Triggered when the drop down value is changed.
    *   @param dropDown - TMP_Dropdown object being changed.
    */
    public void OnValueChanged(TMP_Dropdown dropDown){
        GameObject activePlayer = ActiveChampion.instance.champions[ActiveChampion.instance.ActiveChamp];
        PlayerSpells playerSpells = activePlayer.GetComponent<PlayerSpells>();
        playerSpells.AddNewSpell(spells[dropDown.value], spellNum, spellData[dropDown.value]);
        dropDown.Hide();
    }
}
