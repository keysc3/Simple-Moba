using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Purpose: Implements Billia's third spell. Billia lobs a seed at a target location which then rolls until colliding with a GameObject.
* Upon collision the seed explodes in a cone in its forward direction. The seeds hitbox on landing is slightly larger than its rolling hitbox.
*
* @author: Colin Keys
*/
public class BilliaSpell3 : Spell, IHasHit, IHasCast
{
    public SpellHitCallback spellHitCallback { get; set; }

    new private BilliaSpell3Data spellData;
    private float p1_y_offset = 3f;
    private float p2_y = 0.85f;

    // Start is called before the first frame update.
    protected override void Start(){
        base.Start();
        this.spellData = (BilliaSpell3Data) base.spellData;
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        DrawSpellUIHitbox(0, 0f, Vector2.one * spellData.maxLobMagnitude * 2f, false);
        float offset = 0f;
        Vector3 targetDirection = spellController.GetTargetDirection();
        // Set the target position to be in the direction of the mouse on cast.
        Vector3 targetPosition = (targetDirection - transform.position);
        // Set target to lob seed to to max lob distance if casted at a greater distance.
        if(targetPosition.magnitude > spellData.maxLobMagnitude)
            offset = spellData.maxLobMagnitude;
        else
            offset = Mathf.Abs(targetPosition.magnitude);
        
        DrawSpellUIHitbox(1, offset, Vector2.one * spellData.seedScale * spellData.lobLandScale, true);

        float length = 2f;
        Vector2 size = new Vector2(spellData.seedScale, length);
        offset = (offset + (spellData.seedScale*spellData.lobLandScale)/2f + length/2f);
        DrawSpellUIHitbox(2, offset, size, true);
    }

     /*
    *   Cast - Casts the spell.
    */
    public bool Cast(){
        if(!OnCd && !player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            // Start cast time then cast the spell.
            StartCoroutine(spellController.CastTime(spellData.castTime, spellData.name));
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = spellController.GetTargetDirection();
            player.MouseOnCast = targetDirection;
            // Set the target position to be in the direction of the mouse on cast.
            Vector3 targetPosition = (targetDirection - transform.position);
            // Set target to lob seed to to max lob distance if casted at a greater distance.
            if(targetPosition.magnitude > spellData.maxLobMagnitude)
                targetPosition = transform.position + (targetPosition.normalized * spellData.maxLobMagnitude);
            else
                targetPosition = transform.position + (targetDirection - transform.position);
            StartCoroutine(Spell_3_Cast(targetPosition, targetPosition - transform.position));
            // Use mana.
            championStats.UseMana(spellData.baseMana[SpellLevel]);
            OnCd = true;
            return true;
        }
        return false;
    }

    /*
    *   Spell_3_Cast - Casts Billia's third ability after the cast time.
    *   @param targetPosition - Vector3 of where the seed is to be lobbed.
    */
    private IEnumerator Spell_3_Cast(Vector3 targetPosition, Vector3 targetDirection){
        // Wait for cast time.
        while(player.IsCasting)
            yield return null;
        StartCoroutine(spellController.Spell_Cd_Timer(spellData.baseCd[SpellLevel]));
        StartCoroutine(Spell_3_Lob(targetPosition, targetDirection));
    }
    
    /*
    *   Spell_3_Lob - Lobs the seed at the target location over a set time using a Quadratic Bezier Curve.
    *   @param targetPosition - Vector3 of the target position for the seed to land.
    */
    private IEnumerator Spell_3_Lob(Vector3 targetPosition, Vector3 targetDirection){
        // Create spell object.
        GameObject seed = (GameObject) Instantiate(spellData.visualPrefab, transform.position, Quaternion.identity);
        seed.transform.localScale = Vector3.one * spellData.seedScale;
        // Look at roll direction.
        seed.transform.LookAt(seed.transform.position + targetDirection);
        BilliaSpell3Trigger billiaSpell3Trigger = seed.GetComponentInChildren<BilliaSpell3Trigger>();
        billiaSpell3Trigger.billiaSpell3 = this;
        billiaSpell3Trigger.casted = transform;
        // Set p0.
        Vector3 p0 = transform.position;
        // Set p1. X and Z of p1 are halfway between Billia and target position. Y of p1 is an offset value.
        Vector3 p1 = transform.position;
        p1.y = p1.y + p1_y_offset;
        Vector3 dir = targetDirection.normalized;
        float mag = targetDirection.magnitude;
        p1.x = p1.x + (dir.x * (mag/2f));
        p1.z = p1.z + (dir.z * (mag/2f));
        // Set p2. p2 y is a value directly above the ground.
        Vector3 p2 = targetPosition;
        p2.y = p2_y;
        // While lob time has not finished.
        float timer = 0.0f;
        while(timer < spellData.lobTime){
            // Get t value, a value between 0 and 1.
            float t = Mathf.Clamp01(timer/spellData.lobTime);
            // Get the next position on the Quadratic Bezier curve.
            Vector3 point = spellController.QuadraticBezierCurvePoint(t, p0, p1, p2);
            // Set the seeds new position.
            seed.transform.position = point;
            timer += Time.deltaTime;
            yield return null;
        }
        // Set the seeds final point.
        Vector3 lastPoint = spellController.QuadraticBezierCurvePoint(1, p0, p1, p2);
        seed.transform.position = lastPoint;
        billiaSpell3Trigger.enabled = true;
        // Start the seeds rolling.
        StartCoroutine(Spell_3_Move(targetDirection.normalized, seed, billiaSpell3Trigger));
    }

