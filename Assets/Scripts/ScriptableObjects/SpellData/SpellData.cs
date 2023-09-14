using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableObject for constant spell data.
*
* @author: Colin Keys
*/
public class SpellData : ScriptableObject
{
    [field: SerializeField] public Sprite sprite { get; private set; }
    [field: SerializeField] public string defaultSpellNum { get; private set; }
    [field: SerializeField] public float castTime { get; private set; }
    [field: SerializeField] public List<float> baseDamage { get; private set; }
    [field: SerializeField] public List<float> baseCd { get; private set; }
    [field: SerializeField] public List<float> baseMana { get; private set; }

    public static SpellData CreateNewInstance(string spellNum){
        SpellData data = CreateInstance<SpellData>();
        data.defaultSpellNum = spellNum;
        return data;
    }
}
