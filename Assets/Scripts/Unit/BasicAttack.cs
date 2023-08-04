using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements basic attacks for a unit.
*
* @author: Colin Keys
*/
public class BasicAttack : MonoBehaviour
{
    public bool windingUp { get; set; } = false;
    public float nextAuto { get; private set; } = 0.0f;

    [SerializeField] private GameObject attackProjectile;
    private UnitStats unitStats;
    private Unit unit;
    private PlayerController playerController;

    // Called when the script instance is being loaded. 
    protected virtual void Awake(){
        playerController = GetComponent<PlayerController>();
        unit = GetComponent<Unit>();
    }

    protected virtual void Start(){
        unitStats = unit.unitStats;
    }

    /*
    *   Attack - Units basic attack method.
    *   @param target - GameObject of the enemy to attack.
    */
    public void Attack(GameObject target){
        if(unit.SUnit.rangeType == "melee")
            MeleeAttack(target);
        else
            RangeAttack(target);
    }

    /*
    *   MeleeAttack - Melee basic attack method.
    *   @param target - GameObject of the target to attack.
    */
    protected virtual void MeleeAttack(GameObject target){
        AttackHit(target);
    }

    /*
    *   RangeAttack - Ranged basic attack method.
    *   @param target - GameObject of the target to attack.
    */
    protected virtual void RangeAttack(GameObject target){
        StartCoroutine(AttackProjectile(target));
    }
    
    /*
    *   AttackHit - Apply basic attack damage.
    *   @param target - GameObject of the target to attack.
    */
    public virtual void AttackHit(GameObject target){
        float physicalDamage = unitStats.physicalDamage.GetValue();
        target.GetComponent<Unit>().TakeDamage(physicalDamage, "physical", gameObject, false);
    }

    /*
    *   AttackProjectile - Animates the basic attack GameObject towards its target.
    *   @param target - GameObject of the target to attack.
    */
    private IEnumerator AttackProjectile(GameObject target){
        // Create attack GameObject and set necessary variables.
        GameObject projectile = (GameObject)Instantiate(attackProjectile, transform.position, Quaternion.identity);
        BasicAttackTrigger basicAttackTrigger = projectile.gameObject.GetComponent<BasicAttackTrigger>();
        basicAttackTrigger.target = target;
        basicAttackTrigger.basicAttack = this;
        // While the attack still exists animate it.
        while(projectile){
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, target.transform.position, unitStats.attackProjectileSpeed.GetValue() * Time.deltaTime);
            yield return null;
        }
    }

    /*
    *   AutoAttackWindUp - Winds up the players basic attack. Attacks can be canceled if an action is input before the windup finishes.
    */
    public IEnumerator BasicAttackWindUp(){
        float timer = 0.0f;
        unitStats.UpdateAttackSpeed();
        // Wind up time is the time it takes for the player to attack * the percentage of 
        float windUpTime = ((1.0f/unitStats.attackSpeed.GetValue()) * unitStats.autoWindUp.GetValue());
        while(playerController.targetedEnemy != null && windingUp){
            if(timer <= windUpTime){
                // TODO: Animate windup
            }
            else{
                Attack(playerController.targetedEnemy);
                nextAuto = Time.time + 1.0f/unitStats.attackSpeed.GetValue();
                Debug.Log("Next auto in: " + 1.0f/unitStats.attackSpeed.GetValue());
                windingUp = false;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        windingUp = false;
    }

    /*
    *   SetWindingUp - Sets the winding up bool.
    *   @param isWindingUp - bool of whether or not to set winding up to true or false.
    */
    /*public void SetWindingUp(bool isWindingUp){
        windingUp = isWindingUp;
    }*/
}
