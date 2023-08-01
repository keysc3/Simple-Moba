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
    public Spell passive { get; protected set; }
    public Spell spell1 { get; protected set; }
    public Spell spell2 { get; protected set; }
    public Spell spell3 { get; protected set; }
    public Spell spell4 { get; protected set; }

    public List<Spell> mySpells { get; protected set; }
    public List<Effect> initializationEffects { get; protected set; } = new List<Effect>();
    public List<SpellData> mySpellData { get; protected set; }

    [SerializeField] protected SpellData passiveData;
    [SerializeField] protected SpellData spell1Data;
    [SerializeField] protected SpellData spell2Data;
    [SerializeField] protected SpellData spell3Data;
    [SerializeField] protected SpellData spell4Data;

    public delegate void UpdateCallback(); 
    public UpdateCallback updateCallback;

    public delegate void LateUpdateCallback(); 
    public LateUpdateCallback lateUpdateCallback;

    // Called when the script instance is being loaded.
    protected virtual void Awake(){
        mySpells = new List<Spell>(){passive, spell1, spell2, spell3, spell4};
        mySpellData = new List<SpellData>(){passiveData, spell1Data, spell2Data, spell3Data, spell4Data};
        CallbackSetup();
    }

    // Start is called before the first frame update.
    protected virtual void Start(){
        Player player = GetComponent<Player>();
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
}
