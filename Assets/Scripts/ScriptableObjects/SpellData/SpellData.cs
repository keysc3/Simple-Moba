using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableObject for constant spell data.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "Spell", menuName = "Spell/Base")]
public class SpellData : ScriptableObject
{
    [field: SerializeField] public Sprite sprite { get; private set; }
    [field: SerializeField] public SpellType defaultSpellNum { get; private set; }
    [field: SerializeField] public float castTime { get; private set; }
    [field: SerializeField] public List<float> baseDamage { get; private set; }
    [field: SerializeField] public List<float> baseCd { get; private set; }
    [field: SerializeField] public List<float> baseMana { get; private set; }
    [field: SerializeField] public string spellID { get; private set; }
    [field: SerializeField] public List<GameObject> drawSpellImages {get; private set; }
    [field: SerializeField] public System.Type type {get; protected set; }

    public static SpellData CreateNewInstance(SpellType spellNum){
        SpellData data = CreateInstance<SpellData>();
        data.defaultSpellNum = spellNum;
        return data;
    }
}
