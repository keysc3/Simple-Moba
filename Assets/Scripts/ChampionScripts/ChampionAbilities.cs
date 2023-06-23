using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

/*
* Purpose: Abstract class for Champion abilities and their cooldowns.
*
* @author: Colin Keys
*/
public abstract class ChampionAbilities : MonoBehaviour
{
    public bool isCasting { get; protected set; }
    public Vector3 mouseOnCast { get; private set; }
    
    protected bool spell_1_onCd = false;
    protected bool spell_2_onCd = false;
    protected bool spell_3_onCd = false;
    protected bool spell_4_onCd = false;
    protected List<GameObject> activeSpellObjects = new List<GameObject>(); 
    protected NavMeshAgent navMeshAgent;
    protected UIManager uiManager;
    protected LevelManager levelManager;
    protected ChampionStats championStats;

    private Collider myCollider;
    private Camera mainCamera;
    

    // Called when the script instance is being loaded.
    protected virtual void Awake(){
        isCasting = false;
        mainCamera = Camera.main;
        uiManager = GetComponent<UIManager>();
        myCollider = GetComponent<Collider>();
        levelManager = GetComponent<LevelManager>();
        championStats = GetComponent<ChampionStats>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    /*
    *   Spell_1 - Champions first ability method.
    */
    public abstract void Spell_1();

    /*
    *   Spell_2 - Champions second ability method.
    */
    public abstract void Spell_2();

    /*
    *   Spell_3 - Champions third ability method.
    */
    public abstract void Spell_3();

    /*
    *   Spell_4 - Champions fourth ability method.
    */
    public abstract void Spell_4();

    /*
    *   Attack - Champions auto attack method.
    *   @param targetedEnemy - GameObject of the enemy to attack.
    */
    public abstract void Attack(GameObject targetedEnemy);

    /*
    *   OnDeathCleanUp - Method for any necessary spell cleanup on death.
    */
    public abstract void OnDeathCleanUp();

    /*
    *   OnRespawnCleanUp - Method for any necessary spell cleanup on respawn.
    */
    public abstract void OnRespawnCleanUp();

    /*
    *   SpellObjectCreated - Adds a spell object to the active spell objects list.
    *   @param spellObject - GameObject of the activated spell object.
    */
    protected void SpellObjectCreated(GameObject spellObject){
        activeSpellObjects.Add(spellObject);
    }

    /*
    *   RemoveDestroyedObject - Method for cleaning up activeSpellObjects list.
    */
    protected void RemoveDestroyedObjects(){
        if(activeSpellObjects.Count > 0){
            for(int i = activeSpellObjects.Count - 1; i >=0; i--){
                if(activeSpellObjects[i] == null)
                    activeSpellObjects.RemoveAt(i);
            }
        }
    }

    /*
    *   GetTargetDirection - Gets and sets the direction of the mouse.
    *   @return targetDirection - A world space position of where the mouse was over.
    */
    protected Vector3 GetTargetDirection(){
        RaycastHit hitInfo;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hitInfo);
        Debug.DrawLine(mainCamera.transform.position, hitInfo.point, Color.red, 20f);
        Vector3 targetDirection = hitInfo.point;
        mouseOnCast = targetDirection;
        targetDirection.y = myCollider.bounds.center.y;
        return targetDirection;
    }

    /*
    *   CastTime - Stops the champion for the duration of the spells cast.
    *   @param castTime - float for the duration to stop the champion for casting.
    */
    protected IEnumerator CastTime(float castTime){
        float timer = 0.0f;
        isCasting = true;
        // While still casting spell stop the player.
        while(timer <= castTime){
            if(!navMeshAgent.isStopped)
                navMeshAgent.isStopped = true;
            timer += Time.deltaTime;
            yield return null;
        }
        isCasting = false;
        navMeshAgent.isStopped = false;
    }

    /*
    *   Spell_Cd_Timer - Times the cooldown of a spell and sets it cd bool to false when its cooldown is complete.
    *   @param spell_cd - float representing the spells cooldown.
    *   @param myResult - Action<bool> method used for returning a value for setting the spell cooldowns onCd value back to false.
    */
    protected IEnumerator Spell_Cd_Timer(float spell_cd, Action<bool> myResult, string spell){
        spell_cd = CalculateCooldown(spell_cd, championStats.haste.GetValue());
        float spell_timer = 0.0f;
        //Debug.Log(spell_timer);
        while(spell_timer <= spell_cd){
            //Debug.Log(spell_timer);
            spell_timer += Time.deltaTime;
            uiManager.UpdateCooldown(spell, spell_cd - spell_timer, spell_cd);
            yield return null;
        }
        uiManager.UpdateCooldown(spell, 0, spell_cd);
        myResult(false);
    }

    /*
    *   CalculateCooldown - Calculates the cooldown of a spell after applying the champions haste value.
    *   @param baseCD - float of the base cooldown.
    *   @param haste - float of the haste value the champ has.
    */
    private float CalculateCooldown(float baseCD, float haste){
        float reducedCD = baseCD*(100f/(100f+haste));
        return Mathf.Round(reducedCD * 1000.0f) * 0.001f;
    }
 
}
