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
    private Collider collider;

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
        collider = player.GameObject.GetComponent<Collider>();
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
        player.MouseOnCast = targetDirection;
        targetDirection.y = collider.bounds.center.y;
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
                if(navMeshAgent != null){
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
        if(navMeshAgent != null)
            navMeshAgent.isStopped = false;
    }

    /*
    *   Spell_Cd_Timer - Times the cooldown of a spell and sets it cd bool to false when its cooldown is complete.
    *   @param spell_cd - float representing the spells cooldown.
    */
    public IEnumerator Spell_Cd_Timer(float spell_cd){
        SpellCDChildrenSetActive(spell.spellCDTransform, true);
        spell_cd = CalculateCooldown(spell_cd);
        float spell_timer = 0.0f;
        // While spell is still on CD
        while(spell_timer < spell_cd){
            spell_timer += Time.deltaTime;
            SpellCDUpdateCallback?.Invoke(spell.SpellNum, spell_cd - spell_timer, spell_cd);
            yield return null;
        }
        spell.OnCd = false;
        SpellCDChildrenSetActive(spell.spellCDTransform, false);
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
    *   SpellCDChildrenSetActive - Sets the children of a transform as active or inactive based on given bool.
    *   @param parent - Transform of parent.
    *   @param isActive - bool of wether to set children active or inactive.
    */
    public void SpellCDChildrenSetActive(Transform parent, bool isActive){
        if(parent != null){
            for(int i = 0; i < parent.childCount; i++){
                parent.GetChild(i).gameObject.SetActive(isActive);
            }
        }
    }

    /*
    *   UpdateActiveSpellSlider - Updates a spells UI component representing an active duration.
    *   @param imageSlider - Image component to update.
    *   @param duration - float of the total duration.
    *   @param active - float of the active duration.
    */
    public void UpdateActiveSpellSlider(Image imageSlider, float duration, float active){
        if(imageSlider != null){
            // Get value between 0 and 1 representing the percent of the spell duration left.
            float fill = 1.0f - (active/duration);
            fill = Mathf.Clamp(fill, 0f, 1f);
            // Set the fill on the active spells slider.
            imageSlider.fillAmount = fill;
        }
    }

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

    public bool CheckInRange(IUnit unit, float maxMagnitude){
        float distToTarget = (player.Position - unit.Position).magnitude;
        return distToTarget <= maxMagnitude;
    }

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
}
