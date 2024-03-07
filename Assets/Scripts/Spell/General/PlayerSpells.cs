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

    public delegate void SpellAdded(ISpell newSpell, LevelManager levelManager);
    public event SpellAdded SpellAddedCallback;

    public delegate void SpellsInitialized();
    public event SpellsInitialized SpellsInitializedCallback;

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
            SpellAddedCallback?.Invoke(spellInterface, player.levelManager);
        }
        // Setup each spells callbacks, if any.
        foreach(ISpell spellInterface in objSpells){
            if(spellInterface is IHasCallback)
                ((IHasCallback) spellInterface).SetupCallbacks(spells);
        }
        SpellsInitializedCallback?.Invoke();
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
            CastBarUI castBarUIScript = GetComponentInChildren<CastBarUI>();
            if(castBarUIScript != null)
                castBarUIScript.SpellCallbacks(spell);
            SpellUI spellUIScript = GetComponentInChildren<SpellUI>();
            if(spellUIScript != null)
                spellUIScript.SpellCallbacks(spell);
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
            SpellAddedCallback?.Invoke(newSpell, player.levelManager);
            // Setup any callbacks.
            if(newSpell is IHasCallback){
                ((IHasCallback) newSpell).SetupCallbacks(spells);
            }
        }
    }
}
