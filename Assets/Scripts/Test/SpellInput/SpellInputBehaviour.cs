using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellInputBehaviour : MonoBehaviour, ISpellInput
{
    private NewSpellInput newSI;

    public bool ButtonClick { get; set; } = false;
    public ISpell LastSpellPressed { get; set; } = null;
    public KeyCode LastButtonPressed { get; set; } = KeyCode.None;
    private Dictionary<string, int> spellLevels;
    public Dictionary<string, int> SpellLevels { get => levelManager.spellLevels; }
    private LevelManager levelManager;
    private NewChampionSpells championSpells;
    private Camera mainCamera;

    void Awake(){
        championSpells = GetComponent<NewChampionSpells>();
        newSI = new NewSpellInput(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        levelManager = GetComponent<IPlayer>().levelManager;
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // If any input is detected.
        if(Input.anyKeyDown){
            newSI.AnyInput();
        }
        // Spell 1
        if(Input.GetKeyDown(KeyCode.Q)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                newSI.SpellButtonPressed(KeyCode.Q, championSpells.spells["Spell_1"]);
            }
        }
        // Spell 2
        if(Input.GetKeyDown(KeyCode.W)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                newSI.SpellButtonPressed(KeyCode.W, championSpells.spells["Spell_2"]);
            }
        }
        // Spell 3
        if(Input.GetKeyDown(KeyCode.E)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                newSI.SpellButtonPressed(KeyCode.E, championSpells.spells["Spell_3"]);
            }
        }
        // Spell 4
        if(Input.GetKeyDown(KeyCode.R)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                newSI.SpellButtonPressed(KeyCode.R, championSpells.spells["Spell_4"]);
            }
        }
        // Left click
        if(Input.GetMouseButtonDown(0)){
            newSI.LeftClick(mainCamera);
        }
    }

    //LateUpdate is called after all Update functions have been called.
    private void LateUpdate(){
        ButtonClick = false;
    }
}
