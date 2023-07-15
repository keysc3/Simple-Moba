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
    new public string name = "New Unit";
    public Sprite icon = null;

    #region "Base Stats"
    public float baseHealth;
    public float magicDamage;
    public float physicalDamage;
    public float speed;
    public float HP5;
    public float armor;
    public float magicResist;
    public string rangeType;
    public float autoRange;
    public float autoWindUp;
    public float attackSpeed;
    public float attackProjectileSpeed;
    #endregion
}
