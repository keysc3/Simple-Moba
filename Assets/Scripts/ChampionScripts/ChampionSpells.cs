using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionSpells : MonoBehaviour
{
    [SerializeField] protected SpellData passiveData;
    [SerializeField] protected SpellData spell1Data;
    [SerializeField] protected SpellData spell2Data;
    [SerializeField] protected SpellData spell3Data;
    [SerializeField] protected SpellData spell4Data;

    public Spell passive;
    public Spell spell1;
    public Spell spell2;
    public Spell spell3;
    public Spell spell4;

    public List<Spell> mySpells;
    public List<Effect> initializationEffects = new List<Effect>();
    public List<SpellData> mySpellData;

    public delegate void UpdateCallback(); 
    public UpdateCallback updateCallback;

    public delegate void LateUpdateCallback(); 
    public LateUpdateCallback lateUpdateCallback;

    protected virtual void Awake(){
        mySpells = new List<Spell>(){passive, spell1, spell2, spell3, spell4};
        mySpellData = new List<SpellData>(){passiveData, spell1Data, spell2Data, spell3Data, spell4Data};
        CallbackSetup();
    }

    protected virtual void Start(){
        Player player = GetComponent<Player>();
        foreach(Effect effect in initializationEffects){
            player.statusEffects.AddEffect(effect);
        }
    }

    void Update(){
        updateCallback?.Invoke();
    }
    
    void LateUpdate(){
        lateUpdateCallback?.Invoke();
    }

    protected void CallbackSetup(){
        foreach(Spell newSpell in mySpells){
            if(newSpell is IHasCallback){
                ((IHasCallback) newSpell).SetupCallbacks(mySpells);
            }
        }
    }

    public void OnDeathSpellCleanUp(){
        foreach(Spell newSpell in mySpells){
            if(newSpell is IDeathCleanUp){
                ((IDeathCleanUp) newSpell).OnDeathCleanUp();
            }
        }
    }
}
