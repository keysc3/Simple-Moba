using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Drowsy")]
public class ScriptableDrowsy : ScriptableEffect
{
    public ScriptableSleep sleep { get; private set; }
    public int spellLevel { get; private set; }
    [field: SerializeField] public float duration { get; private set; }
    [field: SerializeField] public float slowPercent { get; private set; }

    /*
    *   InitializeEffect - Initializes a new drowsy effect.
    *   @param unitCasted - GameObject of the unit that casted the charm.
    *   @param unitEffected - GameObject of the unit effected by the charm.
    */
    public Effect InitializeEffect(ScriptableSleep sleep, int spellLevel, GameObject unitCasted, GameObject unitEffected){
        this.sleep = sleep;
        this.spellLevel = spellLevel;
        return new Drowsy(this, slowPercent, duration, unitCasted, unitEffected);
    }
}
