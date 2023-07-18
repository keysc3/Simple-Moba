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
public class Bahri : ScriptableChampion
{
    #region "Spell_1"
    [field: SerializeField] public float spell_1_magnitude { get; private set; }
    [field: SerializeField] public float spell_1_speed { get; private set; }
    [field: SerializeField] public float spell_1_minSpeed { get; private set; }
    [field: SerializeField] public float spell_1_maxSpeed { get; private set; }
    [field: SerializeField] public float spell_1_accel { get; private set; }
    #endregion

    #region "Spell_2"
    [field: SerializeField] public float spell_2_magnitude { get; private set; }
    [field: SerializeField] public float spell_2_rotationSpeed { get; private set; }
    [field: SerializeField] public float spell_2_heightOffset { get; private set; }
    [field: SerializeField] public float spell_2_duration { get; private set; }
    [field: SerializeField] public float spell_2_msBoost { get; private set; }
    [field: SerializeField] public float spell_2_radius { get; private set; }
    [field: SerializeField] public float spell_2_speed { get; private set; }
    [field: SerializeField] public float spell_2_multiplier { get; private set; }
    #endregion

    #region "Spell_3"
    [field: SerializeField] public float spell_3_magnitude { get; private set; }
    [field: SerializeField] public float spell_3_speed { get; private set; }
    [field: SerializeField] public float spell_3_speedMultiplier { get; private set; }
    #endregion

    #region "Spell_4"
    [field: SerializeField] public float spell_4_maxMagnitude { get; private set; }
    [field: SerializeField] public float spell_4_speed { get; private set; }
    [field: SerializeField] public float spell_4_radius { get; private set; }
    [field: SerializeField] public float spell_4_charges { get; private set; }
    [field: SerializeField] public float spell_4_duration { get; private set; }
    [field: SerializeField] public float spell_4_takedownDurationIncrease { get; private set; }
    #endregion

}
