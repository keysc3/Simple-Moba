using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

/*
*   Purpose: Handles a spells actions.
*
*   @author: Colin Keys
*/
public class SpellController
{
    private ISpell spell;
    private IPlayer player;
    private ISpellInput spellInput;
    private Camera mainCamera;
    private NavMeshAgent navMeshAgent;
    //private Collider collider;

    public delegate void CastBarUpdate(float timer, ISpell spell);
    public event CastBarUpdate CastBarUpdateCallback;

    public delegate void SpellCDUpdate(SpellType spellType, float cooldownLeft, float spell_cd);
    public event SpellCDUpdate SpellCDUpdateCallback;

    /*
    *   SpellController - Sets up new SpellController.
    *   @param spell - ISpell to use with methods.
    *   @param player - IPlayer to use with methods.
    */
    public SpellController(ISpell spell, IPlayer player){
        this.spell = spell;
        this.player = player;
        mainCamera = Camera.main;
        spellInput = player.GameObject.GetComponent<ISpellInput>();
        navMeshAgent = player.GameObject.GetComponent<NavMeshAgent>();
        //collider = player.GameObject.transform.Find("PlayerCollider").GetComponent<Collider>();
    }

    /*
    *   GetTargetDirection - Gets the mouse world position.
    */
    public Vector3 GetTargetDirection(){
        RaycastHit hitInfo;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        LayerMask groundMask = LayerMask.GetMask("Ground");
        Physics.Raycast(ray, out hitInfo, Mathf.Infinity, groundMask);
        Vector3 targetDirection = hitInfo.point;
        //player.MouseOnCast = targetDirection;
        targetDirection.y = player.GameObject.transform.position.y;
        return targetDirection;
    }

