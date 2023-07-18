using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableObject with the necessary values and attributes for a champion. Extends Unit.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "New Champion", menuName = "Unit/Champion")]

public class ScriptableChampion : ScriptableUnit
{
    #region "Sprites"
    [field: SerializeField] public Sprite passive_sprite { get; private set; }
    [field: SerializeField] public Sprite spell_1_sprite { get; private set; }
    [field: SerializeField] public Sprite spell_2_sprite { get; private set; }
    [field: SerializeField] public Sprite spell_3_sprite { get; private set; }
    [field: SerializeField] public Sprite spell_4_sprite { get; private set; }
    #endregion

    #region "Base Stats"
    [field: SerializeField] public float baseMana { get; private set; }
    [field: SerializeField] public float MP5 { get; private set; }
    #endregion

    #region "Growth Statistic"
    [field: SerializeField] public float physicalDamageGrowth { get; private set; }
    [field: SerializeField] public float healthGrowth { get; private set; }
    [field: SerializeField] public float manaGrowth { get; private set; }
    [field: SerializeField] public float HP5Growth { get; private set; }
    [field: SerializeField] public float MP5Growth { get; private set; }
    [field: SerializeField] public float armorGrowth { get; private set; }
    [field: SerializeField] public float magicResistGrowth { get; private set; }
    [field: SerializeField] public float attackSpeedGrowth { get; private set; }
    #endregion

    #region "Spell Values Per Level"
    //dmg, cd, cost
    [field: SerializeField] public List<float> spell1BaseDamage { get; private set; }
    [field: SerializeField] public List<float> spell1BaseCd { get; private set; }
    [field: SerializeField] public List<float> spell1BaseMana { get; private set; }

    [field: SerializeField] public List<float> spell2BaseDamage { get; private set; }
    [field: SerializeField] public List<float> spell2BaseCd { get; private set; }
    [field: SerializeField] public List<float> spell2BaseMana { get; private set; }

    [field: SerializeField] public List<float> spell3BaseDamage { get; private set; }
    [field: SerializeField] public List<float> spell3BaseCd { get; private set; }
    [field: SerializeField] public List<float> spell3BaseMana { get; private set; }

    [field: SerializeField] public List<float> spell4BaseDamage { get; private set; }
    [field: SerializeField] public List<float> spell4BaseCd { get; private set; }
    [field: SerializeField] public List<float> spell4BaseMana { get; private set; }
    #endregion

    #region "Spell Attributes"
    [field: SerializeField] public float spell_1_castTime { get; private set; }
    [field: SerializeField] public float spell_2_castTime { get; private set; }
    [field: SerializeField] public float spell_3_castTime { get; private set; }
    [field: SerializeField] public float spell_4_castTime { get; private set; }
    #endregion
}