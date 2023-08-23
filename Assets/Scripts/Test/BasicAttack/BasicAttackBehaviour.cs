using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttackBehaviour : MonoBehaviour, IBasicAttack
{
    protected BasicAttack nba;
    protected UnitStats unitStats;
    [SerializeField] protected GameObject attackProjectile;

    public bool WindingUp { get; set; } = false;
    public float NextAuto { get; set; } = 0.0f;
    public float AutoWindUp { get => unitStats.autoWindUp.GetValue(); }
    public float AttackSpeed { get => unitStats.attackSpeed.GetValue(); }
    public float PhysicalDamage { get => unitStats.physicalDamage.GetValue(); }
    //public string RangeType { get; }
    

    void Awake(){
        nba = new BasicAttack(this, GetComponent<IPlayerMover>());
        unitStats = GetComponent<IUnit>().unitStats;
        //RangeType = unitStats.rangeType;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(nba.CanAuto(Time.time)){
            StartCoroutine(nba.BasicAttackWindUp());
        }
    }

    public virtual void Attack(GameObject target){}
}
