using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ignite", menuName = "Spell/Summoner Spells/Ignite")]
public class IgniteData : SpellData
{
    [field: SerializeField] public float maxMagnitude { get; private set; }
    [field: SerializeField] public ScriptableDot dot { get; private set; }
    
    protected void OnEnable(){
        type = typeof(Ignite);
    }
}
