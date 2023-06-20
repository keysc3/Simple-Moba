using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Damage
{

    [field: SerializeField] public float amount { get; private set; }
    [field: SerializeField] public float time { get; private set; }
    [field: SerializeField] public string type { get; private set; }
    //private string spell;
    [field: SerializeField] public GameObject from { get; private set; }

    public Damage(GameObject damageDealer, float damageAmount, string damageType){
        from = damageDealer;
        amount = damageAmount;
        type = damageType;
        time = Time.time;
    }
}
