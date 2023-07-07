using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableEffect for intializing a Charm effect.
*
* @author: Colin Keys
*/
[CreateAssetMenu(menuName = "Effects/Charm")]
public class ScriptableCharm : ScriptableEffect
{
    [SerializeField] private List<float> duration = new List<float>();
    [field: SerializeField] public ScriptableSlow slow { get; private set; }

    /*
    *   InitializeEffect - Initializes a new charm effect with the objects duration and slow percent.
    *   @param spellLevel - int of the charms spell level.
    *   @param unitCasted - GameObject of the unit that casted the charm.
    *   @param unitEffected - GameObject of the unit effected by the charm.
    */
    public Effect InitializeEffect(int spellLevel, GameObject unitCasted, GameObject unitEffected){
        ccValue = 1;
        slow.SetDuration(duration[spellLevel]);
        return new Charm(this, duration[spellLevel], unitCasted, unitEffected);
    }
}
