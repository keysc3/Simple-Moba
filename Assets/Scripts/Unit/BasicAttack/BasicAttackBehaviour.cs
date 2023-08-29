using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
*   Purpose: Handles calling basic attack methods.
*
*   @author: Colin Keys
*/
public class BasicAttackBehaviour : MonoBehaviour, IBasicAttack
{
    public bool WindingUp { get; set; } = false;
    public float NextAuto { get; set; } = 0.0f;
    public float AutoWindUp { get => unitStats.autoWindUp.GetValue(); }
    public float AttackSpeed { get => unitStats.attackSpeed.GetValue(); }
    public float PhysicalDamage { get => unitStats.physicalDamage.GetValue(); }

    protected BasicAttackController basicAttackController;
    protected UnitStats unitStats;
    [SerializeField] protected GameObject attackProjectile;
    //public string RangeType { get; }
    
    // Called when the script instance is being loaded.
    private void Awake(){
        basicAttackController = new BasicAttackController(this, GetComponent<IPlayerMover>(), GetComponent<IPlayer>());
        //RangeType = unitStats.rangeType;
    }

    private void Start(){
        unitStats = GetComponent<IUnit>().unitStats;
    }

    // Update is called once per frame
    private void Update()
    {
        if(basicAttackController.CanAuto(Time.time)){
            StartCoroutine(basicAttackController.BasicAttackWindUp());
        }
    }

    /*
    *   Attack - Method for attacking a target with a basic attack.
    *   @param target - IUnit of the target.
    */
    public virtual void Attack(IUnit target){}
}
