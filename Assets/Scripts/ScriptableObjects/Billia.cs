using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Billia", menuName = "Unit/Champion/Billia")]
public class Billia : Champion
{
    #region "Spell_1"
    public float spell_1_passiveSpeedDuration;
    public float spell_1_passiveExpireDuration;
    public int spell_1_passiveMaxStacks;
    public List<float> spell_1_passiveSpeed;
    public float spell_1_innerRadius;
    public float spell_1_outerRadius;
    #endregion

    #region "Spell_2"
    public float spell_2_maxMagnitude;
    public float spell_2_dashOffset;
    public float spell_2_innerRadius;
    public float spell_2_outerRadius;
    public float spell_2_dashTime;
    #endregion

    #region "Spell_3"
    public float spell_3_maxLobMagnitude;
    public float spell_3_lobTime;
    public float spell_3_seedSpeed;
    public float spell_3_seedRotation;
    #endregion

    #region "Spell_4"
    #endregion
}
