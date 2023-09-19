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
    public void AddNewSpell(System.Type newSpell, string num, SpellData spellData){
        if(newSpell != null){
            Spell spell = (Spell) gameObject.AddComponent(newSpell);
            spell.spellData = spellData;
            ISpell spellInterface = (ISpell) spell;
            // Check if spell num exists already
            if(spells.ContainsKey(num)){
                // If the spell num already exists and has an associated spell destroy it.
                if(spells[num] != null){
                    Destroy(spells[num] as MonoBehaviour);
                }
                // Set new value.
                spells[num] = spellInterface;
            }
            else
                spells.Add(num, spellInterface);
            // Set spells num.
            spellInterface.SpellNum = num;
            // Setup UI buttons.
            SetupSpellButtons(spellInterface);
            // Setup any callbacks.
            if(spellInterface is IHasCallback){
                ((IHasCallback) spellInterface).SetupCallbacks(spells);
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
            List<KeyCode> inputs = new List<KeyCode>(){KeyCode.None, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.D, KeyCode.F};
            List<string> spellNames = new List<string>(){"Passive", "Spell_1", "Spell_2", "Spell_3", "Spell_4", "SummonerSpell_1", "SummonerSpell_2"};
            int index = spellNames.FindIndex(name => name == num);
            if(index != -1)
                spellButton.keyCode = inputs[index];
            spellButton.SpellInput = gameObject.GetComponent<ISpellInput>();
            // Spell level up button.
            spellsContainer.Find(num + "_Container/SpellContainer/Spell/Icon").GetComponent<Image>().sprite = newSpell.spellData.sprite;
            if(num != "Passive" && !newSpell.IsSummonerSpell){
                SpellLevelUpButton spellLevelUpButton = spellsContainer.Find(num + "_Container/LevelUp/Button").GetComponent<SpellLevelUpButton>();
                spellLevelUpButton.spell = num;
                spellLevelUpButton.LevelManager = player.levelManager;
                spellLevelUpButton.SpellInput = gameObject.GetComponent<ISpellInput>();
            }
        }
    }
}
