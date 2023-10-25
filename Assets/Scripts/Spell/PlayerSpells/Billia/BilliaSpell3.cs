using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
        if(SpellNum == null){
            SpellNum = spellData.defaultSpellNum;
        }
    }

    /*
    *   DrawSpell - Method for drawing the spells magnitudes.
    */
    protected override void DrawSpell(){
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.up, spellData.maxLobMagnitude, 1f);

       // Get the players mouse position on spell cast for spells target direction.
        Vector3 targetDirection = spellController.GetTargetDirection();
        // Set the target position to be in the direction of the mouse on cast.
        Vector3 targetPosition = (targetDirection - transform.position);
        // Set target to lob seed to to max lob distance if casted at a greater distance.
        if(targetPosition.magnitude > spellData.maxLobMagnitude)
            targetPosition = transform.position + (targetPosition.normalized * spellData.maxLobMagnitude);
        else
            targetPosition = transform.position + (targetDirection - transform.position);
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(targetPosition, Vector3.up, spellData.visualPrefab.transform.localScale.x, 1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(targetPosition, targetPosition + ((targetPosition - transform.position).normalized * 2f));
    }

     /*
    *   Cast - Casts the spell.
    */
    public void Cast(){
        if(!player.IsCasting && championStats.CurrentMana >= spellData.baseMana[SpellLevel]){
            // Start cast time then cast the spell.
            StartCoroutine(spellController.CastTime(spellData.castTime));
            // Get the players mouse position on spell cast for spells target direction.
            Vector3 targetDirection = spellController.GetTargetDirection();
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
        }
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
        // Look at roll direction.
        seed.transform.LookAt(seed.transform.position + targetDirection);
        BilliaSpell3Trigger billiaSpell3Trigger = seed.GetComponent<BilliaSpell3Trigger>();
        billiaSpell3Trigger.billiaSpell3 = this;
        billiaSpell3Trigger.casted = gameObject;
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
            Vector3 point = QuadraticBezierCurvePoint(t, p0, p1, p2);
            // Set the seeds new position.
            seed.transform.position = point;
            timer += Time.deltaTime;
            yield return null;
        }
        // Set the seeds final point.
        Vector3 lastPoint = QuadraticBezierCurvePoint(1, p0, p1, p2);
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
        LayerMask groundMask = LayerMask.GetMask("Ground", "Projectile");
        // Check for lob landing hits.
        List<Collider> lobHit = new List<Collider>(Physics.OverlapSphere(seed.transform.position, 
        seed.GetComponent<SphereCollider>().radius * spellData.lobLandHitbox, ~groundMask));
        // If a hit then apply damage in a cone in the roll direction.
        if(lobHit.Count > 0){
            if(lobHit[0].gameObject != gameObject){
                Debug.Log("Hit on lob land: " + lobHit[0].gameObject.name);
                Spell_3_ConeHitbox(seed, lobHit[0].gameObject, targetDirection);
                Destroy(seed);
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
    *   @param seed - GameObject of the seed.
    *   @param initialHit - GameObject of the first hit object.
    *   @param forwardDirection - Vector3 of the roll direction.
    */
    public void Spell_3_ConeHitbox(GameObject seed, GameObject initialHit, Vector3 forwardDirection){
        if(initialHit.tag == "Enemy"){
            Hit(initialHit.GetComponent<IUnit>());
        }
        // Check for hits in a sphere with radius of the cone to be checked.
        LayerMask groundMask = LayerMask.GetMask("Ground", "Projectile");
        Collider [] seedConeHits = Physics.OverlapSphere(seed.transform.position, spellData.seedConeRadius, ~groundMask);
        foreach (Collider collider in seedConeHits){
            if(collider.tag == "Enemy" && collider.gameObject != initialHit){
                // Get the direction to the hit collider.
                Vector3 colliderPos = collider.transform.position;
                Vector3 directionToHit = (new Vector3(colliderPos.x, seed.transform.position.y, colliderPos.z) - seed.transform.position).normalized;
                // If the angle between the roll direction and hit collider direction is within the cone then apply damage.
                if(Vector3.Angle(forwardDirection, directionToHit) < spellData.seedConeAngle/2){
                    Hit(collider.gameObject.GetComponent<IUnit>());
                }
            }
        }
    }

    /*
    *   Hit - Deals third spells damage to the enemy hit. Magic damage with a slow on hit.
    *   @param unit - IUnit of the enemy hit.
    */
    public void Hit(IUnit unit){
        spellHitCallback?.Invoke(unit, this);
        if(unit is IDamageable){
            unit.statusEffects.AddEffect(spellData.slowEffect.InitializeEffect(SpellLevel, player, unit));
            ((IDamageable) unit).TakeDamage(spellData.baseDamage[SpellLevel] + (0.6f * championStats.magicDamage.GetValue()), DamageType.Magic, player, false);   
        }
    }

    /*
    *   QuadraticBezierCurvePoint - Calculates a point on a quadratic Bezier curve based on the t value.
    *   It is a linear interpolation of two points obtained from linear Bezier curves from p0 to p1 and p1 to p2.
    *   @param t - float of a time value between 0 and 1 for the progress on the curve.
    *   @param p0 - Vector3 of the first control point (starting point).
    *   @param p1 - Vector3 of the second control point (connecting point).
    *   @param p2 - Vector 3 of the third control point (end point).
    */
    private Vector3 QuadraticBezierCurvePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2){
        // p = ((1-t)^2 * P0) + (2(1-t)t * P1) + (t^2 * P2)
        float coefficient = 1 - t;
        float alpha = Mathf.Pow(coefficient, 2f);
        float beta = 2 * coefficient * t;
        float phi = Mathf.Pow(t, 2f);

        float x = (alpha * p0.x) + (beta * p1.x) + (phi * p2.x);
        float y = (alpha * p0.y) + (beta * p1.y) + (phi * p2.y);
        float z = (alpha * p0.z) + (beta * p1.z) + (phi * p2.z);
        return new Vector3(x, y, z);
    }
}
