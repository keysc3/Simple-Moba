using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableObject with the necessary values and attributes for a champion. Extends Unit.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "New Champion", menuName = "Unit/Champion")]
public class Champion : Unit
{
    #region "Sprites"
    public Sprite passive_sprite;
    public Sprite spell_1_sprite;
    public Sprite spell_2_sprite;
    public Sprite spell_3_sprite;
    public Sprite spell_4_sprite;
    #endregion

    #region "Base Stats"
    public float baseMana;
    public float MP5;
    #endregion

    #region "Growth Statistic"
    public float physicalDamageGrowth;
    public float healthGrowth;
    public float manaGrowth;
    public float HP5Growth;
    public float MP5Growth;
    public float armorGrowth;
    public float magicResistGrowth;
    public float attackSpeedGrowth;
    #endregion

    #region "Spell Values Per Level"
    //dmg, cd, cost
    public List<float> spell1BaseDamage;
    public List<float> spell1BaseCd;
    public List<float> spell1BaseMana;

    public List<float> spell2BaseDamage;
    public List<float> spell2BaseCd;
    public List<float> spell2BaseMana;

    public List<float> spell3BaseDamage;
    public List<float> spell3BaseCd;
    public List<float> spell3BaseMana;

    public List<float> spell4BaseDamage;
    public List<float> spell4BaseCd;
    public List<float> spell4BaseMana;

    #endregion

    #region "Spell Attributes"
    public float spell_1_castTime;
    public float spell_2_castTime;
    public float spell_3_castTime;
    public float spell_4_castTime;
    #endregion
}