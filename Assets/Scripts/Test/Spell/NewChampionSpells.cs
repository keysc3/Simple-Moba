using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewChampionSpells : MonoBehaviour
{
    private Transform spellsContainer;

    [SerializeField] protected SpellData passiveData;
    [SerializeField] protected SpellData spell1Data;
    [SerializeField] protected SpellData spell2Data;
    [SerializeField] protected SpellData spell3Data;
    [SerializeField] protected SpellData spell4Data;

    public Dictionary<string, ISpell> spells { get; set; } = new Dictionary<string, ISpell>(); 
    
    private NewSpell passive;
    public NewSpell Passive {
        get => passive;
        set {
            SpellSet(value, ref passive, "Passive");
        }
    }
    private NewSpell spell1;
    public NewSpell Spell1 {
        get => spell1;
        set {
            SpellSet(value, ref spell1, "Spell_1");
        }
    }
    private NewSpell spell2;
    public NewSpell Spell2 {
        get => spell2;
        set {
            SpellSet(value, ref spell2, "Spell_2");
        }
    }
    private NewSpell spell3;
    public NewSpell Spell3 {
        get => spell3;
        set {
            SpellSet(value, ref spell3, "Spell_3");
        }
    }
    private NewSpell spell4;
    public NewSpell Spell4 {
        get => spell4;
        set {
            SpellSet(value, ref spell4, "Spell_4");
        }
    }

    public List<Effect> initializationEffects { get; } = new List<Effect>();
    public List<SpellData> mySpellData { get; } = new List<SpellData>();

    private IPlayer player;

    public delegate void UpdateCallback(); 
    public UpdateCallback updateCallback;

    public delegate void LateUpdateCallback(); 
    public LateUpdateCallback lateUpdateCallback;


    void Awake(){
        player = GetComponent<IPlayer>();
        if(player.playerUI != null)
            spellsContainer = player.playerUI.transform.Find("Player/Combat/SpellsContainer");
    }
    // Start is called before the first frame update
    void Start(){
        mySpellData.AddRange(new List<SpellData>(){passiveData, spell1Data, spell2Data, spell3Data, spell4Data});
        CallbackSetup();
        foreach(Effect effect in initializationEffects){
            player.statusEffects.AddEffect(effect);
        }
        ISpell[] objSpells = GetComponents<ISpell>();
        foreach(ISpell spellInterface in objSpells){
            spells.Add(spellInterface.SpellNum, spellInterface);
        }
    }

    // Update is called once per frame.
    private void Update(){
        updateCallback?.Invoke();
    }
    
    //LateUpdate is called after all Update functions have been called.
    private void LateUpdate(){
        lateUpdateCallback?.Invoke();
    }

    /*
    *   CallbackSetup - Sets up callbacks for any spell that needs callback setup.
    */
    protected void CallbackSetup(){
        List<NewSpell> mySpells = new List<NewSpell>(){passive, spell1, spell2, spell3, spell4};
        foreach(NewSpell newSpell in mySpells){
            if(newSpell is IHasCallback){
                //((IHasCallback) newSpell).SetupCallbacks(mySpells);
            }
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

    private void SpellSet(NewSpell newSpell, ref NewSpell setSpell, string spellNum){
        if(newSpell != null){
            if(setSpell != null)
                setSpell.SpellRemoved();
            setSpell = newSpell;
            setSpell.SpellNum = spellNum;
            //UIManager.instance.SetupSpellButtons(player, setSpell);
        }
    }

    /*
    *   SetupSpellButtons - Setup for the a spells button click and level up button click.
    *   @param player - Player the spell is for.
    *   @param newSpell - Spell to set the buttons for.
    */
    public void SetupSpellButtons(Spell newSpell){
        //Spell button
        SpellButton spellButton = spellsContainer.Find(newSpell.SpellNum + "_Container/SpellContainer/Spell/Button").GetComponent<SpellButton>();
        spellButton.spell = newSpell;
        //TODO: Change this to not be hardcoded using a proper keybind/input system?
        List<KeyCode> inputs = new List<KeyCode>(){KeyCode.None, KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R};
        List<string> spellNames = new List<string>(){"Passive", "Spell_1", "Spell_2", "Spell_3", "Spell_4"};
        int index = spellNames.FindIndex(name => name == newSpell.SpellNum);
        if(index != -1)
            spellButton.keyCode = inputs[index];
        spellButton.playerSpellInput = gameObject.GetComponent<PlayerSpellInput>();
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
