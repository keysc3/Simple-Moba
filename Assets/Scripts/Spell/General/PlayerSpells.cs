using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
* Purpose: Handles player spells set up and storing.
*
* @author: Colin Keys
*/
public class PlayerSpells : MonoBehaviour
{
    public Dictionary<string, ISpell> spells { get; set; } = new Dictionary<string, ISpell>(); 
    private Transform spellsContainer;
    private IPlayer player;

    // Called when the script instance is being loaded.
    private void Awake(){
        player = GetComponent<IPlayer>();
    }
    
    // Start is called before the first frame update
    private void Start(){
        // Get spells UI transform
        if(player.playerUI != null)
            spellsContainer = player.playerUI.transform.Find("Player/Combat/SpellsContainer");
        // Add spells on this GameObject to spells dictionary
        ISpell[] objSpells = GetComponents<ISpell>();
        foreach(ISpell spellInterface in objSpells){
            spells.Add(spellInterface.spellData.defaultSpellNum, spellInterface);
            // Setup UI buttons.
            SetupSpellButtons(spellInterface);
        }
        // Setup each spells callbacks, if any.
        foreach(ISpell spellInterface in objSpells){
            if(spellInterface is IHasCallback)
                ((IHasCallback) spellInterface).SetupCallbacks(spells);
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

    /*
    *   AddNewSpell - Setup for adding a new spell to the spells dictionary.
    *   @param newSpell - ISpell of the spell being setup and added.
    *   @param num - string of the spells num.
    */
    public void AddNewSpell(ISpell newSpell, string num){
        if(newSpell != null){
            // Remove spell in new spells slot if necessary.
            if(spells[num] != null){
                Destroy(spells[num] as MonoBehaviour);
            }
            // Add new spell as key.
            spells[num] = newSpell;
            newSpell.SpellNum = num;
            // Setup UI buttons.
            SetupSpellButtons(newSpell);
            // Setup any callbacks.
            if(newSpell is IHasCallback){
                ((IHasCallback) newSpell).SetupCallbacks(spells);
            }
        }
    }

    /*
    *   SetupSpellButtons - Setup for the a spells button click and level up button click.
    *   @param newSpell - Spell to set the buttons for.
    */
    private void SetupSpellButtons(ISpell newSpell){
        if(spellsContainer != null){
            string num;
            //Spell button
            if(newSpell.SpellNum == null)
                num = newSpell.spellData.defaultSpellNum;
            else
                num = newSpell.SpellNum;
            SpellButton spellButton = spellsContainer.Find(num + "_Container/SpellContainer/Spell/Button").GetComponent<SpellButton>();
            spellButton.spell = newSpell;
            //TODO: Change this to not be hardcoded using a proper keybind/input system?
            List<KeyCode> inputs = new List<KeyCode>(){KeyCode.None, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R};
            List<string> spellNames = new List<string>(){"Passive", "Spell_1", "Spell_2", "Spell_3", "Spell_4"};
            int index = spellNames.FindIndex(name => name == num);
            if(index != -1)
                spellButton.keyCode = inputs[index];
            spellButton.SpellInput = gameObject.GetComponent<ISpellInput>();
            // Spell level up button.
            spellsContainer.Find(num + "_Container/SpellContainer/Spell/Icon").GetComponent<Image>().sprite = newSpell.spellData.sprite;
            if(num != "Passive"){
                SpellLevelUpButton spellLevelUpButton = spellsContainer.Find(num + "_Container/LevelUp/Button").GetComponent<SpellLevelUpButton>();
                spellLevelUpButton.spell = num;
                spellLevelUpButton.LevelManager = player.levelManager;
                spellLevelUpButton._SI = gameObject.GetComponent<ISpellInput>();
            }
        }
    }
}
