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

    public delegate void UpdateCallback(); 
    public UpdateCallback updateCallback;

    public delegate void LateUpdateCallback(); 
    public LateUpdateCallback lateUpdateCallback;

    void Awake(){
        mySpells = new List<Spell>(){passive, spell1, spell2, spell3, spell4};
    }

    protected virtual void Start(){
        CallbackSetup();
    }

    void Update(){
        updateCallback?.Invoke();
    }
    
    void LateUpdate(){
        lateUpdateCallback?.Invoke();
    }

    protected void CallbackSetup(){
        List<Spell> mySpells = new List<Spell>(){passive, spell1, spell2, spell3, spell4};
        foreach(Spell newSpell in mySpells){
            if(newSpell is IHasCallback){
                ((IHasCallback) newSpell).SetupCallbacks(mySpells);
            }
        }
    }
}
