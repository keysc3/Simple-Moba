using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewChampionSpells : MonoBehaviour
{
    private Transform spellsContainer;

    public Dictionary<string, ISpell> spells { get; set; } = new Dictionary<string, ISpell>(); 

    private IPlayer player;


    void Awake(){
        player = GetComponent<IPlayer>();
        if(player.playerUI != null)
            spellsContainer = player.playerUI.transform.Find("Player/Combat/SpellsContainer");
    }
    
    // Start is called before the first frame update
    void Start(){
        ISpell[] objSpells = GetComponents<ISpell>();
        foreach(ISpell spellInterface in objSpells){
            spells.Add(spellInterface.SpellNum, spellInterface);
            SetupSpellButtons(spellInterface);
        }
    }

    /*
    *   OnDeathSpellCleanUp - Handles calling OnDeathCleanUp method for any spell that needs death clean up.
    */
    public void OnDeathSpellCleanUp(){
        foreach(KeyValuePair<string, ISpell> entry in spells){
            if(entry.Value is IDeathCleanUp){
                ((IDeathCleanUp) entry.Value).OnDeathCleanUp();
            }
        }
    }

    public void SetSpell(ISpell newSpell, string num){
        if(newSpell != null){
            if(spells[num] != null){
                spells[num].SpellRemoved();
            }
            spells[num] = newSpell;
            newSpell.SpellNum = num;
            SetupSpellButtons(newSpell);
        }
    }
    /*
    *   SetupSpellButtons - Setup for the a spells button click and level up button click.
    *   @param player - Player the spell is for.
    *   @param newSpell - Spell to set the buttons for.
    */
    private void SetupSpellButtons(ISpell newSpell){
        //Spell button
        NewSpellButton spellButton = spellsContainer.Find(newSpell.SpellNum + "_Container/SpellContainer/Spell/Button").GetComponent<NewSpellButton>();
        spellButton.spell = newSpell;
        //TODO: Change this to not be hardcoded using a proper keybind/input system?
        List<KeyCode> inputs = new List<KeyCode>(){KeyCode.None, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R};
        List<string> spellNames = new List<string>(){"Passive", "Spell_1", "Spell_2", "Spell_3", "Spell_4"};
        int index = spellNames.FindIndex(name => name == newSpell.SpellNum);
        if(index != -1)
            spellButton.keyCode = inputs[index];
        spellButton._SI = gameObject.GetComponent<ISpellInput>();
        // Spell level up button.
        spellsContainer.Find(newSpell.SpellNum + "_Container/SpellContainer/Spell/Icon").GetComponent<Image>().sprite = newSpell.spellData.sprite;
        if(newSpell.SpellNum != "Passive"){
            NewSpellLevelUpButton spellLevelUpButton = spellsContainer.Find(newSpell.SpellNum + "_Container/LevelUp/Button").GetComponent<NewSpellLevelUpButton>();
            spellLevelUpButton.spell = newSpell.SpellNum;
            spellLevelUpButton.LevelManager = player.levelManager;
            spellLevelUpButton._SI = gameObject.GetComponent<ISpellInput>();
        }
    }
}
