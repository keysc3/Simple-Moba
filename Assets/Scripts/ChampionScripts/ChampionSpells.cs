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
        set => SetSpell(value, ref passive);
    }
    private Spell spell1;
    public Spell Spell1 {
        get => spell1;
        set => SetSpell(value, ref spell1);
    }
    private Spell spell2;
    public Spell Spell2 {
        get => spell2;
        set => SetSpell(value, ref spell2);
    }
    private Spell spell3;
    public Spell Spell3 {
        get => spell3;
        set => SetSpell(value, ref spell3);
    }
    private Spell spell4;
    public Spell Spell4 {
        get => spell4;
        set => SetSpell(value, ref spell4);
    }

    public List<Spell> mySpells { get; } = new List<Spell>();
    public List<Effect> initializationEffects { get; } = new List<Effect>();
    public List<SpellData> mySpellData { get; } = new List<SpellData>();


    public delegate void UpdateCallback(); 
    public UpdateCallback updateCallback;

    public delegate void LateUpdateCallback(); 
    public LateUpdateCallback lateUpdateCallback;

    // Start is called before the first frame update.
    protected virtual void Start(){
        mySpellData.AddRange(new List<SpellData>(){passiveData, spell1Data, spell2Data, spell3Data, spell4Data});
        CallbackSetup();
        Player player = GetComponent<Player>();
        foreach(Effect effect in initializationEffects){
            player.statusEffects.AddEffect(effect);
        }
        UIManager.instance.SetupSpellUI(player);
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
        foreach(Spell newSpell in mySpells){
            if(newSpell is IDeathCleanUp){
                ((IDeathCleanUp) newSpell).OnDeathCleanUp();
            }
        }
    }

    /*
    *   SetSpell - Sets a spell.
    *   @param value - Spell object being used.
    *   @param toSet - Spell variable being set.
    */
    private void SetSpell(Spell value, ref Spell toSet){
        int index = mySpells.FindIndex(spell => spell.spellNum == value.spellNum);
        if(index == -1)
            mySpells.Add(value);
        else
            mySpells[index] = value;
        toSet = value;
    }
}
