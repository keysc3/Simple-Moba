using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeSpellUI : MonoBehaviour
{
    public List<SpellData> spellData = new List<SpellData>();
    public List<System.Type> spells = new List<System.Type>(){typeof(Flash), typeof(Ghost), typeof(Ignite)};
    public SpellType spellNum;
    private TMP_Text text;
    private PlayerSpells playerSpells;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TMP_Text>();
        playerSpells = ActiveChampion.instance.champions[ActiveChampion.instance.ActiveChamp].GetComponent<PlayerSpells>();
        playerSpells.SpellsInitializedCallback += SetButtonText;
    }

    /*
    *   OnValueChanged - Triggered when the drop down value is changed.
    *   @param dropDown - TMP_Dropdown object being changed.
    */
    public void SummonerSpellChange(){
        GameObject activePlayer = ActiveChampion.instance.champions[ActiveChampion.instance.ActiveChamp];
        PlayerSpells playerSpells = activePlayer.GetComponent<PlayerSpells>();
        print(playerSpells.spells[spellNum].spellData.name);
        int myIndex = spellData.FindIndex(s => s.name == playerSpells.spells[spellNum].spellData.name);
        print(myIndex);
        myIndex++;
        myIndex = myIndex % spellData.Count; 
        text.text = spellData[myIndex].name;
        playerSpells.AddNewSpell(spells[myIndex], spellNum, spellData[myIndex]);
    }

    private void SetButtonText(){
        GameObject activePlayer = ActiveChampion.instance.champions[ActiveChampion.instance.ActiveChamp];
        PlayerSpells playerSpells = activePlayer.GetComponent<PlayerSpells>();
        int myIndex = spellData.FindIndex(s => s.name == playerSpells.spells[spellNum].spellData.name);
        print(myIndex);
        text.text = spellData[myIndex].name;
    }
}
