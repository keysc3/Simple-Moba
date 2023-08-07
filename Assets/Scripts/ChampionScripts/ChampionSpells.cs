using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements spell structure for a champion. There is a passive and 4 spells for a champion.
*
* @author: Colin Keys
*/
public class ChampionSpells : MonoBehaviour
{
    [SerializeField] protected SpellData passiveData;
    [SerializeField] protected SpellData spell1Data;
    [SerializeField] protected SpellData spell2Data;
    [SerializeField] protected SpellData spell3Data;
    [SerializeField] protected SpellData spell4Data;
    
    private Spell passive;
    public Spell Passive {
        get => passive;
        set {
            SpellSet(value, ref passive, "Passive");
        }
    }
    private Spell spell1;
    public Spell Spell1 {
        get => spell1;
        set {
            SpellSet(value, ref spell1, "Spell_1");
        }
    }
    private Spell spell2;
    public Spell Spell2 {
        get => spell2;
        set {
            SpellSet(value, ref spell2, "Spell_2");
        }
    }
    private Spell spell3;
    public Spell Spell3 {
        get => spell3;
        set {
            SpellSet(value, ref spell3, "Spell_3");
        }
    }
    private Spell spell4;
    public Spell Spell4 {
        get => spell4;
        set {
            SpellSet(value, ref spell4, "Spell_4");
        }
    }

    public List<Effect> initializationEffects { get; } = new List<Effect>();
    public List<SpellData> mySpellData { get; } = new List<SpellData>();

    private Player player;

    public delegate void UpdateCallback(); 
    public UpdateCallback updateCallback;

    public delegate void LateUpdateCallback(); 
    public LateUpdateCallback lateUpdateCallback;

    private void Awake(){
        player = GetComponent<Player>();
    }

    // Start is called before the first frame update.
    protected virtual void Start(){
        mySpellData.AddRange(new List<SpellData>(){passiveData, spell1Data, spell2Data, spell3Data, spell4Data});
        CallbackSetup();
        foreach(Effect effect in initializationEffects){
            player.statusEffects.AddEffect(effect);
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
        List<Spell> mySpells = new List<Spell>(){passive, spell1, spell2, spell3, spell4};
        foreach(Spell newSpell in mySpells){
            if(newSpell is IHasCallback){
                ((IHasCallback) newSpell).SetupCallbacks(mySpells);
            }
        }
    }

    /*
    *   OnDeathSpellCleanUp - Handles calling OnDeathCleanUp method for any spell that needs death clean up.
    */
    public void OnDeathSpellCleanUp(){
        List<Spell> mySpells = new List<Spell>(){passive, spell1, spell2, spell3, spell4};
        foreach(Spell newSpell in mySpells){
            if(newSpell is IDeathCleanUp){
                ((IDeathCleanUp) newSpell).OnDeathCleanUp();
            }
        }
    }

    private void SpellSet(Spell newSpell, ref Spell setSpell, string spellNum){
        if(newSpell != null){
            if(setSpell != null)
                setSpell.SpellRemoved();
            setSpell = newSpell;
            setSpell.SpellNum = spellNum;
            UIManager.instance.SetupSpellButtons(player, setSpell);
        }
    }
}