    /*
    *   Spell_3_Move - Instantiates the seed and checks for collision on lob landing. If no landing collision the seed rolls in the 
    *   target forward direction until a collision.
    *   @param targetDirection - Vector3 of the lobbed seeds direction to roll.
    *   @param seed - GameObject of the spell.
    *   @param billaSpell3Trigger - GameObjects collision script.
    */
    private IEnumerator Spell_3_Move(Vector3 targetDirection, GameObject seed, BilliaSpell3Trigger billiaSpell3Trigger){
        billiaSpell3Trigger.forwardDirection = targetDirection;
        SphereCollider seedCollider = seed.GetComponentInChildren<SphereCollider>();
        // Check for lob landing hits.
        List<Collider> lobHit = new List<Collider>(Physics.OverlapSphere(seedCollider.transform.position, 
        (spellData.seedScale/2f) * spellData.lobLandScale, hitboxMask));
        // If a hit then apply damage in a cone in the roll direction.
        if(lobHit.Count > 0){
            foreach(Collider hit in lobHit){
                if(hit.transform.parent != seed.transform && hit.transform.parent != transform){
                    Spell_3_ConeHitbox(seed.transform, hit.transform, targetDirection);
                    Destroy(seed);
                }
            }
        }
        // While seed hasn't been destroyed, no collision.
        while(seed){
            // Move the seed in the target direction and rotate it to animate rolling.
            float step = spellData.seedSpeed * Time.deltaTime;
            seed.transform.position = Vector3.MoveTowards(seed.transform.position, seed.transform.position + targetDirection, step);
            seed.transform.RotateAround(seed.transform.position, seed.transform.right, spellData.seedRotation * Time.deltaTime);
            yield return null;
        }
    }

    /*
    *   Spell_3_ConeHitBox - Checks the seeds post collision cone hitbox for any units to apply the damage to.
    *   @param seed - Transform of the seed.
    *   @param initialHit - Transform of the first hit object.
    *   @param forwardDirection - Vector3 of the roll direction.
    */
    public void Spell_3_ConeHitbox(Transform seed, Transform initialHit, Vector3 forwardDirection){
        if(initialHit.parent.tag == "Enemy"){
            Debug.Log("SEED COLLIDER HIT");
            Hit(initialHit.GetComponentInParent<IUnit>());
        }
        // Check for hits in a sphere with radius of the cone to be checked.
        Collider [] seedConeHits = Physics.OverlapSphere(seed.position, spellData.seedConeRadius, hitboxMask);
        foreach (Collider collider in seedConeHits){
            if(collider.transform.parent.tag == "Enemy" && collider.transform != initialHit){
                // Get the direction to the hit collider.
                Vector3 colliderPos = collider.transform.position;
                Vector3 directionToHit = (colliderPos - seed.transform.position).normalized;
                // If the angle between the roll direction and hit collider direction is within the cone then apply damage.
                if(Vector3.Angle(forwardDirection, directionToHit) < spellData.seedConeAngle/2){
                    Hit(collider.gameObject.GetComponentInParent<IUnit>());
                    Debug.Log("SEED CONE HIT");
                }
            }
        }
    }

    /*
    *   Hit - Deals third spells damage to the enemy hit. Magic damage with a slow on hit.
    *   @param unit - IUnit of the enemy hit.
    */
    public void Hit(IUnit unit, params object[] args){
        spellHitCallback?.Invoke(unit, this);
        if(unit is IDamageable){
            unit.statusEffects.AddEffect(spellData.slowEffect.InitializeEffect(SpellLevel, player, unit));
            ((IDamageable) unit).TakeDamage(spellData.baseDamage[SpellLevel] + (0.6f * championStats.magicDamage.GetValue()), DamageType.Magic, player, false);   
        }
    }
}
