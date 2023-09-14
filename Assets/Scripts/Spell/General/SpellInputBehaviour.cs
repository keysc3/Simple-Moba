using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*   Purpose: Handles receiving input for spells.
*
*   @author: Colin Keys
*/
public class SpellInputBehaviour : MonoBehaviour, ISpellInput
{
    public bool ButtonClick { get; set; } = false;
    public ISpell LastSpellPressed { get; set; } = null;
    public KeyCode LastButtonPressed { get; set; } = KeyCode.None;

    private SpellInputController spellInputController;
    private Dictionary<string, int> spellLevels;
    public Dictionary<string, int> SpellLevels { get => levelManager.spellLevels; }
    private LevelManager levelManager;
    private PlayerSpells playerSpells;
    private Camera mainCamera;

    // Called when the script instance is being loaded.
    private void Awake(){
        playerSpells = GetComponent<PlayerSpells>();
        spellInputController = new SpellInputController(this);
    }

    // Start is called before the first frame update
    private void Start()
    {
        levelManager = GetComponent<IPlayer>().levelManager;
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        // If any input is detected.
        if(Input.anyKeyDown){
            // If the input detected is different than the last button press and not left click.
            if(!Input.GetKeyDown(LastButtonPressed) && !Input.GetMouseButtonDown(0))
                spellInputController.CheckForUnready();
        }
        // Spell 1
        if(Input.GetKeyDown(KeyCode.Q)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                spellInputController.SpellButtonPressed(KeyCode.Q, playerSpells.spells["Spell_1"]);
            }
        }
        // Spell 2
        if(Input.GetKeyDown(KeyCode.W)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                spellInputController.SpellButtonPressed(KeyCode.W, playerSpells.spells["Spell_2"]);
            }
        }
        // Spell 3
        if(Input.GetKeyDown(KeyCode.E)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                spellInputController.SpellButtonPressed(KeyCode.E, playerSpells.spells["Spell_3"]);
            }
        }
        // Spell 4
        if(Input.GetKeyDown(KeyCode.R)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                spellInputController.SpellButtonPressed(KeyCode.R, playerSpells.spells["Spell_4"]);
            }
        }
        // Spell 4
        if(Input.GetKeyDown(KeyCode.D)){
            spellInputController.SpellButtonPressed(KeyCode.D, playerSpells.spells["SummonerSpell_1"]);
        }
        // Left click
        if(Input.GetMouseButtonDown(0)){
            spellInputController.LeftClick(mainCamera.ScreenPointToRay(Input.mousePosition));
        }
    }

    // LateUpdate is called after all Update functions have been called.
    private void LateUpdate(){
        ButtonClick = false;
    }
}
