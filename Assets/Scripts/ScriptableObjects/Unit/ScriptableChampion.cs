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
    #region "Base Stats"
    [field: SerializeField] public float baseMana { get; private set; }
    [field: SerializeField] public float MP5 { get; private set; }
    #endregion

    #region "Growth Statistic"
    [field: SerializeField] public float physicalDamageGrowth { get; set; }
    [field: SerializeField] public float healthGrowth { get; set; }
    [field: SerializeField] public float manaGrowth { get; set; }
    [field: SerializeField] public float HP5Growth { get; set; }
    [field: SerializeField] public float MP5Growth { get; set; }
    [field: SerializeField] public float armorGrowth { get; set; }
    [field: SerializeField] public float magicResistGrowth { get; set; }
    [field: SerializeField] public float attackSpeedGrowth { get; set; }
    #endregion
}