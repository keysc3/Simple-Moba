using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: ScriptableObject for for any unit.
*
* @author: Colin Keys
*/
public class Unit : ScriptableObject
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
    public float autoRange;
    public float autoWindUp;
    public float attackSpeed;
    public float attackProjectileSpeed;
    #endregion
}
