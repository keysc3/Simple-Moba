using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Creates a ScriptableObject for the champion Bahri that extends the Champion ScriptableObject.
*
* @author: Colin Keys
*/
[System.Serializable]
[CreateAssetMenu(fileName = "New Bahri", menuName = "Unit/Champion/Bahri")]
public class Bahri : Champion
{
    #region "Spell_1"
    public float spell_1_magnitude;
    public float spell_1_speed;
    public float spell_1_minSpeed;
    public float spell_1_maxSpeed;
    public float spell_1_accel;
    #endregion

    #region "Spell_2"
    public float spell_2_magnitude;
    public float spell_2_rotationSpeed;
    public float spell_2_heightOffset;
    public float spell_2_duration;
    public float spell_2_msBoost;
    public float spell_2_radius;
    public float spell_2_speed;
    public float spell_2_multiplier;
    #endregion

    #region "Spell_3"
    public float spell_3_magnitude;
    public float spell_3_speed;
    public float spell_3_speedMultiplier;
    #endregion

    #region "Spell_4"
    public float spell_4_maxMagnitude;
    public float spell_4_speed;
    public float spell_4_radius;
    public float spell_4_charges;
    public float spell_4_duration;
    public float spell_4_takedownDurationIncrease;
    #endregion

}
