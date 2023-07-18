using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Creates a ScriptableObject for the champion Billia that extends the Champion ScriptableObject.
*
* @author: Colin Keys
*/
[CreateAssetMenu(fileName = "New Billia", menuName = "Unit/Champion/Billia")]
public class Billia : ScriptableChampion
{
    #region "Spell_1"
    [field: SerializeField] public float spell_1_passiveSpeedDuration { get; private set; } 
    [field: SerializeField] public float spell_1_passiveExpireDuration { get; private set; }
    [field: SerializeField] public int spell_1_passiveMaxStacks { get; private set; }
    [field: SerializeField] public List<float> spell_1_passiveSpeed { get; private set; }
    [field: SerializeField] public float spell_1_innerRadius { get; private set; }
    [field: SerializeField] public float spell_1_outerRadius { get; private set; }
    #endregion

    #region "Spell_2"
    [field: SerializeField] public float spell_2_maxMagnitude { get; private set; }
    [field: SerializeField] public float spell_2_dashOffset { get; private set; }
    [field: SerializeField] public float spell_2_innerRadius { get; private set; }
    [field: SerializeField] public float spell_2_outerRadius { get; private set; }
    [field: SerializeField] public float spell_2_dashTime { get; private set; }
    #endregion

    #region "Spell_3"
    [field: SerializeField] public float spell_3_maxLobMagnitude { get; private set; }
    [field: SerializeField] public float spell_3_lobTime { get; private set; }
    [field: SerializeField] public float spell_3_seedSpeed { get; private set; }
    [field: SerializeField] public float spell_3_seedRotation { get; private set; }
    [field: SerializeField] public float spell_3_lobLandHitbox { get; private set; }
    [field: SerializeField] public float spell_3_seedConeAngle { get; private set; }
    [field: SerializeField] public float spell_3_seedConeRadius { get; private set; }
    #endregion

    #region "Spell_4"
    [field: SerializeField] public float spell_4_travelTime { get; private set; }
    #endregion
}
