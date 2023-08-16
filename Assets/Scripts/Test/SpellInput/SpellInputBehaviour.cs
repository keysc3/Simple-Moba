using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellInputBehaviour : MonoBehaviour, ISpellInput
{
    private NewSpellInput newSI;

    public bool ButtonClick { get; set; } = false;
    public Spell LastSpellPressed { get; set; } = null;
    public KeyCode LastButtonPressed { get; set; } = KeyCode.None;
    private Dictionary<string, int> spellLevels;
    public Dictionary<string, int> SpellLevels { get => levelManager.spellLevels; }
    private LevelManager levelManager;
    private ChampionSpells championSpells;
    private Camera mainCamera;

    void Awake(){
        levelManager = GetComponent<Player>().levelManager;
        championSpells = GetComponent<ChampionSpells>();
        newSI = new NewSpellInput(this);
    }

    // Start is called before the first frame update
    void Start()
    {
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
                newSI.SpellButtonPressed(KeyCode.Q, championSpells.Spell1);
            }
        }
        // Spell 2
        if(Input.GetKeyDown(KeyCode.W)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                newSI.SpellButtonPressed(KeyCode.W, championSpells.Spell2);
            }
        }
        // Spell 3
        if(Input.GetKeyDown(KeyCode.E)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                newSI.SpellButtonPressed(KeyCode.E, championSpells.Spell3);
            }
        }
        // Spell 4
        if(Input.GetKeyDown(KeyCode.R)){
            if(!Input.GetKey(KeyCode.LeftControl)){
                newSI.SpellButtonPressed(KeyCode.R, championSpells.Spell4);
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
