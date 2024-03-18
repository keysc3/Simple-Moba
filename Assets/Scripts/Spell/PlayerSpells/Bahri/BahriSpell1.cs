using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements Bahri'a first spell. Bahri throws a damage dealing orb at a target direction that returns after reaching its maximum magnitude.
* Any unit hit on the initial cast takes magic damage and any unit hit by the return takes true damage.
* The orb accelerates upon starting its return until reaching Bahri.
*
* @author: Colin Keys
*/
public class BahriSpell1 : Spell, IHasCast, IHasHitTest
{
    public bool returning { get; set; } = false;
    public SpellHitCallbackTest spellHitCallback { get; set; }

    new private BahriSpell1Data spellData;
    private List<IUnit> enemiesHit = new List<IUnit>();
    private Bounds bahriBounds;
    private bool deathBoundsSet = false;

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BahriSpell1Data) base.spellData;
    }

    // Update is called once per frame
    private void Update()
    {
        // Store current bounds. If unit died set bounds once.
        if(!player.IsDead){
            bahriBounds = player.hitbox.bounds;
        }
        else{
            if(!deathBoundsSet){
                player.hitbox.enabled = true;
                bahriBounds = player.hitbox.bounds;
                player.hitbox.enabled = false;
                deathBoundsSet = true;
            }
        }
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Vector2 size = new Vector2(spellData.orbScale, spellData.magnitude + spellData.orbScale/2f);
        DrawSpellUIHitbox(0, spellData.magnitude/2f + spellData.orbScale/4f, size, true);
    }

    /*
    *   Cast - Casts the spell.
    */
    public bool Cast(){
        if(!OnCd && !player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = spellController.GetTargetDirection();
            player.MouseOnCast = targetDirection;
            // Set the target position to be in the direction of the mouse on cast and at max spell distance from the player.
            Vector3 targetPosition = (targetDirection - transform.position).normalized;
            targetPosition = transform.position + (targetPosition * spellData.magnitude);
            // Start coroutines to handle the spells cast time and animation.
            StartCoroutine(spellController.CastTime(spellData.castTime, spellData.name));
            StartCoroutine(Spell_1_Move(targetPosition));
            // Use mana and set spell on cooldown to true.
            championStats.UseMana(spellData.baseMana[SpellLevel]);
            OnCd = true;
            return true;
        }
        return false;
    }

    /*
    *   Spell_1_Move - Animates the players first spell.
    *   @param targetPosition - Vector3 representing the orbs return point.
    */
    private IEnumerator Spell_1_Move(Vector3 targetPosition){
        List<IUnit> enemiesHit = new List<IUnit>();
        bool isReturning = false;
        // Wait for the spells cast time.
        while(player.IsCasting)
            yield return null;
        // Cooldown starts on cast.
        StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
        // Create the spells object and set necessary values.
        GameObject orb = (GameObject) Instantiate(spellData.orb, transform.position, Quaternion.identity);
        orb.transform.localScale = Vector3.one * spellData.orbScale;
        //BahriSpell1Trigger spell1Trigger = orb.GetComponentInChildren<BahriSpell1Trigger>();
        //spell1Trigger.bahriSpell1 = this;
        //spell1Trigger.unit = player; 
        // Set initial return values.
        //returning = false;
        float returnSpeed = spellData.minSpeed;
        // While the spell is active.
        while(orb){
            Vector3 check = orb.transform.position;
            check.y = GameController.instance.collisionPlane;
            Collider[] hitColliders = Physics.OverlapSphere(check, spellData.orbScale/2f, hitboxMask);
            foreach(Collider hitCollider in hitColliders){
                IUnit hitUnit = hitCollider.gameObject.GetComponentInParent<IUnit>();
                if(hitUnit == null)
                    continue;
                print(hitCollider.transform.parent.name);
                // Call collision handler if enemy is hit.
                if(hitCollider.transform.parent.tag == "Enemy" && hitUnit != player){
                    print(hitCollider.transform.parent.name);
                    if(!enemiesHit.Contains(hitUnit) && hitUnit is IDamageable){
                        Hit(hitUnit, isReturning);
                        enemiesHit.Add(hitUnit);
                    }
                }
            }
            // If the spell hasn't started returning.
            if(!isReturning){
                // If target location has not been reached then move the orb towards the target location.
                if(orb.transform.position != targetPosition){
                    orb.transform.position = Vector3.MoveTowards(orb.transform.position, targetPosition, spellData.speed * Time.deltaTime);
                }
                else{
                    // Set return bools.
                    isReturning = true;
                    enemiesHit.Clear();
                }
            }
            else{
                //  Destroy GameObject if it has returned to Bahri.
                CheckContained(check, orb);
                // The orb is returning, move it towards the player.
                orb.transform.position = Vector3.MoveTowards(orb.transform.position, transform.position, returnSpeed * Time.deltaTime);
                // Speed up the orb as it returns until the max speed is reached.
                returnSpeed += spellData.accel * Time.deltaTime;
                if(returnSpeed > spellData.maxSpeed)
                    returnSpeed = spellData.maxSpeed;
            }
            yield return null;
        }
    }

    /*
    *   CheckContained - Checks if the orb is contained within Bahri.
    */
    private void CheckContained(Vector3 check, GameObject orb){
        check.y = 0f;
        Vector3 pos = transform.position;
        pos.y = 0f;
        if((check - pos).magnitude < ((CapsuleCollider) myCollider).radius)
            Destroy(orb);
        /*Vector3 min = orbCollider.bounds.min;
        Vector3 max = orbCollider.bounds.max;
        if(bahriBounds.Contains(new Vector3(min.x, bahriBounds.center.y, min.z)) && bahriBounds.Contains(new Vector3(max.x, bahriBounds.center.y, max.z))){
            Destroy(transform.parent.gameObject);
        }*/
    }
    
    /*
    *   Hit - Deals first spells damage to the enemy hit. Magic damage on first part then true damage on return.
    *   @param unit - IUnit of the enemy hit.
    */
    public void Hit(IUnit unit, params object[] args){
        spellHitCallback?.Invoke(unit, this);
        float damageValue = spellData.baseDamage[SpellLevel] + (0.45f * championStats.magicDamage.GetValue());
        // Magic damage on first part then true damage on return.
        if(!(bool) args[0]){
            ((IDamageable) unit).TakeDamage(damageValue, DamageType.Magic, player, false);
        }
        else{
            ((IDamageable) unit).TakeDamage(damageValue, DamageType.True, player, false);
        }
    }
}
