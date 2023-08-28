using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableObject for for any unit.
*
* @author: Colin Keys
*/
public class ScriptableUnit : ScriptableObject
{
    [field: SerializeField] new public string name { get; set; } = "New Unit";
    [field: SerializeField] public Sprite icon { get; private set; } = null;

    #region "Base Stats"
    [field: SerializeField] public float baseHealth { get; private set; }
    [field: SerializeField] public float magicDamage { get; private set; }
    [field: SerializeField] public float physicalDamage { get; private set; }
    [field: SerializeField] public float speed { get; private set; }
    [field: SerializeField] public float HP5 { get; private set; }
    [field: SerializeField] public float armor { get; private set; }
    [field: SerializeField] public float magicResist { get; private set; }
    [field: SerializeField] public string rangeType { get; private set; }
    [field: SerializeField] public float autoRange { get; private set; }
    [field: SerializeField] public float autoWindUp { get; private set; }
    [field: SerializeField] public float attackSpeed { get; private set; }
    [field: SerializeField] public float attackProjectileSpeed { get; private set; }
    #endregion
}
