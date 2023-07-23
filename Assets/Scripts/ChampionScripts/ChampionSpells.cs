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

    public delegate void UpdateCallback(); 
    public UpdateCallback updateCallback;

    public delegate void LateUpdateCallback(); 
    public LateUpdateCallback lateUpdateCallback;


    void Update(){
        updateCallback?.Invoke();
    }
    
    void LateUpdate(){
        lateUpdateCallback?.Invoke();
    }
}
