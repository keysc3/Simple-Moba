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
    public Dictionary<SpellType, ISpell> spells { get; set; } = new Dictionary<SpellType, ISpell>(); 
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
        foreach(KeyValuePair<SpellType, ISpell> entry in spells){
            if(entry.Value is IDeathCleanUp){
                ((IDeathCleanUp) entry.Value).OnDeathCleanUp();
            }
        }
    }

    /*
    *   AddNewSpell - Adds the spell script to the player and calls the setup method.
    *   @param newSpell - ISpell of the spell being setup and added.
    *   @param num - string of the spells num.
    *   @param spellData - SpellData the spell will use.
    */
    public void AddNewSpell(System.Type newSpell, SpellType num, SpellData spellData){
        if(newSpell != null){
            Spell spell = (Spell) gameObject.AddComponent(newSpell);
            spell.spellData = spellData;
            SetupSpell((ISpell) spell, num);
        }
    }
    /*
    *   SetupSpell - Setup for adding a new spell to the spells dictionary.
    *   @param newSpell - ISpell of the spell being setup and added.
    *   @param num - string of the spells num.
    */
    public void SetupSpell(ISpell newSpell, SpellType num){
        if(newSpell != null){
            // Check if spell num exists already
            if(spells.ContainsKey(num)){
                // If the spell num already exists and has an associated spell destroy it.
                if(spells[num] != null){
                    Destroy(spells[num] as MonoBehaviour);
                }
                // Set new value.
                spells[num] = newSpell;
            }
            else
                spells.Add(num, newSpell);
            // Set spells num.
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
            SpellType num;
            //Spell button
            if(newSpell.SpellNum == SpellType.None)
                num = newSpell.spellData.defaultSpellNum;
            else
                num = newSpell.SpellNum;
            SpellButton spellButton = spellsContainer.Find(num.ToString() + "_Container/SpellContainer/Spell/Button").GetComponent<SpellButton>();
            spellButton.spell = newSpell;
            //TODO: Change this to not be hardcoded using a proper keybind/input system?
            List<KeyCode> inputs = new List<KeyCode>(){KeyCode.None, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.D, KeyCode.F};
            if(num != SpellType.Passive)
                spellButton.keyCode = inputs[((int) num) - 1];
            else
                spellButton.keyCode = inputs[0];
            spellButton.SpellInput = gameObject.GetComponent<ISpellInput>();
            // Spell level up button.
            spellsContainer.Find(num + "_Container/SpellContainer/Spell/Icon").GetComponent<Image>().sprite = newSpell.spellData.sprite;
            if(num != SpellType.Passive && !newSpell.IsSummonerSpell){
                SpellLevelUpButton spellLevelUpButton = spellsContainer.Find(num + "_Container/LevelUp/Button").GetComponent<SpellLevelUpButton>();
                spellLevelUpButton.spell = num;
                spellLevelUpButton.LevelManager = player.levelManager;
                spellLevelUpButton.SpellInput = gameObject.GetComponent<ISpellInput>();
            }
        }
    }
}