    /*
    *   CastTime - Stops the champion for the duration of the spells cast.
    *   @param castTime - float for the duration to stop the champion for casting.
    */
    public IEnumerator CastTime(){
        float timer = 0.0f;
        player.IsCasting = true;
        player.CurrentCastedSpell = spell;
        // While still casting spell stop the player.
        while(timer < spell.spellData.castTime){
            CastBarUpdateCallback?.Invoke(timer, spell);
            if(!spell.CanMove){
                if(navMeshAgent != null && navMeshAgent.enabled){
                    if(!navMeshAgent.isStopped)
                        navMeshAgent.isStopped = true;
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }
        CastBarUpdateCallback?.Invoke(timer, spell);
        player.IsCasting = false;
        player.CurrentCastedSpell = spell;
        if(navMeshAgent != null && navMeshAgent.enabled)
            navMeshAgent.isStopped = false;
    }

    /*
    *   Spell_Cd_Timer - Times the cooldown of a spell and sets it cd bool to false when its cooldown is complete.
    *   @param spell_cd - float representing the spells cooldown.
    */
    public IEnumerator Spell_Cd_Timer(float spell_cd){
        spell_cd = CalculateCooldown(spell_cd);
        float spell_timer = 0.0f;
        // While spell is still on CD
        while(spell_timer < spell_cd && spell.OnCd){
            SpellCDUpdateCallback?.Invoke(spell.SpellNum, spell_cd - spell_timer, spell_cd);
            spell_timer += Time.deltaTime;
            yield return null;
        }
        SpellCDUpdateCallback?.Invoke(spell.SpellNum, 0f, spell_cd);
        spell.OnCd = false;
    }

    /*
    *   CalculateCooldown - Calculates the cooldown of a spell after applying the champions haste value.
    *   @param baseCD - float of the base cooldown.
    */
    public float CalculateCooldown(float baseCD){
        float haste;
        if(!spell.IsSummonerSpell){
            haste = player.unitStats.haste.GetValue();
        }
        else
            //TODO: Implement summoner spell haste.
            haste = 0f;
        float reducedCD = baseCD*(100f/(100f+player.unitStats.haste.GetValue()));
        return Mathf.Round(reducedCD * 1000.0f) * 0.001f;
    }

    /*
    *   GetPositionOnWalkableNavMesh - Gets a position on the walkable NavMesh given a position.
    *   @param targetPosition - Vector3 of the position being checked.
    *   @param fullClear - bool for whether or not the targetPosition should clear the entire un-walkable for a return value on the opposite side.
    */
    public Vector3 GetPositionOnWalkableNavMesh(Vector3 targetPosition, bool fullClear){
        // Initalize variables 
        NavMeshHit meshHit;
        Vector3 finalPosition = player.Position;
        int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
        // Sample for a point on the walkable navmesh within 4 units of target position.
        if(NavMesh.SamplePosition(targetPosition, out meshHit, 4.0f, walkableMask)){
            finalPosition = meshHit.position;
            Debug.DrawLine(finalPosition, finalPosition + (Vector3.up*10), Color.blue, 30f);
            finalPosition.y = targetPosition.y;
            if(fullClear){
                // If finalPosition does not equal targetPosition, then the targetPosition was not on a walkable area.
                if(targetPosition != finalPosition){
                    // Raycast between the target position and the players current position.
                    // If the ray hits any NavMesh areas besides walkableArea then the RayCast returns true.
                    // The Raycast should always return true because we know the target position is not a walkable area.
                    if(NavMesh.Raycast(player.Position, targetPosition, out meshHit, walkableMask)){
                        // Use the value returned in meshHit to set a new target position on a walkable area in the direction of the original target position.
                        finalPosition = new Vector3(meshHit.position.x, targetPosition.y, meshHit.position.z);
                    }
                }
            }
        }
        return finalPosition;
    }

    /*
    *   CheckInRange - Checks if the player is in range of a given unit.
    *   @param unit - IUnit to check if in range of.
    *   @param maxMagnitude - float of the max range to check for.
    *   @return - bool of wether or not the unit is in range.
    */
    public bool CheckInRange(IUnit unit, float maxMagnitude){
        float distToTarget = (player.Position - unit.Position).magnitude;
        return distToTarget <= maxMagnitude;
    }

    /*
    *   MoveTowardsSpellTarget - Moves the player in range of a given unit.
    *   @param unit - IUnit to move in range of.
    *   @param maxMagnitude - float of the max range to check to.
    */
    public IEnumerator MoveTowardsSpellTarget(IUnit unit, float maxMagnitude){
        while(!CheckInRange(unit, maxMagnitude)){
            if(spellInput.LastSpellPressed != spell && spellInput.LastSpellPressed == null){
                if(!Input.GetMouseButtonDown(1))
                    navMeshAgent.ResetPath();
                yield break;
            }
            navMeshAgent.destination = unit.Position;
            yield return null;
        }
        navMeshAgent.ResetPath();
        ((IHasTargetedCast) spell).Cast(unit);
    }
    
    /*
    *   RaiseSpellCDUpdateEvent - Raises a spell cd update event.
    *   @param spellType - SpellType of the spell raising the event.
    *   @param cooldownLeft - float of the spells cooldown left.
    *   @param spell_cd - float of the spells total cooldown.
    */
    public void RaiseSpellCDUpdateEvent(SpellType spellType, float cooldownLeft, float spell_cd){
        SpellCDUpdateCallback?.Invoke(spellType, cooldownLeft, spell_cd);
    }

    /*
    *   QuadraticBezierCurvePoint - Calculates a point on a quadratic Bezier curve based on the t value.
    *   It is a linear interpolation of two points obtained from linear Bezier curves from p0 to p1 and p1 to p2.
    *   @param t - float of a time value between 0 and 1 for the progress on the curve.
    *   @param p0 - Vector3 of the first control point (starting point).
    *   @param p1 - Vector3 of the second control point (connecting point).
    *   @param p2 - Vector 3 of the third control point (end point).
    */
    public Vector3 QuadraticBezierCurvePoint(float t, Vector3 p0, Vector3 p1, Vector3 p2){
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
